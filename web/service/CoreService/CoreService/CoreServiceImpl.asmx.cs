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
                var tabs = dt.Tabs.Where(tab => tab.Name.CompareTo(tabName) == 0);
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
        // 3 - failed
        public int AddRSSItem(int tabid, string rsslink)
        {
            int result = 0;
            try
            {
                if (IsExist(rsslink))
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

                RSSItem newRssItem = new RSSItem();
                newRssItem.Name = nameStr;
                newRssItem.Description = descriptionStr;
                newRssItem.RSSLink = rsslink;
                newRssItem.TabID = tabid;

                tabToInsert.RSSItems.Add(newRssItem);

                data.SubmitChanges();
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

        private bool IsExist(string newRssLink)
        {
            RSSDBDataContext data = new RSSDBDataContext();
            List<RSSItem> listOfItem = (from rssitem in data.RSSItems
                                        where rssitem.RSSLink == newRssLink
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
        public string GetRSSResult(int rssid, int count)
        {
            string resultStr = "";
            try
            {
                RSSDBDataContext data = new RSSDBDataContext();
                RSSItem item = (from rssitem in data.RSSItems
                                where rssitem.ID == rssid
                                select rssitem).Single();
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
            catch
            {
                resultStr = "";
            }
            finally
            {
                
            }
            return resultStr;
        }
        #endregion Deo

        #region Advance
        [WebMethod]
        public string GetRSSResultHTML(RSSItem rssItem, int count)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public string[] GetAllRSSPluginLink()
        {
            List<String> result = new List<string>();
            try
            {
                string[] fileNames = Directory.GetFiles(Server.MapPath("~") , "*.dll");
                foreach (string fileName in fileNames)
                {
                    Assembly asm = Assembly.LoadFile(fileName);

                    Type[] types = asm.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.GetInterface("IRSSPlugin") != null)
                        {
                            IRSSPlugin plugin = Activator.CreateInstance(type) as IRSSPlugin;
                            result.Add(plugin.GetPluginName());
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

        private int  GetCurrentUserID()
        {
            return int.Parse(Context.User.Identity.Name);
        }
    }
}