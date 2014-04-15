using System;
using System.Collections.Generic;
using System.Xml;

namespace WT.Components.Common.Helpers
{
    public class XmlNodeHelper
    {
        private XmlDocument document = new XmlDocument();
        private XmlNode node = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        public XmlNodeHelper(string xml)
        {
            document.LoadXml(xml);
            node = document.SelectSingleNode("/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public XmlNodeHelper(XmlNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        public void LoadXml(string xml)
        {
            document.LoadXml(xml);
            node = document.SelectSingleNode("/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNode GetNode(string xpath)
        {
            return node.SelectSingleNode(xpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNodeList GetNodes(string xpath)
        {
            return node.SelectNodes(xpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNodeHelper GetHelperNode(string xpath)
        {
            XmlNode item = GetNode(xpath);
            if (item != null)
            {
                return new XmlNodeHelper(item);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public List<XmlNodeHelper> GetHelperNodes(string xpath)
        {
            XmlNodeList items = GetNodes(xpath);
            List<XmlNodeHelper> result = new List<XmlNodeHelper>();
            if (items != null)
            {
                foreach (XmlNode item in items)
                {
                    result.Add(new XmlNodeHelper(item));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (node == null)
            {
                return "";
            }

            return node.InnerText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetValueParseInt(out int result)
        {
            result = 0;

            string value = GetValue();
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return int.TryParse(value, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetValueParseInt()
        {
            string value = GetValue();
            if (string.IsNullOrEmpty(value))
            {
                return -1;
            }
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetValueParseLong(out long result)
        {
            result = 0;

            string value = GetValue();
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return long.TryParse(value, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetValueParseLong()
        {
            string value = GetValue();
            if (string.IsNullOrEmpty(value))
            {
                return -1;
            }

            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetValueParseDouble(out Double result)
        {
            result = -1;

            string value = GetValue();
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return Double.TryParse(value, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Double GetValueParseDouble()
        {
            string value = GetValue();
            if (string.IsNullOrEmpty(value))
            {
                return 0.0;
            }

            Double result;
            if (Double.TryParse(value, out result))
            {
                return result;
            }

            return 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetAttribute(string attributeName)
        {
            if (node == null)
            {
                return "";
            }

            if (node.Attributes != null && node.Attributes[attributeName] != null)
            {
                return node.Attributes[attributeName].Value;
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetAttributeParseInt(string attributeName, out int result)
        {
            result = 0;

            string value = GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return int.TryParse(value, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public int GetAttributeParseInt(string attributeName)
        {
            string value = GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetAttributeParseLong(string attributeName, out long result)
        {
            result = 0;

            string value = GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return long.TryParse(value, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public long GetAttributeParseLong(string attributeName)
        {
            string value = GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool GetAttributeParseDouble(string attributeName, out Double result)
        {
            result = 0;

            string value = GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return Double.TryParse(value, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public Double GetAttributeParseDouble(string attributeName)
        {
            string value = GetAttribute(attributeName);
            if (string.IsNullOrEmpty(value))
            {
                return 0.0;
            }

            Double result;
            if (Double.TryParse(value, out result))
            {
                return result;
            }

            return 0.0;
        }
    }
}
