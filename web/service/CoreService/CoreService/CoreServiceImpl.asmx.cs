using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

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
        #region Phu
        [WebMethod]
        public bool CheckValidUsername(string username)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool Register(string username, string password)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool Logout()
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool AddTab(Tab tab)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool RemoveTab(Tab tab)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool ShareTab(Tab tab, string userName)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public bool RenameTab(Tab tab, string newName)
        {
            throw new NotImplementedException();
        }
        [WebMethod]
        public Tab[] GetAllTabs()
        {
            throw new NotImplementedException();
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
    }
}