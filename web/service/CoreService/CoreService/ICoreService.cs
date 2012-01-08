using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Security.Permissions;

namespace CoreService
{
    public interface ICoreService
    {
        #region Phu
        bool CheckValidUsername(string username);

        bool Register(string username, string password);

        bool Login(string username, string password);

        bool Logout();

        bool AddTab(string tabName);

        bool RemoveTab(int tabID);

        bool ShareTab(int tabID, string userName);

        bool RenameTab(int tabID, string newName);

        TabDTO[] GetAllTabs();

        TabDTO[] GetAllSharedTabs();
        #endregion Phu

        #region Deo
        bool AddRSSItem(int tabID, string name, string description, string rsslink);

        bool RemoveRSSItem(int rssid);

        RSSItemDTO[] GetAllRSSItems(int tabID);

        string GetRSSResult(int rssid, int count);
        #endregion Deo

        string GetRSSResultHTML(RSSItem rssItem, int count);

        string[] GetAllRSSPluginLink();
    }
}