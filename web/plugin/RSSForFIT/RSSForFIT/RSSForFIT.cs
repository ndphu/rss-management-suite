using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Xml;

namespace RSSForFIT
{
    public class RSSForFIT : RSSMSPluginInterface.IRSSPlugin
    {
        private static string websiteLinkStr = @"http://www.fit.hcmus.edu.vn/vn/";
        private static string pluginNameStr = "RSS for F.I.T - ĐH.KHTN";
        private static string desciptionStr = "Diễn đàn tin tức - khoa Công nghệ thông tin - Đại học KHOA HỌC TỰ NHIÊN";
        private static string rssNameStr = "Tin tức - Khoa CNTT - ĐH KHTN";

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
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument docHTML = hw.Load(websiteLinkStr);
                HtmlNodeCollection listOfTable = docHTML.DocumentNode.SelectNodes("//table[@id='dnn_ctr989_ModuleContent']/tr/td/div/table");

                CreateFITRSS(docXML, listOfTable, count);
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

        private void CreateFITRSS(XmlDocument doc, HtmlNodeCollection listOfNode, int count)
        {
            CreateBasicRssDoc(doc, rssNameStr, desciptionStr, websiteLinkStr);
            foreach (HtmlNode node in listOfNode)
            {
                CreateFITNodeInfo(doc, node);
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

        private void CreateFITNodeInfo(XmlDocument doc, HtmlNode node)
        {
            XmlNode newItem = doc.CreateElement("item");

            XmlNode titleNode = doc.CreateElement("title");
            HtmlNode titleNodeOfHTML = node.SelectSingleNode("./tr/td[@class='post_title']");

            for (int i = 0; i < titleNodeOfHTML.ChildNodes.Count; i++)
            {
                switch (titleNodeOfHTML.ChildNodes[i].Name)
                {
                    case "a":
                        {
                            titleNodeOfHTML.ChildNodes[i].Attributes[0].Value = websiteLinkStr + titleNodeOfHTML.ChildNodes[i].Attributes[0].Value;
                            break;
                        }
                    case "img":
                        {
                            titleNodeOfHTML.ChildNodes[i].Attributes[1].Value = websiteLinkStr + titleNodeOfHTML.ChildNodes[i].Attributes[1].Value;
                            break;
                        }
                }
            }
            titleNode.InnerText = titleNodeOfHTML.InnerHtml;
            newItem.AppendChild(titleNode);


            XmlNode descriptionNode = doc.CreateElement("description");
            descriptionNode.InnerText = "";
            newItem.AppendChild(descriptionNode);

            XmlNode linkNode = doc.CreateElement("link");
            linkNode.InnerText =  node.SelectSingleNode("./tr/td/a").Attributes[0].Value.ToString();
            newItem.AppendChild(linkNode);

            XmlNode pubDateNode = doc.CreateElement("pubDate");
            int day = int.Parse(node.SelectSingleNode(".//tr[1]/td[@class='day_month']").InnerText.Trim());
            int month = int.Parse(node.SelectSingleNode(".//tr[2]/td[@class='day_month']").InnerText.Trim());
            int year = int.Parse(node.SelectSingleNode(".//td[@class='post_year']").InnerText.Trim());
            DateTime dtDateTime = new DateTime(year, month, day);
            pubDateNode.InnerText = dtDateTime.ToString("dd/MM/yyyy");
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