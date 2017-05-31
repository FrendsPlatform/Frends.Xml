using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saxon.Api;

#pragma warning disable 1591

namespace Frends.Xml
{
    public class QueryInput
    {
        /// <summary>
        /// XML to be queried
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// The XPath Query
        /// </summary>
        public string XpathQuery { get; set; }
    }
    public enum XPathVersion { V3, V2, V1 }

    public class QueryOptions
    {

        /// <summary>
        /// Throw an exception if no results returned by query
        /// </summary>
        [DefaultValue("true")]
        public bool ThrowErrorOnEmptyResults { get; set; } = true;

        /// <summary>
        /// XPath Query language version
        /// </summary>
        public XPathVersion XpathVersion { get; set; }
    }

    public class QueryResults
    {
        private readonly List<XdmItem> _xdmItems;
        private readonly Lazy<List<object>> _data;
        private readonly Lazy<List<object>> _jTokens;
        public QueryResults(IEnumerable<XdmItem> data)
        {
            _xdmItems = data.ToList();
            _data = new Lazy<List<object>>(() => _xdmItems.Select(Extensions.GetXmlOrAtomicObject).ToList());
            _jTokens = new Lazy<List<object>>(() =>  _xdmItems.Select(Extensions.GetJTokenFromXdmItem).ToList());
        }

        public List<object> Data => _data.Value;

        public List<object> ToJson()
        {
            return _jTokens.Value;
        }

        public object ToJson(int index)
        {
            return _jTokens.IsValueCreated ? _jTokens.Value[index] : Extensions.GetJTokenFromXdmItem(_xdmItems[index]);
        }
    }

    public class QuerySingleResults
    {
        private readonly XdmItem _xdmItem;
        private readonly Lazy<object> _jToken;
        private readonly Lazy<object> _data;

        public QuerySingleResults(XdmItem item)
        {
            _xdmItem = item;
            _data = new Lazy<object>(() =>  _xdmItem != null ? Extensions.GetXmlOrAtomicObject(_xdmItem) : null);
            _jToken = new Lazy<object>(() => _xdmItem != null ? Extensions.GetJTokenFromXdmItem(_xdmItem): null);
        }

        public object Data => _data.Value;

        public object ToJson()
        {
            return _jToken.Value;
        }

        public override string ToString()
        {
            return _data.Value.ToString();
        }
    }

    public static class Extensions
    {
        public static object GetJTokenFromXdmItem(XdmItem xdmItem)
        {
            var xdmAtomicValue = xdmItem as XdmAtomicValue;
            if (xdmAtomicValue != null)
            {
                return JToken.FromObject(xdmAtomicValue.Value);
            }

            var output = new XmlDocument();
            output.LoadXml(xdmItem.ToString());
            return JToken.FromObject(output);
        }

        public static object GetXmlOrAtomicObject(XdmItem item)
        {
            var xdmAtomicValue = item as XdmAtomicValue;
            return xdmAtomicValue != null ? xdmAtomicValue.Value : item.ToString();
        }

    }

    public class TransformInput
    {
        /// <summary>
        /// Source to transform
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Xslt transformation
        /// </summary>
        public string Xslt { get; set; }

        /// <summary>
        /// Xslt parameters
        /// </summary>
        public XsltParameters[] XsltParameters { get; set; }
    }

    public class XsltParameters
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ValidationInput
    {
        /// <summary>
        /// Input must be of type string or XmlDocument
        /// </summary>
        public dynamic Xml { get; set; }

        /// <summary>
        /// List of XML Schema Definitions
        /// </summary>
        public string[] XsdSchemas { get; set; }
    }

    public class ValidationOptions
    {
        /// <summary>
        /// Throw exception on validation error.
        /// </summary>
        public bool ThrowOnValidationErrors { get; set; }
    }

    public class ValidateResult
    {
        public bool IsValid { get; set; }
        public string Error { get; set; }
    }
}
