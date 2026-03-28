#if NET6_0_OR_GREATER
using SaxonCS = Saxon.Api; // For .NET 6.0
using Frends.Saxon.Activation;
#else
using SaxonHE = Saxon.Api; // For .NET Framework (e.g., net452)
#endif

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Newtonsoft.Json;
#pragma warning disable 1591

namespace Frends.Xml
{
    public class Xml
    {
        /// <summary>
        /// Query XML with XPath and return a list of results. See: https://github.com/FrendsPlatform/Frends.Xml
        /// </summary>
        /// <returns>Object { List &lt;object&gt; Data, List&lt;JToken&gt; ToJson(),JToken ToJson(int index) }</returns>
        public static QueryResults XpathQuery([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
#if NET6_0_OR_GREATER
            return XpathQueryCS(input, options);
#else
            return XpathQueryHE(input, options);
#endif
        }

#if NET6_0_OR_GREATER

        private static QueryResults XpathQueryCS([PropertyTab]QueryInput input, [PropertyTab]QueryOptions options)
        {
            var xPathSelector = SetupXPathSelectorCS(input, options);
            var result = xPathSelector.Evaluate().GetList().Cast<SaxonCS.XdmItem>();

            if (options.ThrowErrorOnEmptyResults && !result.Any())
            {
                throw new NullReferenceException($"Could not find any nodes with XPath: {input.XpathQuery}");
            }

            return new QueryResults(result);
        }
#else
        /// <summary>
        /// Query XML with XPath and return a list of results. See: https://github.com/FrendsPlatform/Frends.Xml
        /// </summary>
        /// <returns>Object { List &lt;object&gt; Data, List&lt;JToken&gt; ToJson(),JToken ToJson(int index) }</returns>
        private static QueryResults XpathQueryHE([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
            var xPathSelector = SetupXPathSelectorHE(input, options);
            var result = xPathSelector.Evaluate().GetList().Cast<SaxonHE.XdmItem>();

            if (options.ThrowErrorOnEmptyResults && !result.Any())
            {
                throw new NullReferenceException($"Could not find any nodes with XPath: {input.XpathQuery}");
            }

            return new QueryResults(result);
        }
#endif

        /// <summary>
        /// Query XML with XPath and return a single result. See: https://github.com/FrendsPlatform/Frends.Xml
        /// </summary>
        /// <returns>Object { object Data, JToken ToJson() } </returns>
        public static QuerySingleResults XpathQuerySingle([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
#if NET6_0_OR_GREATER
            return XpathQuerySingleCS(input, options);
#else
            return XpathQuerySingleHE(input, options);
#endif
        }


#if NET6_0_OR_GREATER
        private static QuerySingleResults XpathQuerySingleCS([PropertyTab]QueryInput input, [PropertyTab]QueryOptions options)
        {
            var xPathSelector = SetupXPathSelectorCS(input, options);
            
            var result = xPathSelector.EvaluateSingle();

            if (options.ThrowErrorOnEmptyResults && result == null)
            {
                throw new NullReferenceException($"Could not find any nodes with XPath: {input.XpathQuery}");
            }

            return new QuerySingleResults(result);
        }

#else
        private static QuerySingleResults XpathQuerySingleHE([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
            var xPathSelector = SetupXPathSelectorHE(input, options);

            var result = xPathSelector.EvaluateSingle();

            if (options.ThrowErrorOnEmptyResults && result == null)
            {
                throw new NullReferenceException($"Could not find any nodes with XPath: {input.XpathQuery}");
            }

            return new QuerySingleResults(result);
        }
#endif
        public static string Transform(TransformInput input)
        {
#if NET6_0_OR_GREATER
            return TransformWithSaxonCS(input);
#else
            return TransformWithSaxonHE(input);
#endif
        }

#if NET6_0_OR_GREATER
        private static string TransformWithSaxonCS(TransformInput input)
        {
            var processor = Saxon.Activation.Activator.Activate();
            var compiler = processor.NewXsltCompiler();

            using (var stringReader = new StringReader(input.Xslt))
            {
                var executable = compiler.Compile(stringReader);
                var transformer = executable.Load();


                using (var inputStream = new MemoryStream())
                {
                    //XmlDocument always produces MemoryStream where its encoding matches the input XML's declaration
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.PreserveWhitespace = true;
                    xmldoc.LoadXml(input.Xml);
                    xmldoc.Save(inputStream);
                    xmldoc = null;
                    inputStream.Position = 0;
                    var baseUri = DetectAndSetBaseUri(input.Xml);
                    transformer.SetInputStream(inputStream, baseUri);

                    input.XsltParameters?.ToList().ForEach(x => transformer.SetParameter(new SaxonCS.QName(x.Name), new SaxonCS.XdmAtomicValue(x.Value)));

                    using (var stringWriter = new StringWriter())
                    {
                        var serializer = processor.NewSerializer();
                        serializer.OutputWriter = stringWriter;
                        transformer.Run(serializer);
                        var output = stringWriter.GetStringBuilder().ToString();
                        output = output.Replace("\n", Environment.NewLine);
                        return output;
                    }
                }
            }
        }

#else
        /// <summary>
        /// Create a XSLT transformation for .Net 4.5.2. See: https://github.com/FrendsPlatform/Frends.Xml
        /// </summary>
        /// <returns>string</returns>
        private static string TransformWithSaxonHE(TransformInput input)
        {
            var processor = new SaxonHE.Processor();
            var compiler = processor.NewXsltCompiler();

            using (var stringReader = new StringReader(input.Xslt))
            {
                var executable = compiler.Compile(stringReader);
                var transformer = executable.Load();


                using (var inputStream = new MemoryStream())
                {
                    //XmlDocument always produces MemoryStream where its encoding matches the input XML's declaration
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.PreserveWhitespace = true;
                    xmldoc.LoadXml(input.Xml);
                    xmldoc.Save(inputStream);
                    xmldoc = null;
                    inputStream.Position = 0;
                    transformer.SetInputStream(inputStream, new Uri("file://"));

                    input.XsltParameters?.ToList().ForEach(x => transformer.SetParameter(new SaxonHE.QName(x.Name), new SaxonHE.XdmAtomicValue(x.Value)));

                    using (var stringWriter = new StringWriter())
                    {
                        var serializer = processor.NewSerializer();
                        serializer.SetOutputWriter(stringWriter);
                        transformer.Run(serializer);
                        var output = stringWriter.GetStringBuilder().ToString();
                        output = output.Replace("\n", Environment.NewLine);
                        return output;
                    }
                }
            }
        }

#endif

        /// <summary>
        /// Validate XML against XML Schema Definitions. See: https://github.com/FrendsPlatform/Frends.Xml
        /// </summary>
        /// <returns>Object { bool IsValid, string Error } </returns>
        public static ValidateResult Validate([PropertyTab]ValidationInput input, [PropertyTab]ValidationOptions options)
        {
            var s = input.Xml as string;
            if (s != null)
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(s);
                return ValidateXmlDocument(xmlDocument, input.XsdSchemas, options);
            }

            var document = input.Xml as XmlDocument;
            if (document != null)
            {
                return ValidateXmlDocument(document, input.XsdSchemas, options);
            }

            throw new InvalidDataException("The input data was not recognized as XML. Supported formats are XML string and XMLDocument.");
        }

        /// <summary>
        /// Convert JSON string to XML string. See: https://github.com/FrendsPlatform/Frends.Xml
        /// </summary>
        /// <returns>string</returns>
        public static string ConvertJsonToXml(JsonToXmlInput input)
        {
            return JsonConvert.DeserializeXmlNode(input.Json, input.XmlRootElementName).OuterXml;
        }

        private static ValidateResult ValidateXmlDocument(XmlDocument xmlDocument, IEnumerable<string> inputXsdSchemas, ValidationOptions options)
        {
            var validateResult = new ValidateResult() {IsValid = true};
            var schemas = new XmlSchemaSet();

            var settings = new XmlReaderSettings {ValidationType = ValidationType.Schema};
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            foreach (var schema in inputXsdSchemas)
            {
                schemas.Add(null, XmlReader.Create(new StringReader(schema), settings));
            }

            XDocument.Load(new XmlNodeReader(xmlDocument)).Validate(schemas, (o, e) =>
            {

                if (options.ThrowOnValidationErrors)
                {
                    throw new XmlSchemaValidationException(e.Message, e.Exception);
                }
                validateResult.IsValid = false;
                validateResult.Error = e.Message;
            });
            return validateResult;
        }


#if NET6_0_OR_GREATER
        private static SaxonCS.XPathSelector SetupXPathSelectorCS(QueryInput input, QueryOptions options)
        {
            var proc = Saxon.Activation.Activator.Activate();

            var builder = proc.NewDocumentBuilder();
            builder.SchemaValidationMode = SaxonCS.SchemaValidationMode.Lax;

            var xPathCompiler = proc.NewXPathCompiler();

            switch (options.XpathVersion)
            {
                case XPathVersion.V3:
                    xPathCompiler.XPathLanguageVersion = "3.0";
                    break;
                case XPathVersion.V2:
                    xPathCompiler.XPathLanguageVersion = "2.0";
                    break;
                case XPathVersion.V1:
                    xPathCompiler.XPathLanguageVersion = "1.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var xPathSelector = xPathCompiler.Compile(input.XpathQuery).Load();
            builder.BaseUri = DetectAndSetBaseUri(input.Xml);
            using (var reader = new StringReader(input.Xml))
            {
                var xdmNode = builder.Build(reader);
                xPathSelector.ContextItem = xdmNode;
                return xPathSelector;
            }            
        }
        

#else
        private static SaxonHE.XPathSelector SetupXPathSelectorHE(QueryInput input, QueryOptions options)
        {
            var proc = new SaxonHE.Processor();
            var builder = proc.NewDocumentBuilder();
            builder.SchemaValidationMode = SaxonHE.SchemaValidationMode.Lax;

            var xPathCompiler = proc.NewXPathCompiler();

            switch (options.XpathVersion)
            {
                case XPathVersion.V3:
                    xPathCompiler.XPathLanguageVersion = "3.0";
                    break;
                case XPathVersion.V2:
                    xPathCompiler.XPathLanguageVersion = "2.0";
                    break;
                case XPathVersion.V1:
                    xPathCompiler.XPathLanguageVersion = "1.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var xPathSelector = xPathCompiler.Compile(input.XpathQuery).Load();
            builder.BaseUri = new Uri("file://");
            using (var reader = new StringReader(input.Xml))
            {
                var xdmNode = builder.Build(reader);
                xPathSelector.ContextItem = xdmNode;
                return xPathSelector;
            }
        }
#endif

        private static Uri DetectAndSetBaseUri(string xml)
        {
            if (xml.Contains("<!DOCTYPE") || xml.Contains("schemaLocation") || xml.Contains("noNamespaceSchemaLocation"))
            {
                var currentDir = Directory.GetCurrentDirectory();
                return new Uri($"file:///{currentDir.Replace("\\", "/")}/");
            }

            return new Uri($"file:///{Directory.GetCurrentDirectory().Replace("\\", "/")}/"); ;
        }
    }
}
