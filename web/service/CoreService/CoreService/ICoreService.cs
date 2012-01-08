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

        int AddTab(string tabName);

        int RemoveTab(int tabID);

        int ShareTab(int tabID, string userName);

        int RenameTab(int tabID, string newName);

        TabDTO[] GetAllTabs();

        TabDTO[] GetAllSharedTabs();
        #endregion Phu

        #region Deo
        int AddRSSItem(int tabID, string rsslink);

        bool RemoveRSSItem(int rssid);

        RSSItemDTO[] GetAllRSSItems(int tabID);

        string GetRSSResult(int rssid, int count);
        #endregion Deo

        string GetRSSResultHTML(RSSItem rssItem, int count);

        string[] GetAllRSSPluginLink();
    }
}