using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace Frends.Xml.Tests
{
    [TestFixture]
    public class Tests
    {
        private const string Xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?> \r\n<bookstore>\r\n    <book genre=\"autobiography\" publicationdate=\"1981-03-22\" ISBN=\"1-861003-11-0\">\r\n        <title>The Autobiography of Benjamin Franklin</title>\r\n        <author>\r\n            <first-name>Benjamin</first-name>\r\n            <last-name>Franklin</last-name>\r\n        </author>\r\n        <price>8.99</price>\r\n    </book>\r\n    <book genre=\"novel\" publicationdate=\"1967-11-17\" ISBN=\"0-201-63361-2\">\r\n        <title>The Confidence Man</title>\r\n        <author>\r\n            <first-name>Herman</first-name>\r\n            <last-name>Melville</last-name>\r\n        </author>\r\n        <price>11.99</price>\r\n    </book>\r\n    <book genre=\"philosophy\" publicationdate=\"1991-02-15\" ISBN=\"1-861001-57-6\">\r\n        <title>The Gorgias</title>\r\n        <author>\r\n            <name>Plato</name>\r\n        </author>\r\n        <price>9.99</price>\r\n    </book>\r\n</bookstore>\r\n";

        private const string SimpleXsd = @"<xsd:schema xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                                targetNamespace=""urn:books""
                                                xmlns:bks=""urn:books"">

                                      <xsd:element name=""books"" type=""bks:BooksForm""/>

                                      <xsd:complexType name=""BooksForm"">
                                        <xsd:sequence>
                                          <xsd:element name=""book"" 
                                                      type=""bks:BookForm"" 
                                                      minOccurs=""0"" 
                                                      maxOccurs=""unbounded""/>
                                          </xsd:sequence>
                                      </xsd:complexType>

                                      <xsd:complexType name=""BookForm"">
                                        <xsd:sequence>
                                          <xsd:element name=""author""   type=""xsd:string""/>
                                          <xsd:element name=""title""    type=""xsd:string""/>
                                          <xsd:element name=""genre""    type=""xsd:string""/>
                                          <xsd:element name=""price""    type=""xsd:float"" />
                                          <xsd:element name=""pub_date"" type=""xsd:date"" />
                                          <xsd:element name=""review""   type=""xsd:string""/>
                                        </xsd:sequence>
                                        <xsd:attribute name=""id""   type=""xsd:string""/>
                                      </xsd:complexType>
                                    </xsd:schema>";

        [Test]
        public void TestXPathQuerySingleNoResultsFoundDontThrow()
        {
            const string xPath = "/bookstore/book/price/things";
            var res = Frends.Xml.Xml.XpathQuerySingle(new QueryInput() { Xml = Xml, XpathQuery = xPath }, new QueryOptions() {ThrowErrorOnEmptyResults = false, XpathVersion = XPathVersion.V2});

            Assert.That(res.Data, Is.Null);
            Assert.That(res.ToJson(), Is.EqualTo(null));
        }

        [Test]
        public void TestXPathQueryNoResultsFoundDontThrow()
        {
            const string xPath = "/bookstore/book/price/things";
            var res = Frends.Xml.Xml.XpathQuery(new QueryInput() { Xml = Xml, XpathQuery = xPath }, new QueryOptions() { ThrowErrorOnEmptyResults = false });

            Assert.That(res.Data.Count, Is.EqualTo(0));
            Assert.That(res.ToJson().Count, Is.EqualTo(0));
        }

        [Test]
        public void TestXPathQuerySingleResultsThatIsAtomic()
        {
            const string xPath = "/bookstore/book[1]/price/string()";
            var res = Frends.Xml.Xml.XpathQuerySingle(new QueryInput() {Xml = Xml, XpathQuery = xPath }, new QueryOptions());
            
            Assert.That(res.Data, Is.EqualTo("8.99"));
            var jTokenRes = res.ToJson() as JToken;
            Assert.That(jTokenRes.Value<string>(), Is.EqualTo("8.99"));
        }

        [Test]
        public void TestXPathQuerySingleResultsThatIsOfTypeXml()
        {
            const string xPath = "/bookstore/book[1]/price";
            var res = Frends.Xml.Xml.XpathQuerySingle(new QueryInput() { Xml = Xml, XpathQuery = xPath }, new QueryOptions());

            Assert.That(res.Data, Is.EqualTo("<price>8.99</price>"));
            var jTokenRes = res.ToJson() as JToken;
            Assert.That(jTokenRes["price"].Value<string>(), Is.EqualTo("8.99"));
        }

        [Test]
        public void TestXPathQueryResultsThatIsAtomic()
        {
            const string xPath = "/bookstore/book/price/string()";
            var res = Frends.Xml.Xml.XpathQuery(new QueryInput() { Xml = Xml, XpathQuery = xPath }, new QueryOptions());

            Assert.That(res.Data[0], Is.EqualTo("8.99"));
            var jTokenRes = res.ToJson();
            Assert.That(jTokenRes[1].ToString(), Is.EqualTo("11.99"));
            Assert.That(jTokenRes[2].ToString(), Is.EqualTo("9.99"));
        }

        [Test]
        public void TestXPathQueryResultsThatIsAtomicAndCastToDouble()
        {
            const string xPath = "xs:double(/bookstore/book[1]/price/text())";
          
            var res = Frends.Xml.Xml.XpathQuery(new QueryInput() { Xml = Xml, XpathQuery = xPath }, new QueryOptions());
            var jTokenRes = res.ToJson();
            Assert.That(res.Data[0], Is.EqualTo(8.99d));
            Assert.That(((JToken)jTokenRes[0]).Value<double>(), Is.EqualTo(8.99d));
        }

        [Test]
        public void TestXPathQueryResultsThatIsOfTypeXml()
        {
           const string xPath = "/bookstore/book/price";
            var res = Frends.Xml.Xml.XpathQuery(new QueryInput() { Xml = Xml, XpathQuery = xPath }, new QueryOptions());
            var jTokenRes = res.ToJson();        
            Assert.That(res.Data[0], Is.EqualTo("<price>8.99</price>"));
            Assert.That(((JToken)jTokenRes[1])["price"].Value<string>(), Is.EqualTo("11.99"));
            Assert.That(((JToken)jTokenRes[2])["price"].Value<string>(), Is.EqualTo("9.99"));
        }

        //TODO: Add test for XsltTransformParameters
        [Test]
        public void TestXsltTransform()
        {
            const string transformXml = @"<?xml version=""1.0""?>
                                <hello-world>   <greeter>An XSLT Programmer</greeter>   <greeting>Hello, World!</greeting></hello-world>";

            const string xslt = @"<?xml version=""1.0""?>
                                <xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" version=""1.0"">
                                  <xsl:template match=""/hello-world"">
                                       <xsl:value-of select=""greeting""/> <xsl:apply-templates select=""greeter""/>
                                  </xsl:template>
                                  <xsl:template match=""greeter"">
                                    <DIV>from <I><xsl:value-of select="".""/></I></DIV>
                                  </xsl:template>
                                </xsl:stylesheet>";

            var res = Frends.Xml.Xml.Transform(new TransformInput() { Xml = transformXml, Xslt = xslt });
            Assert.That(res, Is.EqualTo("<?xml version=\"1.0\" encoding=\"UTF-8\"?>Hello, World!<DIV>from <I>An XSLT Programmer</I></DIV>"));
        }


        [Test]
        public void TestXsltTransformWithNonUTF8Declaration()
        {
            const string transformXml = @"<?xml version=""1.0"" encoding=""Windows-1252"" ?><in>Ä</in>";

            const string xslt = @"<?xml version=""1.0""?>
                                <xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" version=""2.0"">
                                <xsl:output method=""xml"" />
                                  <xsl:template match=""/in"">
                                       <out><xsl:value-of select="".""/></out>
                                  </xsl:template>
                                </xsl:stylesheet>";

            var res = Frends.Xml.Xml.Transform(new TransformInput() { Xml = transformXml, Xslt = xslt });
            Assert.That(res, Is.EqualTo("<?xml version=\"1.0\" encoding=\"UTF-8\"?><out>Ä</out>"));
        }

        [Test]
        public void TestValidationFailWithMessage()
        {
            const string simpleXml = @"<?xml version=""1.0""?>
                                    <x:books xmlns:x=""urn:books"">
                                       <book id=""bk001"">
                                          <author>Writer</author>
                                          <title>The First Book</title>
                                          <genre>Fiction</genre>
                                          <price>44.95</price>
                                          <pub_date>2000-10-01</pub_date>
                                          <review>An amazing story of nothing.</review>
                                       </book>

                                       <book id=""bk002"">
                                          <author>Poet</author>
                                          <title>The Poet's First Poem</title>
                                          <genre>Poem</genre>
                                          <price>24.95</price>
                                          <review>Least poetic poems.</review>
                                       </book>
                                    </x:books>";


            var result = Frends.Xml.Xml.Validate(new ValidationInput() {Xml = simpleXml, XsdSchemas = new []{ SimpleXsd } }, new ValidationOptions());
            Assert.That(result.IsValid,Is.False);
            Assert.That(result.Error, Does.Contain("The element 'book' has invalid child element 'review'."));
        }

        [Test]
        public void TestValidationSucceeds()
        {
            const string simpleXml = @"<?xml version=""1.0""?>
                                    <x:books xmlns:x=""urn:books"">
                                       <book id=""bk001"">
                                          <author>Writer</author>
                                          <title>The First Book</title>
                                          <genre>Fiction</genre>
                                          <price>44.95</price>
                                          <pub_date>2000-10-01</pub_date>
                                          <review>An amazing story of nothing.</review>
                                       </book>

                                       <book id=""bk002"">
                                          <author>Poet</author>
                                          <title>The Poet's First Poem</title>
                                          <genre>Poem</genre>
                                          <price>24.95</price>
                                          <pub_date>2000-10-01</pub_date>
                                          <review>Least poetic poems.</review>
                                       </book>
                                    </x:books>";


            var result = Frends.Xml.Xml.Validate(new ValidationInput() { Xml = simpleXml, XsdSchemas = new[] { SimpleXsd } }, new ValidationOptions());
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ConvertJsonToXmlString()
        {
            const string json = @"{
              '?xml': {
               '@version': '1.0',
               '@standalone': 'no'
               },
               'root': {
                 'person': [
                   {
                     '@id': '1',
                    'name': 'Alan',
                    'url': 'http://www.google.com'
                  },
                  {
                    '@id': '2',
                    'name': 'Louis',
                    'url': 'http://www.yahoo.com'
                  }
                ]
              }
            }";
          var result = Frends.Xml.Xml.ConvertJsonToXml(new JsonToXmlInput() {Json = json});
          Assert.That(result, Does.Contain("<url>http://www.google.com</url>"));
        }
    }
}
