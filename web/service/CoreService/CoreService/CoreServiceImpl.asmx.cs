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
        public bool AddTab(string tabName)
        {
            bool result = false;
            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                int UserID = GetCurrentUserID();

                Tab newTab = new Tab()
                {
                    Name = tabName,
                    UserID = UserID,
                };

                dt.Tabs.InsertOnSubmit(newTab);
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
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public bool RemoveTab(int tabid)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabToDelete = dt.Tabs.Single(_tab => _tab.ID == tabid && _tab.UserID == GetCurrentUserID());
                var shares = dt.Shares.Where(share => share.TabID == tabToDelete.ID);
                dt.Shares.DeleteAllOnSubmit(shares);
                dt.Tabs.DeleteOnSubmit(tabToDelete);
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
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public bool ShareTab(int tabid, string userName)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabToShare = dt.Tabs.Single(_tab => _tab.ID == tabid && _tab.UserID == GetCurrentUserID());

                Share share = new Share();
                share.TabID = tabToShare.ID;

                var IDUserToShare = dt.Accounts.Single(account => account.Username.CompareTo(userName) == 0).ID;

                share.AccountID = IDUserToShare;

                dt.Shares.InsertOnSubmit(share);
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
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public bool RenameTab(int tabid, string newName)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabToRename = dt.Tabs.Single(_tab => _tab.ID == tabid && _tab.UserID == GetCurrentUserID());
                tabToRename.Name = newName;

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
            throw new NotImplementedException();
        }
        #endregion Advance

        private int  GetCurrentUserID()
        {
            return int.Parse(Context.User.Identity.Name);
        }
    }
}