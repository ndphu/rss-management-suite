using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Security.Permissions;

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
        public bool AddTab(Tab tab)
        {
            bool result = false;
            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                int UserID = GetCurrentUserID();

                Tab newTab = new Tab()
                {
                    Name = tab.Name,
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
        public bool RemoveTab(Tab tab)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabToDelete = dt.Tabs.Single(_tab => _tab.ID == tab.ID);

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
        public bool ShareTab(Tab tab, string userName)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                Share share = new Share();
                share.TabID = tab.ID;

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
        public bool RenameTab(Tab tab, string newName)
        {
            bool result = false;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();

                var tabToRename = dt.Tabs.Single(_tab => _tab.ID == tab.ID);
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
        public Tab[] GetAllTabs()
        {
            Tab[] result = null;

            try
            {
                RSSDBDataContext dt = new RSSDBDataContext();
                int currentUserID = GetCurrentUserID();

                var tabs = dt.Tabs.Where(tab => tab.UserID == currentUserID);

                result = tabs.ToArray<Tab>();
            }
            catch
            {
                result = new Tab[0];
            }
            finally
            {
            }

            return result;

        }
        #endregion Phu

        #region Deo
        [WebMethod]
        public bool AddRSSItem(Tab tab, RSSItem rssItem)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool RemoveRSSItem(Tab tab, RSSItem rssItem)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public RSSItem[] GetAllRSSItems(Tab tab)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public string GetRSSResult(RSSItem rssItem, int count)
        {
            throw new NotImplementedException();
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