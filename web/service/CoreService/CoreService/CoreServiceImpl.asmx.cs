using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Security.Permissions;
using System.Net;
using System.IO;
using System.Xml;
using System.Reflection;
using RSSMSPluginInterface;
using System.Globalization;

namespace CoreService
{
    /// <summary>
    /// Summary description for CoreServiceImpl
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CoreServiceImpl : System.Web.Services.WebService, ICoreService
    {
        private static Random RANDOM = new Random();
        
        #region Phu

        [WebMethod]
        public bool CheckValidUsername(string username)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                var user = dt.Accounts.Where(account => account.Username.CompareTo(username) == 0);
                if (user.Count<Account>() == 0)
                    result = true;
                else
                    result = false;
            }
            catch 
            {
                result = false;
            }
            finally
            {

            }
            return result;
        }

        [WebMethod]
        public bool Register(string username, string password)
        {
            if (!CheckValidUsername(username))
                return false;
            bool result = false;
            
            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                int randomvalue = RANDOM.Next(0, int.MaxValue);
                string md5pass = ServiceUtility.CalculateMD5Hash(password + randomvalue.ToString());

                Account account = new Account()
                {
                    Username = username,
                    Password = md5pass,
                    Salt = randomvalue,
                };

                dt.Accounts.InsertOnSubmit(account);
                dt.SubmitChanges();

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
            }

            return result;
        }

        [WebMethod]
        public bool Login(string username, string password)
        {
            if (Membership.ValidateUser(username, password))
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                string UserID = dt.Accounts.Single(account => account.Username.CompareTo(username) == 0).ID.ToString();
                FormsAuthentication.SetAuthCookie(UserID, true);
                return true;
            }
            return false;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public bool Logout()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return true;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        // 0 - success.
        // 1 - name exist
        // 2 - exception
        public int AddTab(string tabName)
        {
            int result = -1;
            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                var tabs = dt.Tabs.Where(tab => tab.Name.CompareTo(tabName) == 0 && tab.UserID == GetCurrentUserID());
                if (tabs.Count<Tab>() == 0)
                {
                    int UserID = GetCurrentUserID();

                    Tab newTab = new Tab()
                    {
                        Name = tabName,
                        UserID = UserID,
                    };

                    dt.Tabs.InsertOnSubmit(newTab);
                    dt.SubmitChanges();

                    result = 0;
                }
                else
                {
                    result = 1;
                }
            }
            catch
            {
                result = 2;
            }
            finally
            {
            }

            return result;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        // 0 - success
        // 1 - different owner 
        // 2 - tab not exist
        // 3 - exception
        public int RemoveTab(int tabid)
        {
            int result = -1;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                
                var tabsToDelete = dt.Tabs.Where(_tab => _tab.ID == tabid);
                if (tabsToDelete.Count<Tab>() == 0)
                {
                    result = 2;
                }
                else
                {
                    var tabToDelete = tabsToDelete.Single();
                    if (tabToDelete.UserID != GetCurrentUserID())
                    {
                        var shares = dt.Shares.Where(share => share.TabID == tabToDelete.ID && share.AccountID == GetCurrentUserID());
                        dt.Shares.DeleteAllOnSubmit(shares);
                        dt.SubmitChanges();
                        result = 0;
                    }
                    else
                    {
                        var shares = dt.Shares.Where(share => share.TabID == tabToDelete.ID);

                        dt.Shares.DeleteAllOnSubmit(shares);

                        dt.RSSItems.DeleteAllOnSubmit(tabToDelete.RSSItems);

                        dt.Tabs.DeleteOnSubmit(tabToDelete);
                        dt.SubmitChanges();
                        result = 0;
                    }
                }
                
            }
            catch
            {
                result = 3;
            }
            finally
            {
            }

            return result;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        // 0 - success
        // 1 - different owner 
        // 2 - tab not exist
        // 3 - username not exist
        // 4 - can't share yourselt
        // 5 - exception
        public int ShareTab(int tabid, string userName)
        {
            int result = -1;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabsToShare = dt.Tabs.Where(_tab => _tab.ID == tabid);

                if (tabsToShare.Count<Tab>() == 0)
                    result = 2;
                else
                {
                    var tabToShare = tabsToShare.Single();
                    if (tabToShare.UserID != GetCurrentUserID())
                        result = 1;
                    else
                    {
                        Share share = new Share();
                        share.TabID = tabToShare.ID;

                        var UsersToShare = dt.Accounts.Where(account => account.Username.CompareTo(userName) == 0);
                        if (UsersToShare.Count<Account>() == 0)
                        {
                            result = 3;
                        }
                        else
                        {
                            share.AccountID = UsersToShare.Single().ID;
                            if (share.AccountID == GetCurrentUserID())
                                result = 4;
                            else
                            {
                                var shares = dt.Shares.Where(_share => _share.TabID == tabid && (_share.AccountID == GetCurrentUserID() || _share.AccountID == share.AccountID));
                                if (shares.Count<Share>() == 0)
                                {
                                    dt.Shares.InsertOnSubmit(share);
                                    dt.SubmitChanges();
                                }

                                result = 0;
                            }
                        }

                       
                    }
                }
                
            }
            catch
            {
                result = 3;
            }
            finally
            {
            }

            return result;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        // 0 - success
        // 1 - different owner 
        // 2 - tab not exist
        // 3 - duplicate name
        // 4 - exception
        public int RenameTab(int tabid, string newName)
        {
            int result = -1;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabsToRename = dt.Tabs.Where(_tab => _tab.ID == tabid );
                if (tabsToRename.Count<Tab>() == 0)
                    result = 2;
                else
                {
                    var tabToRename = tabsToRename.Single();
                    if (tabToRename.UserID != GetCurrentUserID())
                    {
                        result = 1;
                    }
                    else
                    {
                        var sameName = dt.Tabs.Where(tab => tab.UserID == GetCurrentUserID() && tab.Name.CompareTo(newName) == 0);
                        if (sameName.Count<Tab>() != 0)
                            result = 3;
                        else
                        {
                            tabToRename.Name = newName;
                            dt.SubmitChanges();

                            result = 0;
                        }
                    }
                }
                
            }
            catch
            {
                result = 4;
            }
            finally
            {
            }

            return result;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public TabDTO[] GetAllTabs()
        {
            List<TabDTO> result = new List<TabDTO> ();
            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                int currentUserID = GetCurrentUserID();

                var tabs = dt.Tabs.Where(tab => tab.UserID == currentUserID);

                List<Tab> list = tabs.ToList();
                foreach (Tab tab in list)
                {
                    TabDTO tabdto = new TabDTO();
                    tabdto.Id = tab.ID;
                    tabdto.Name = tab.Name;
                    tabdto.OwnerID = tab.UserID;
                    tabdto.OwnerUsername = tab.Account.Username;
                    tabdto.ChildCount = tab.RSSItems.Count;
                    result.Add(tabdto);
                }
            }
            catch
            {
                
            }
            finally
            {
            }
            return result.ToArray();
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public TabDTO[] GetAllSharedTabs()
        {
            List<TabDTO> result = new List<TabDTO>();
            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                int currentUserID = GetCurrentUserID();

                var tabs = dt.Shares.Where(share => share.AccountID == currentUserID).Select(_share => _share.Tab);
                
                List<Tab> list = tabs.ToList();
                foreach (Tab tab in list)
                {
                    TabDTO tabdto = new TabDTO();
                    tabdto.Id = tab.ID;
                    tabdto.Name = tab.Name;
                    tabdto.OwnerID = tab.UserID;
                    tabdto.OwnerUsername = tab.Account.Username;
                    tabdto.ChildCount = tab.RSSItems.Count;
                    result.Add(tabdto);
                }
            }
            catch
            {

            }
            finally
            {
            }
            return result.ToArray();
        }
        #endregion Phu

        #region Deo
        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        // 0 - successful
        // 1 - existed
        // 2 - incorrect link
        // 3 - Different onwer
        // 4 - failed
        public int AddRSSItem(int tabid, string rsslink)
        {
            int result = 0;
            try
            {
                if (IsExist(rsslink, tabid))
                {
                    result = 1;
                    return result;
                }

                string nameStr = "";
                string descriptionStr = "";

                if (!IsValid(rsslink, ref nameStr, ref descriptionStr))
                {
                    result = 2;
                    return result;
                }
                
                RSSDBDataContext data = new RSSDBDataContext();
                Tab tabToInsert = (from tab in data.Tabs
                                   where tab.ID == tabid
                                   select tab).Single();

                if (tabToInsert.UserID != GetCurrentUserID())
                    return 3;

                RSSItem newRssItem = new RSSItem();
                newRssItem.Name = nameStr;
                newRssItem.Description = descriptionStr;
                newRssItem.RSSLink = rsslink;
                newRssItem.TabID = tabid;
                newRssItem.PluginID = null;

                tabToInsert.RSSItems.Add(newRssItem);

                data.SubmitChanges();
            }
            catch
            {
                result = 4;
            }
            finally
            {
                
            }
            return result;
        }

        private bool IsExist(string newRssLink, int tabid)
        {
            RSSDBDataContext data = new RSSDBDataContext();
            List<RSSItem> listOfItem = (from rssitem in data.RSSItems
                                        where rssitem.RSSLink == newRssLink && rssitem.Tab.UserID == GetCurrentUserID() && rssitem.TabID == tabid
                                        select rssitem).ToList();
            if (listOfItem.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool IsValid(string newRssLink, ref string nameStr, ref string descriptionStr)
        {
            bool result = true;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newRssLink);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader.ReadToEnd());

                XmlNode rssNode = doc.DocumentElement.SelectSingleNode("//rss");
                XmlNode channelNode = rssNode.SelectSingleNode("./channel");

                XmlNode titleNode = channelNode.SelectSingleNode("./title");
                XmlNode linkNode = channelNode.SelectSingleNode("./link");
                XmlNode descriptionNode = channelNode.SelectSingleNode("./description");

                XmlNodeList listOfItem = channelNode.SelectNodes(".//item");

                if (titleNode == null || descriptionNode == null || linkNode == null || listOfItem.Count == 0)
                    result = false;

                nameStr = titleNode.InnerText;
                descriptionStr = descriptionNode.InnerText;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public bool RemoveRSSItem(int rssid)
        {
            bool result = true;
            try
            {
                RSSDBDataContext data = new RSSDBDataContext();
                RSSItem item = (from rssitem in data.RSSItems
                                where rssitem.ID == rssid
                                select rssitem).Single();

                if (item.Tab.UserID != GetCurrentUserID())
                    return false;

                data.RSSItems.DeleteOnSubmit(item);
                data.SubmitChanges();
            }
            catch
            {
                result = false;
            }
            finally
            {
 
            }
            return result;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public RSSItemDTO[] GetAllRSSItems(int tabid)
        {
            List<RSSItemDTO> listOfItem = new List<RSSItemDTO>();
            try
            {
                RSSDBDataContext data = new RSSDBDataContext();

                int currentUserID = GetCurrentUserID();
                List<Tab> listOfTab_test = (from tab in data.Tabs
                                            where tab.ID == tabid && tab.UserID == currentUserID
                                            select tab).ToList();
                List<Share> listOfShare_test = (from share in data.Shares
                                                where share.TabID == tabid && share.AccountID == currentUserID
                                                select share).ToList();
                if (listOfShare_test.Count == 0 && listOfTab_test.Count == 0)
                {
                    listOfItem = new List<RSSItemDTO>();
                    return listOfItem.ToArray();
                }

                List<RSSItem> list = (from rssItem in data.RSSItems
                                      where rssItem.TabID == tabid
                                      select rssItem).ToList();
                foreach (RSSItem item in list)
                {
                    RSSItemDTO temp = new RSSItemDTO();
                    temp.Id = item.ID;
                    temp.Name = item.Name;
                    temp.Description = item.Description;
                    temp.RSSLink = item.RSSLink;
                    temp.TabID = item.TabID;
                    listOfItem.Add(temp);
                }
            }
            catch
            {
                listOfItem = null;
            }
            finally
            {
                
            }
            return listOfItem.ToArray();
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string GetRSSResult(int rssid, int count)
        {
            string resultStr = "";
            try
            {
                RSSDBDataContext data = new RSSDBDataContext();
                List<RSSItem> items = (from rssitem in data.RSSItems
                                        where rssitem.ID == rssid
                                        select rssitem).ToList();
                if (items.Count == 0)
                    return CreateXmlErrorMessage("Error!", "Not exist RSSItem with ID = " + rssid.ToString());
                RSSItem item = items[0];

                //kiểm tra
                bool test_1 = true;
                bool test_2 = true;
                int currentUserID = GetCurrentUserID();
                if (item.Tab.UserID != currentUserID)
                    test_1 = false;
                int tabParentID = item.TabID;
                List<Share> listOfShare_test = (from share in data.Shares
                                                where share.TabID == tabParentID && share.AccountID == currentUserID
                                                select share).ToList();
                if (listOfShare_test.Count == 0)
                    test_2 = false;
                if (!test_1 && !test_2)
                {
                    return CreateXmlErrorMessage("Error!", "Permission Denied");
                }
                //------------------------------------------------------

                if (item.PluginID == null)
                {
                    string rssLink = item.RSSLink;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rssLink);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(reader.ReadToEnd());

                    XmlNode rssNode = doc.DocumentElement.SelectSingleNode("//rss");
                    XmlNode channelNode = rssNode.SelectSingleNode("./channel");

                    XmlNodeList listOfItemnNode = channelNode.SelectNodes(".//item");
                    int nItemNode = listOfItemnNode.Count;

                    for (int i = count; i < nItemNode; i++)
                    {
                        if (i < 0)
                            continue;
                        channelNode.RemoveChild(listOfItemnNode[i]);
                    }
                    resultStr = doc.OuterXml;
                }
                else
                {
                    List<RSSPlugin> itemPlugins = (from plu in data.RSSPlugins
                                                  where plu.ID == item.PluginID
                                                  select plu).ToList();
                    if (itemPlugins.Count == 0)
                        return CreateXmlErrorMessage("Error!", "Not exist RSSItem with ID = " + rssid.ToString());

                    RSSPlugin itemPlugin = itemPlugins[0];

                    string[] fileNames = Directory.GetFiles(Server.MapPath("~") + @"\bin", itemPlugin.DLLName);
                    foreach (string fileName in fileNames)
                    {
                        Assembly asm = Assembly.LoadFile(fileName);
                        Type[] types = asm.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.GetInterface("IRSSPlugin") != null)
                            {
                                IRSSPlugin pluginObject = Activator.CreateInstance(type) as IRSSPlugin;
                                return pluginObject.GetRSSResult(count);
                            }
                        }
                    }
                }
            }
            catch
            {
                return CreateXmlErrorMessage("Error!", "Some thing wrong" + rssid.ToString());
            }
            finally
            {
                
            }
            return resultStr;
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
        #endregion Deo

        #region Advance
        
        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string[] GetAllRSSPluginLink()
        {
            List<String> result = new List<string>();
            try
            {
                string[] fileNames = Directory.GetFiles(Server.MapPath("~") + @"\bin" , "*.dll");
                foreach (string fileName in fileNames)
                {
                    Assembly asm = Assembly.LoadFile(fileName);

                    Type[] types = asm.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.GetInterface("IRSSPlugin") != null)
                        {
                            IRSSPlugin plugin = Activator.CreateInstance(type) as IRSSPlugin;
                            result.Add(plugin.GetRSSWebsiteLink());
                        }
                    }
                }
            }
            catch
            {

            }

            result.Add(Server.MapPath("~"));

            return result.ToArray();
            
        }
        #endregion Advance
        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        private int  GetCurrentUserID()
        {
            return int.Parse(Context.User.Identity.Name);
        }


        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public PluginDTO[] GetAllPlugin()
        { List<PluginDTO> result = new List<PluginDTO>();
            try
            {
                RSSDBDataContext data = new RSSDBDataContext();
                List<RSSPlugin> listOfPlugin = (from plugin in data.RSSPlugins
                                                select plugin).ToList();
                for (int i = 0; i < listOfPlugin.Count; i++)
                {
                    PluginDTO temp = new PluginDTO();
                    temp.PluginID = listOfPlugin[i].ID;
                    temp.Name = listOfPlugin[i].Name;
                    temp.Description = listOfPlugin[i].Description;
                    temp.WebsiteLink = listOfPlugin[i].WebsiteLink;
                    result.Add(temp);
                }
            }
            catch
            {
                result = new List<PluginDTO>();
            }
            finally
            {
 
            }
            return result.ToArray();
       }
        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]

        // 0 - successful
        // 1 - existed
        // 2 - tab not exist
        // 3 - Different onwer
        // 4 - plugin not exist
        // 5 - failed
        public int AddRSSItemWithPlugin(int tabid, int pluginID)
        {
            int result = 5;
            try
            {
                RSSDBDataContext data = new RSSDBDataContext ();
                List<Tab> tabs = (from tab in data.Tabs
                                  where tab.ID == tabid
                                  select tab).ToList();
                if (tabs.Count == 0)
                    return 2;

                Tab tabToAdd = tabs[0];
                if (tabToAdd.UserID != GetCurrentUserID())
                    return 3;

                List<RSSPlugin> plugins = (from plu in data.RSSPlugins
                                           where plu.ID == pluginID
                                           select plu).ToList();
                if (plugins.Count == 0)
                    return 4;

                RSSPlugin plugin = plugins[0];

                //Kiểm tra đã tồn tại plugin trong tab hay chưa
                for (int i = 0; i < tabToAdd.RSSItems.Count; i++)
                {
                    if (tabToAdd.RSSItems[i].PluginID == pluginID)
                        return 1;
                }
                //---------------------------------------------------------------------------

                string[] fileNames = Directory.GetFiles(Server.MapPath("~") + @"\bin", plugin.DLLName);
                foreach (string fileName in fileNames)
                {
                    Assembly asm = Assembly.LoadFile(fileName);
                    Type[] types = asm.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.GetInterface("IRSSPlugin") != null)
                        {
                            IRSSPlugin pluginObject = Activator.CreateInstance(type) as IRSSPlugin;

                            RSSItem newItem = new RSSItem();
                            newItem.Name = pluginObject.GetRSSName();
                            newItem.Description = pluginObject.GetRSSDescription();
                            newItem.RSSLink = pluginObject.GetRSSWebsiteLink();
                            newItem.TabID = tabid;
                            newItem.PluginID = plugin.ID;
                            data.RSSItems.InsertOnSubmit(newItem);
                            data.SubmitChanges();
                            result = 0;
                        }
                    }
                }
            }
            catch
            {
                result = 5;
            }
            finally
            {

            }
            return result;
        }

       [WebMethod]
       [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
       public string GetNewRSSFromTab(int tabid)
        {
            XmlDocument resultXml = new XmlDocument();
            RSSDBDataContext dt = new RSSDBDataContext();
            try
            {
                int currentUserID = GetCurrentUserID();
                List<Tab> listOfTab_test = (from tab in dt.Tabs
                                            where tab.ID == tabid && tab.UserID == currentUserID
                                            select tab).ToList();
                List<Share> listOfShare_test = (from share in dt.Shares
                                                where share.TabID == tabid && share.AccountID == currentUserID
                                                select share).ToList();
                if (listOfShare_test.Count == 0 && listOfTab_test.Count == 0)
                {
                    return CreateXmlErrorMessage("Permission Error", "You are not allowed to view this tab");
                }
                Tab _tabToCheck = dt.Tabs.Single(_s => _s.ID == tabid);
                List<XmlElement> itemElements = new List<XmlElement>();
                foreach (RSSItem rssitem in _tabToCheck.RSSItems)
                {
                    try
                    {
                        string response = GetRSSResult(rssitem.ID, 999);
                        response = response.Replace("pubdate", "pubDate");
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response);
                        foreach (XmlElement element in doc.GetElementsByTagName("item"))
                        {
                            XmlElement pubDate = (XmlElement)element.GetElementsByTagName("pubDate").Item(0);
                            string dateTimeString = pubDate.InnerText;
                            CultureInfo cultureInfo = new CultureInfo("fr-FR", false);
                            DateTime result = new DateTime();

                            if (dateTimeString.Contains("SA") || dateTimeString.Contains("CH"))
                            {
                                dateTimeString = dateTimeString.Replace("SA", "AM").Replace("CH", "PM");
                                result = DateTime.Parse(dateTimeString, cultureInfo);
                            }
                            else
                            {
                                result = DateTime.Parse(dateTimeString);

                            }
                            pubDate.InnerText = result.ToString();
                            itemElements.Add(element);
                        }
                    }
                    catch
                    {
                    }
                }

                var sorted = itemElements.OrderByDescending(c => DateTime.Parse(c.GetElementsByTagName("pubDate")[0].InnerText));
                itemElements = sorted.ToList();

                string title = _tabToCheck.Name;
                string description = "All new from " + title + " tab";
                string link = "/CoreService.asmx";
                resultXml.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><title>" + title + "</title><description>" + description + "</description><link>" + link + "</link></channel></rss>");

                XmlElement channel = (XmlElement)resultXml.DocumentElement.SelectNodes("channel")[0];
                if (itemElements.Count == 0)
                    return CreateXmlErrorMessage("Notification", "This tab is empty");
                int count = 0;
                foreach (XmlElement element in itemElements)
                {
                    XmlElement item = resultXml.CreateElement("item");
                    item.InnerXml = element.InnerXml;
                    channel.AppendChild(item);
                    count++;
                    if (count == 15)
                        break;
                }
            }
            catch
            {
                return CreateXmlErrorMessage("Error!", "Not exist Tab that had ID = " + tabid.ToString());
            }
            return resultXml.InnerXml;
        }
    }
}