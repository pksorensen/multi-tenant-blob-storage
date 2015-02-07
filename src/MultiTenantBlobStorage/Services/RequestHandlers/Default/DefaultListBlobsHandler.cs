using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration;
using SInnovations.Azure.MultiTenantBlobStorage.Configuration.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SInnovations.Azure.MultiTenantBlobStorage.Services.RequestHandlers.Default
{
    public class DefaultListBlobsHandler : IListBlobsHandler
    {
        protected ListBlobOptions Options { get; set; }
        public DefaultListBlobsHandler(MultiTenantBlobStorageOptions options)
        {
            ImplementsRequestTransformer = options.ListBlobOptions.BlobListFilter != null;
            ImplementsHandleRequest = options.ListBlobOptions.BlobListProvider != null;
            Options = options.ListBlobOptions;
        }
        public virtual bool ImplementsHandleRequest { get; private set; }
        public virtual bool ImplementsRequestTransformer
        {
            get;
            private set;
        }


        public async Task Transformer(Stream incoming, Stream outgoing, RequestOptions options)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = false;
            var reader = XmlReader.Create(incoming, readerSettings);

            if (options.ContentType == Constants.ContentTypes.Xml)
            {

                using (var writer = XmlWriter.Create(outgoing, new XmlWriterSettings { Async = true, CloseOutput = false }))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "Blob" || reader.Name == "BlobPrefix")
                        {

                            while (reader.Name == "Blob" || reader.Name == "BlobPrefix")
                            {
                                var node = XNode.ReadFrom(reader);
                                var el = (XElement)node;
                                if (Options.BlobListFilter(el))
                                {
                                    el.WriteTo(writer);
                                    await writer.FlushAsync();
                                }
                            }

                        }


                        WriteShallowNode(reader, writer);

                    }
                }



            }else if(options.ContentType == Constants.ContentTypes.Json)
            {
                var jsonWriter = new JsonTextWriter(new StreamWriter(outgoing));
                jsonWriter.WriteStartObject();
                var serializer = JsonSerializer.CreateDefault();
                while (reader.Read())
                {
                    if (reader.Name == "Blob")
                    {
                        jsonWriter.WriteStartArray();
                        while (reader.Name == "Blob")
                        {
                            var node = XNode.ReadFrom(reader);
                            var el = (XElement)node;
                            if (Options.BlobListFilter(el))
                            {
                                serializer.Serialize(jsonWriter, node);
                            }
                           
                        }

                    }
                    else
                    {
                        WriteShallowNode(reader, jsonWriter);
                    }

                }
                jsonWriter.WriteEndObject();
                jsonWriter.Flush();
            }

        }
        void WriteShallowNode(XmlReader reader, JsonWriter jsonWriter = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (jsonWriter == null)
            {
                throw new ArgumentNullException("reader");
            }


            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (jsonWriter != null)
                        WriteJsonElement(reader, jsonWriter);
                    break;
                case XmlNodeType.Text:
                    jsonWriter.WriteValue(reader.Value);
                    break;
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    jsonWriter.WriteWhitespace(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    //jsonWriter.WriteCData(reader.Value);
                    break;
                case XmlNodeType.EntityReference:
                    //    jsonWriter.WriteEntityRef(reader.Name);
                    break;
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.ProcessingInstruction:
                    //    jsonWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                case XmlNodeType.DocumentType:
                    //    jsonWriter.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    break;
                case XmlNodeType.Comment:
                    jsonWriter.WriteComment(reader.Value);
                    break;
                case XmlNodeType.EndElement:
                    jsonWriter.WriteEndObject();
                    break;
            }
        }

        void WriteShallowNode(XmlReader reader, XmlWriter xmlWriter = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (xmlWriter == null)
            {
                throw new ArgumentNullException("reader");
            }


            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    WriteXmlElement(reader, xmlWriter);

                    break;
                case XmlNodeType.Text:
                    xmlWriter.WriteString(reader.Value);
                    break;
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    xmlWriter.WriteWhitespace(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    xmlWriter.WriteCData(reader.Value);
                    break;
                case XmlNodeType.EntityReference:
                    xmlWriter.WriteEntityRef(reader.Name);
                    break;
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.ProcessingInstruction:
                    xmlWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                case XmlNodeType.DocumentType:
                    xmlWriter.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                    break;
                case XmlNodeType.Comment:
                    xmlWriter.WriteComment(reader.Value);
                    break;
                case XmlNodeType.EndElement:
                    xmlWriter.WriteFullEndElement();
                    break;
            }
        }

        private void WriteJsonElement(XmlReader reader, JsonWriter writer)
        {
            writer.WritePropertyName(reader.LocalName);
            bool started = false;
            if (reader.HasAttributes)
            {
                started = true;
                writer.WriteStartObject();
                while (reader.MoveToNextAttribute())
                {
                    writer.WritePropertyName(reader.Name);
                    writer.WriteValue(reader.Value);
                    //  Console.WriteLine(" {0}={1}", reader.Name, reader.Value);
                }
                // Move the reader back to the element node.
                reader.MoveToElement();
            }

            if (reader.IsEmptyElement)
            {
                if (!started)
                    writer.WriteStartObject();
                writer.WriteEndObject();
            }
        }

        private static void WriteXmlElement(XmlReader reader, XmlWriter writer)
        {
            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
            writer.WriteAttributes(reader, true);
            if (reader.IsEmptyElement)
            {
                writer.WriteEndElement();
            }
        }





        public async Task HandleRequest(Microsoft.Owin.IOwinContext context, ResourceContext resourceContext, RequestOptions options)
        {
            var blobs = await Options.BlobListProvider();

            var writer = XmlWriter.Create(context.Response.Body);
           // writer.

        }
    }
}
