﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace Fatura.UI.CustomHtmlHelper
{
    public static class CustomHTMLHelper
    {
        public static HtmlString RenderXml(string xml, string xsltPath)
        {
            XsltArgumentList args = new XsltArgumentList();
            // Create XslCompiledTransform object to loads and compile XSLT file.  
            XslCompiledTransform tranformObj = new XslCompiledTransform();
            tranformObj.Load(xsltPath);
            // Create XMLReaderSetting object to assign DtdProcessing, Validation type  
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.DtdProcessing = DtdProcessing.Parse;
            xmlSettings.ValidationType = ValidationType.DTD;
            // Create XMLReader object to Transform xml value with XSLT setting   
            using (XmlReader reader = XmlReader.Create(new StringReader(xml), xmlSettings))
            {
                StringWriter writer = new StringWriter();
                tranformObj.Transform(reader, args, writer);
                // Generate HTML string from StringWriter  
                HtmlString htmlString = new HtmlString(writer.ToString());
                return htmlString;
            }
        }
    }
}