using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreService
{
    public interface ICoreService
    {
        #region Phu
        bool CheckValidUsername(string username);

        bool Register(string username, string password);

        bool Login(string username, string password);

        bool Logout();

        bool AddTab(Tab tab);

        bool RemoveTab(Tab tab);

        bool ShareTab(Tab tab, string userName);

        bool RenameTab(Tab tab, string newName);

        Tab[] GetAllTabs();
        #endregion Phu

        #region Deo
        bool AddRSSItem(Tab tab, RSSItem rssItem);

        bool RemoveRSSItem(Tab tab, RSSItem rssItem);

        RSSItem[] GetAllRSSItems(Tab tab);

        string GetRSSResult(RSSItem rssItem, int count);
        #endregion Deo

        string GetRSSResultHTML(RSSItem rssItem, int count);

        string[] GetAllRSSPluginLink();
    }
}