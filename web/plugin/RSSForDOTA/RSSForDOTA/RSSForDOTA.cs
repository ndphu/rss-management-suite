using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlAgilityPack;

namespace RSSForDOTA
{
    public class RSSForDOTA : RSSMSPluginInterface.IRSSPlugin
    {
        private static string baseLinkStr = @"http://garena.vn";
        private static string websiteLinkStr = @"http://garena.vn/dota";
        private static string pluginNameStr = "RSS for DOTA-News";
        private static string desciptionStr = "Tin tức ESport - Tin tức DOTA - Garena.vn";
        private static string rssNameStr = "Tin tức DOTA - Garena";

        public string GetPluginName()
        {
            return pluginNameStr;
        }

        public string GetRSSDescription()
        {
            return desciptionStr;
        }

        public string GetRSSName()
        {
            return rssNameStr;
        }

        public string GetRSSWebsiteLink()
        {
            return websiteLinkStr;
        }

        public string GetRSSResult(int count)
        {
            XmlDocument docXML = new XmlDocument();
            try
            {

                HtmlAgilityPack.HtmlWeb hw = new HtmlAgilityPack.HtmlWeb();
                HtmlDocument docHTML = hw.Load(websiteLinkStr);
                HtmlNodeCollection listOfTable = docHTML.DocumentNode.SelectNodes("//div[@class='news01']");

                CreateDOTARSS(docXML, listOfTable, count);
            }
            catch
            {
                return CreateXmlErrorMessage("Eror!", "Server not found");
            }
            return FormatHTML(docXML.OuterXml);
        }

        private string FormatHTML(string result)
        {
            result = result.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");
            result = result.Replace("&amp;", "&");
            return result;
        }

        private void CreateDOTARSS(XmlDocument doc, HtmlNodeCollection listOfNode, int count)
        {
            CreateBasicRssDoc(doc, rssNameStr, desciptionStr, websiteLinkStr);
            foreach (HtmlNode node in listOfNode)
            {
                CreateDOTANodeInfo(doc, node);
            }

            XmlNode channelNode = doc.SelectSingleNode("//channel");
            XmlNodeList listOfItemnNode = doc.SelectNodes(".//item");
            int nItemNode = listOfItemnNode.Count;

            for (int i = count; i < nItemNode; i++)
            {
                if (i < 0)
                    continue;
                channelNode.RemoveChild(listOfItemnNode[i]);
            }
        }

        private void CreateBasicRssDoc(XmlDocument doc, string title, string description, string link)
        {
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(declaration, doc.DocumentElement);

            XmlElement rssNode = doc.CreateElement("rss");
            XmlAttribute rssAtt = doc.CreateAttribute("version");
            rssAtt.Value = "2.0";
            rssNode.Attributes.Append(rssAtt);

            XmlElement channelNode = doc.CreateElement("channel");

            XmlElement titleNode = doc.CreateElement("title");
            titleNode.InnerText = title;
            channelNode.AppendChild(titleNode);

            XmlElement desNode = doc.CreateElement("description");
            desNode.InnerText = description;
            channelNode.AppendChild(desNode);

            XmlElement linkNode = doc.CreateElement("link");
            linkNode.InnerText = link;
            channelNode.AppendChild(linkNode);

            rssNode.AppendChild(channelNode);
            doc.AppendChild(rssNode);
        }

        private void CreateDOTANodeInfo(XmlDocument doc, HtmlNode node)
        {
            XmlNode newItem = doc.CreateElement("item");

            HtmlNode imgNode = node.SelectSingleNode("./img");
            imgNode.Attributes[2].Value = baseLinkStr + imgNode.Attributes[2].Value;
            HtmlNode h3Node = node.SelectSingleNode("./h3");
            HtmlNode tableNode = node.SelectSingleNode("./table");

            XmlNode titleNode = doc.CreateElement("title");
            titleNode.InnerText = imgNode.Attributes[3].Value;
            newItem.AppendChild(titleNode);

            XmlNode descriptionNode = doc.CreateElement("description");
            string desTemp = tableNode.SelectSingleNode("./tr[3]/td[1]").InnerText;
            descriptionNode.InnerText = "<![CDATA[" + imgNode.OuterHtml + desTemp + "]]>";
            newItem.AppendChild(descriptionNode);

            XmlNode linkNode = doc.CreateElement("link");
            linkNode.InnerText = baseLinkStr + h3Node.SelectSingleNode("./a").Attributes[0].Value;
            newItem.AppendChild(linkNode);

            XmlNode pubDateNode = doc.CreateElement("pubDate");
            string dateStr = tableNode.SelectSingleNode("./tr[2]/td[1]").InnerText.Trim();
            pubDateNode.InnerText = dateStr;
            newItem.AppendChild(pubDateNode);

            doc.DocumentElement.FirstChild.AppendChild(newItem);
        }

        private string CreateXmlErrorMessage(string error, string message)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(declaration, doc.DocumentElement);

            XmlElement rssNode = doc.CreateElement("rss");
            XmlAttribute rssAtt = doc.CreateAttribute("version");
            rssAtt.Value = "2.0";
            rssNode.Attributes.Append(rssAtt);

            XmlElement channelNode = doc.CreateElement("channel");

            XmlElement titleNode = doc.CreateElement("title");
            titleNode.InnerText = error;
            channelNode.AppendChild(titleNode);

            XmlElement desNode = doc.CreateElement("description");
            desNode.InnerText = message;
            channelNode.AppendChild(desNode);

            XmlElement linkNode = doc.CreateElement("link");
            linkNode.InnerText = "";
            channelNode.AppendChild(linkNode);

            XmlElement itemNode = doc.CreateElement("item");

            XmlElement titleNode_item = doc.CreateElement("title");
            titleNode_item.InnerText = error;
            itemNode.AppendChild(titleNode_item);

            XmlElement desNode_item = doc.CreateElement("description");
            desNode_item.InnerText = message;
            itemNode.AppendChild(desNode_item);

            XmlElement linkNode_item = doc.CreateElement("link");
            linkNode_item.InnerText = "";
            itemNode.AppendChild(linkNode_item);

            XmlElement pubDateNode = doc.CreateElement("pubDate");
            pubDateNode.InnerText = DateTime.Now.ToString();
            itemNode.AppendChild(pubDateNode);

            channelNode.AppendChild(itemNode);
            rssNode.AppendChild(channelNode);
            doc.AppendChild(rssNode);
            return doc.OuterXml;
        }
    }
}
