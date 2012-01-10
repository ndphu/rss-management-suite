using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace RSSForFIT
{
    public class RSSForFIT : RSSMSPluginInterface.IRSSPlugin
    {
        private static string websiteLinkStr = @"http://www.fit.hcmus.edu.vn/vn/";
        private static string pluginNameStr = "RSS for F.I.T - ĐH.KHTN";
        private static string desciptionStr = "Những tin tức nóng hổi nhất từ Trang chủ của Website môn học Khoa Công Nghệ Thông Tin - Trường Đại Học Khoa Học Tự Nhiên - Đại học Quốc gia TP Hồ Chính Minh";
        private static string rssNameStr = "Khoa Công Nghệ Thông Tin _ Trường Đại Học Khoa Học Tự Nhiên";

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
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(websiteLinkStr);
            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
            string line = null;
            StringBuilder sb = new StringBuilder();
            bool b1 = false;
            int check = 0;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Replace("&nbsp;", "").Replace("nowrap", "").Replace("&reg", "").Replace("&copy", "").Replace("&", "&amp;");
                if (line.Contains("-->"))
                    line = line.Substring(line.IndexOf("-->") + 3);
                if (line.Contains("<div id=\"dnn_ctr989_ModuleContent\">"))
                {
                    b1 = true;
                }

                if (b1 == true)
                {

                    if (line.Contains("<div"))
                    {
                        ++check;
                    }
                    else if (line.Contains("</div>"))
                    {
                        --check;
                        if (check == 0)
                        {
                            sb.Append(line.Substring(0, "</div>".Length));
                            sb.Append("\r\n");
                            break;
                        }
                    }
                    sb.Append(line);
                    sb.Append("\r\n");
                }
            }
            try
            {
                XDocument result = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("rss",
                        new XAttribute("version", "2.0"),
                        new XElement("channel",
                            new XElement("title", rssNameStr),
                            new XElement("description", desciptionStr),
                            new XElement("link", websiteLinkStr)
                            )
                        )
                     );

                XDocument xdoc = XDocument.Parse(sb.ToString());
                var infos = xdoc.Element("div").Element("div").Elements("table").Elements("tr").Elements("td").Where(td => td.Attribute("class") != null && td.Attribute("class").Value.CompareTo("post_title") == 0).Select(_element => _element.Element("a"));
                XElement linkElement = result.Element("rss").Element("channel").Element("link");
                DateTime dateTime = DateTime.Now;
                foreach (XElement element in infos)
                {
                    XElement itemElement = new XElement("item",
                        new XElement("title", element.Attribute("title").Value),
                        new XElement("description" , element.Value),
                        new XElement("link", websiteLinkStr + element.Attribute("href").Value),
                        new XElement("pubDate", dateTime.ToLongDateString() + " " + dateTime.ToLongTimeString()));
                    linkElement.AddAfterSelf(itemElement);
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                return CreateXmlErrorMessage("Error", "Website " + websiteLinkStr + " is tempory unavailable.");
            }
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