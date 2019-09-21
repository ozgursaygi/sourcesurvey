using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Data;
using System.Collections;


namespace BaseClasses
{
    public class XmlHelper
    {
        public static XmlDocument transform_dataset(DataSet dataset, string xsltName)
        {
            try
            {
                MemoryStream input = new MemoryStream();

                dataset.WriteXml(input);
                input.Seek(0, SeekOrigin.Begin);
                XmlTextReader xmlReader = new XmlTextReader(input);

                return transform(xmlReader, xsltName);
            }
            catch (Exception ex)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.InnerXml = "error transforming xml" + ex.ToString();

                return xdoc;
            }

        }

        public static XmlDocument transform(XmlReader xmlReader, string xsltName)
        {
            try
            {
                MemoryStream output = new MemoryStream();

                XslCompiledTransform xsl = new XslCompiledTransform();
                xsl.Load(AppDomain.CurrentDomain.BaseDirectory + xsltName);
                XsltArgumentList xslArgs = new XsltArgumentList();
                CustomFormatters custFormatter = new CustomFormatters();
                xslArgs.AddExtensionObject("urn:custFormatter", custFormatter);
                xsl.Transform(xmlReader, xslArgs, output);
                output.Seek(0, SeekOrigin.Begin);
                XmlTextReader textReader = new XmlTextReader(output);
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(textReader);
                return xdoc;
            }
            catch (Exception ex)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.InnerXml = "error transforming xml" + ex.ToString();

                return xdoc;
            }
        }

    }
}
