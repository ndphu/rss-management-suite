using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RSSReaderWebsite.myCoreService;
using System.Xml;

namespace RSSReaderWebsite
{
    public partial class MainPage : System.Web.UI.Page
    {
        Dictionary<string, bool> tv_Content_ExpandState = new Dictionary<string, bool>();
        private static int currentTabID;
        private static TreeNode currentRssItem;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("Default.aspx");
            }
            if (!IsPostBack)
            {
                InvisibleLoginControl();
                ShowUserInfo();
                currentTabID = 0;
            }
        }

        private void InvisibleLoginControl()
        {
            this.Master.FindControl("lb_UserName").Visible = false;
            this.Master.FindControl("lb_Password").Visible = false;
            this.Master.FindControl("tb_UserName").Visible = false;
            this.Master.FindControl("tb_Password").Visible = false;
            this.Master.FindControl("btn_Login").Visible = false;
            
        }

        private void ShowUserInfo()
        {
            Label tempLabel = ((Label)this.Master.FindControl("lb_CurrentUserName"));
            tempLabel.Text = (string)Session["UserName"];
            tempLabel.Visible = true;
            tempLabel.Font.Size = new FontUnit(FontSize.Large);

            LinkButton tempLinkBtn = (LinkButton)this.Master.FindControl("lbn_LogOut");
            tempLinkBtn.Visible = true;

            this.Master.FindControl("img_User").Visible = true;
        }

        //Load event of UpdatePanel
        protected void up_TabContent_OnLoad(object sender, EventArgs e)
        {
            ClearNotifyMessage();
            RestoreTreeViewState();
            LoadUserContent();
        }

        private void RestoreTreeViewState()
        {
            tv_Content_ExpandState.Clear();
            for (int i = 0; i < tv_Content.Nodes.Count; i++)
                tv_Content_ExpandState.Add(tv_Content.Nodes[i].Value, (bool)tv_Content.Nodes[i].Expanded);
        }

        private void ClearNotifyMessage()
        {
            lb_addResult.Text = "";
            lb_addTabResult.Text = "";
            lb_deleteResult.Text = "";
            lb_ShareResult.Text = "";
            lb_RenameResult.Text = "";
            vld_RenameTab.Text = "";
            vld_ShareTab.Text = "";
        }

        private void LoadUserContent()
        {
            TabDTO[] listOfTab = MyProxy.getProxy().GetAllTabs();
            TabDTO[] listOfSharedTab = MyProxy.getProxy().GetAllSharedTabs();

            tv_Content.Nodes.Clear();
            foreach (TabDTO tab in listOfTab)
            {
                TreeNode tnode = new TreeNode(tab.Name, tab.Id.ToString(), "Content/image/Container.png");
                if (tv_Content_ExpandState.ContainsKey(tab.Id.ToString()))
                    tnode.Expanded = tv_Content_ExpandState[tab.Id.ToString()];
                else
                    tnode.Expanded = false;

                LoadTabContent(tab, tnode);
                tv_Content.Nodes.Add(tnode);

                if (currentTabID == 0)
                {
                    currentTabID = tab.Id;
                    LoadTabContent(tab.Name);
                    currentRssItem = null;
                }
            }

            foreach (TabDTO tab in listOfSharedTab)
            {
                string tabNameToDisPlay = tab.Name + " (from: " + tab.OwnerUsername + " )";
                TreeNode tnode = new TreeNode(tabNameToDisPlay, tab.Id.ToString(), "Content/image/ShareContainer.png");
                if (tv_Content_ExpandState.ContainsKey(tab.Id.ToString()))
                    tnode.Expanded = tv_Content_ExpandState[tab.Id.ToString()];
                else
                    tnode.Expanded = false;

                LoadTabContent(tab, tnode);
                tv_Content.Nodes.Add(tnode);

                if (currentTabID == 0)
                {
                    currentTabID = tab.Id;
                    LoadTabContent(tab.Name);
                    currentRssItem = null;
                }
            }
        }

        private void LoadTabContent(TabDTO tabparent, TreeNode tnodeparent)
        {
            RSSItemDTO[] listOfRssItem = MyProxy.getProxy().GetAllRSSItems(tabparent.Id);
            foreach (RSSItemDTO item in listOfRssItem)
            {
                TreeNode tnode = new TreeNode(item.Name, item.Id.ToString(), "Content/image/RssIcon.png");
                tnode.ToolTip = item.Description;
                tnodeparent.ChildNodes.Add(tnode);
            }
        }


        protected void tv_Content_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeView tv = (TreeView)sender;
            TreeNode parent = tv.SelectedNode.Parent;
            if (parent != null)
            {
                int currentRssItemID = int.Parse(tv.SelectedNode.Value);

                currentTabID = int.Parse(parent.Value);

                LoadRssContent(currentRssItemID, tv.SelectedNode.Text, tv_Content.SelectedNode);
                HideShareAndRenameUI();
            }
            else
            {
                currentTabID = int.Parse(tv.SelectedNode.Value);

                LoadTabContent(tv.SelectedNode.Text);
            }
        }

        //Add các content lên rss khi click vao 1 rss item
        private void LoadRssContent(int currentID, string target, TreeNode currentNode)
        {
            lbtn_deleteRssItem.Visible = true;
            lbtn_deleteTab.Visible = false;
            lbtn_shareTab.Visible = false;
            lbtn_Rename.Visible = false;

            currentRssItem = currentNode;

            lb_currentPath.Text = target;

            dtl_tabContent.DataSource = null;
            dtl_tabContent.DataBind();

            string xmlStr = MyProxy.getProxy().GetRSSResult(currentID, 15);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);

            XmlNodeList listOfItem = doc.SelectNodes("//item");
            dtl_RssContent.DataSource = listOfItem;
            dtl_RssContent.DataBind();
            int m = dtl_RssContent.Items.Count;
            int count = 0;
            foreach (XmlNode node in listOfItem)
            {
                XmlNode titleNode = node.SelectSingleNode("./title");
                XmlNode linkNode = node.SelectSingleNode("./link");
                XmlNode descriptionNode = node.SelectSingleNode("./description");
                XmlNode pubdateNode = node.SelectSingleNode("./pubDate");
                if (pubdateNode == null)
                    pubdateNode = node.SelectSingleNode("./pubdate");

                if (titleNode != null && linkNode != null)
                {
                    HyperLink hpl = (HyperLink)dtl_RssContent.Items[count].FindControl("hl_title");
                    hpl.Text = titleNode.InnerText;
                    hpl.NavigateUrl = linkNode.InnerText;
                    hpl.ToolTip = linkNode.InnerText;
                }
                if (pubdateNode != null)
                {
                    Label lb = (Label)dtl_RssContent.Items[count].FindControl("lb_time");
                    try
                    {
                        DateTime postTime = DateTime.Parse(pubdateNode.InnerText);
                        lb.Text = postTime.ToLongDateString() + " - " + postTime.ToLongTimeString();
                    }
                    catch
                    {
                        lb.Text = pubdateNode.InnerText;
                    }
                }
                if (descriptionNode != null)
                {
                    Literal lt = (Literal)dtl_RssContent.Items[count].FindControl("lt_newsContent");
                    lt.Text = descriptionNode.InnerText;
                }
                count++;
            }
        }

        //Load nội dung tab và 3 rss item đầu tiên khi click vào 1 container
        private void LoadTabContent(string target)
        {
            lbtn_deleteRssItem.Visible = false;
            lbtn_deleteTab.Visible = true;
            lbtn_shareTab.Visible = true;
            lbtn_Rename.Visible = true;

            lb_currentPath.Text = target;

            dtl_RssContent.DataSource = null;
            dtl_RssContent.DataBind();

            List<RSSItemDTO> listOfItem = MyProxy.getProxy().GetAllRSSItems(currentTabID).ToList();
            dtl_tabContent.DataSource = listOfItem;
            dtl_tabContent.DataBind();
            int count = 0;
            foreach (RSSItemDTO item in listOfItem)
            {
                string xmlStr = MyProxy.getProxy().GetRSSResult(item.Id, 3);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlStr);

                XmlNode channelNode = doc.SelectSingleNode("//channel");
                if (channelNode != null)
                {
                    XmlNode titleNode = channelNode.SelectSingleNode("./title");
                    XmlNode desNode = channelNode.SelectSingleNode("./link");

                    if (titleNode != null && desNode != null)
                    {
                        LinkButton lbtn = (LinkButton)dtl_tabContent.Items[count].FindControl("lbtn_rss_name");
                        lbtn.Text = titleNode.InnerText;
                        lbtn.ToolTip = desNode.InnerText;

                        lbtn.CommandName = titleNode.InnerText;
                        lbtn.CommandArgument = item.Id.ToString();
                    }
                }

                XmlNodeList listOfRSSItem = doc.SelectNodes("//item");
                if (listOfRSSItem.Count == 3)
                {
                    XmlNode titleNode_1 = listOfRSSItem[0].SelectSingleNode("./title");
                    XmlNode linkNode_1 = listOfRSSItem[0].SelectSingleNode("./link");

                    if (titleNode_1 != null && linkNode_1 != null)
                    {
                        HyperLink hpl1 = (HyperLink)dtl_tabContent.Items[count].FindControl("hpl_rss_1");
                        hpl1.Text = titleNode_1.InnerText;
                        hpl1.NavigateUrl = linkNode_1.InnerText;

                    }

                    XmlNode titleNode_2 = listOfRSSItem[1].SelectSingleNode("./title");
                    XmlNode linkNode_2 = listOfRSSItem[1].SelectSingleNode("./link");

                    if (titleNode_2 != null && linkNode_2 != null)
                    {
                        HyperLink hpl2 = (HyperLink)dtl_tabContent.Items[count].FindControl("hpl_rss_2");
                        hpl2.Text = titleNode_2.InnerText;
                        hpl2.NavigateUrl = linkNode_2.InnerText;

                    }

                    XmlNode titleNode_3 = listOfRSSItem[2].SelectSingleNode("./title");
                    XmlNode linkNode_3 = listOfRSSItem[2].SelectSingleNode("./link");

                    if (titleNode_3 != null && linkNode_3 != null)
                    {
                        HyperLink hpl3 = (HyperLink)dtl_tabContent.Items[count].FindControl("hpl_rss_3");
                        hpl3.Text = titleNode_3.InnerText;
                        hpl3.NavigateUrl = linkNode_3.InnerText;
                    }
                }
                count++;
            }
        }

        //Add một link rss mới vào 1 tab
        protected void btn_addNewRssLink_Click(object sender, EventArgs e)
        {
            string url = tb_newRssLink.Text.Trim();
            int result = MyProxy.getProxy().AddRSSItem(currentTabID, url);
            switch (result)
            {
                case 0:
                    {
                        lb_addResult.ForeColor = System.Drawing.Color.Green;
                        lb_addResult.Text = "  Done!";
                        RestoreTreeViewState();
                        LoadUserContent();
                        break;
                    }
                case 1:
                    {
                        lb_addResult.ForeColor = System.Drawing.Color.Orange;
                        lb_addResult.Text = "  Has been existed in your tab!";
                        break;
                    }
                case 2:
                    {
                        lb_addResult.ForeColor = System.Drawing.Color.Red;
                        lb_addResult.Text = "  Incorrect link!";
                        break;

                    }
                case 3:
                    {
                        lb_addResult.ForeColor = System.Drawing.Color.Red;
                        lb_addResult.Text = "  Different Onwer!";
                        break;
                    }
                case 4:
                    {
                        lb_addResult.ForeColor = System.Drawing.Color.Red;
                        lb_addResult.Text = "  Fail!";
                        break;
                    }
            }
        }

        //Sự kiện khi click vào 1 tab bên update panel
        protected void dtl_tabContent_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                int currentRssItemID = int.Parse((string)e.CommandArgument);
                LoadRssContent(currentRssItemID, e.CommandName, GetTreeNodeOfCurrentRSSItem(currentRssItemID));
            }
            catch
            {
 
            }
        }

        private TreeNode GetTreeNodeOfCurrentRSSItem(int id)
        {
            TreeNode result = null;

            if (id != 0)
            {
                foreach (TreeNode node in tv_Content.Nodes)
                {
                    foreach (TreeNode rssNode in node.ChildNodes)
                    {
                        try
                        {
                            int rssNodeID = int.Parse(rssNode.Value);
                            if (rssNodeID == id)
                                return rssNode;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
            return result;
        }

        //delete rss item
        protected void lbtn_deleteRssItem_Click(object sender, EventArgs e)
        {
            if (currentRssItem == null)
                return;
            int tabParentID = int.Parse(currentRssItem.Parent.Value);
            int rssIdToDel = int.Parse(currentRssItem.Value);

            if (MyProxy.getProxy().RemoveRSSItem(rssIdToDel))
            {
                RestoreTreeViewState();
                LoadUserContent();
                TreeNode tabParent = GetTreeNodeOfCurrentTab(currentTabID);
                if (tabParent.ChildNodes.Count > 0)
                {
                    int newID = int.Parse(tabParent.ChildNodes[0].Value);
                    string newTarget = tabParent.ChildNodes[0].Text;
                    LoadRssContent(newID, newTarget, tabParent.ChildNodes[0]);
                }
                else
                {
                    RestoreTreeViewState();
                    LoadUserContent();
                    LoadTabContent(tabParent.Text);
                }
            }
            else
            {
                lb_deleteResult.ForeColor = System.Drawing.Color.Red;
                lb_deleteResult.Text = "Different Owner!";
            }
        }

        private TreeNode GetTreeNodeOfCurrentTab(int id)
        {
            TreeNode result = null;

            foreach (TreeNode node in tv_Content.Nodes)
            {
                int nodeID = int.Parse(node.Value);
                if (nodeID == id)
                    return node;
            }

            return result;
        }

        //Add tab
        protected void btn_addNewTab_Click(object sender, EventArgs e)
        {
            string newTabName = tb_newTabNew.Text.Trim();
            int result = MyProxy.getProxy().AddTab(newTabName);
            switch (result)
            {
                case 0:
                    {
                        lb_addTabResult.ForeColor = System.Drawing.Color.Green;
                        lb_addTabResult.Text = "Done!";
                        RestoreTreeViewState();
                        LoadUserContent();
                        break;
                    }
                case 1:
                    {
                        lb_addTabResult.ForeColor = System.Drawing.Color.Orange;
                        lb_addTabResult.Text = "Existed!";
                        break;
                    }
                case 2:
                    {
                        lb_addTabResult.ForeColor = System.Drawing.Color.Red;
                        lb_addTabResult.Text = "Fail!";
                        break;
                    }
            }
        }
        
        //Delete tab
        protected void lbtn_deleteTab_Click(object sender, EventArgs e)
        {
            int result = MyProxy.getProxy().RemoveTab(currentTabID);
            switch (result)
            {
                case 0:
                    {
                        currentTabID = 0;
                        currentRssItem = null;
                        RestoreTreeViewState();
                        LoadUserContent();
                        if (currentTabID == 0)
                        {
                            dtl_RssContent.DataSource = null;
                            dtl_RssContent.DataBind();

                            dtl_tabContent.DataSource = null;
                            dtl_tabContent.DataBind();

                            lbtn_deleteRssItem.Visible = false;
                            lbtn_deleteTab.Visible = false;
                            lbtn_shareTab.Visible = false;
                            lb_currentPath.Text = "";

                        }
                        break;
                    }
                case 1:
                    {
                        lb_deleteResult.ForeColor = System.Drawing.Color.Red;
                        lb_deleteResult.Text = "Different Owner!";
                        break;
                    }
                case 2:
                    {
                        lb_deleteResult.ForeColor = System.Drawing.Color.Red;
                        lb_deleteResult.Text = "Not exist!";
                        break;
                    }
                case 3:
                    {
                        lb_deleteResult.ForeColor = System.Drawing.Color.Red;
                        lb_deleteResult.Text = "Fail!";
                        break;
                    }
            }
        }



        protected void lbtn_shareTab_Click(object sender, EventArgs e)
        {
            ShowShareUI();
        }

        private void ShowShareUI()
        {
            lb_UserNameToShare.Visible = true;
            tb_UserNameToShare.Visible = true;
            btn_ShareTab.Visible = true;
            lbtn_HideShareUI.Visible = true;
            //vld_ShareTab.Visible = true;
            lb_ShareResult.Visible = true;
            lb_ShareResult.Text = "";

            lb_NewTabName.Visible = false;
            tb_NewTabName.Visible = false;
            btn_RenameTab.Visible = false;
            lbtn_HideRenameUI.Visible = false;
            //vld_RenameTab.Visible = false;
            lb_RenameResult.Visible = false;
        }

        private void HideShareAndRenameUI()
        {
            lb_UserNameToShare.Visible = false;
            tb_UserNameToShare.Visible = false;
            btn_ShareTab.Visible = false;
            lbtn_HideShareUI.Visible = false;
            //vld_ShareTab.Visible = false;
            lb_ShareResult.Visible = false;

            lb_NewTabName.Visible = false;
            tb_NewTabName.Visible = false;
            btn_RenameTab.Visible = false;
            lbtn_HideRenameUI.Visible = false;
            //vld_RenameTab.Visible = false;
            lb_RenameResult.Visible = false;
        }

        private void ShowRenameUI()
        {
            lb_UserNameToShare.Visible = false;
            tb_UserNameToShare.Visible = false;
            btn_ShareTab.Visible = false;
            lbtn_HideShareUI.Visible = false;
            //vld_ShareTab.Visible = false;
            lb_ShareResult.Visible = false;

            lb_NewTabName.Visible = true;
            tb_NewTabName.Visible = true;
            btn_RenameTab.Visible = true;
            lbtn_HideRenameUI.Visible = true;
            //vld_RenameTab.Visible = true;
            lb_RenameResult.Visible = true;
            lb_RenameResult.Text = "";
        }

        protected void lbtn_Rename_Click(object sender, EventArgs e)
        {
            ShowRenameUI();
        }


        protected void lbtn_HideShareUI_Click(object sender, EventArgs e)
        {
            HideShareAndRenameUI();
        }

        protected void lbtn_HideRenameUI_Click(object sender, EventArgs e)
        {
            HideShareAndRenameUI();
        }


        //Share tab
        protected void btn_ShareTab_Click(object sender, EventArgs e)
        {
            string userName = tb_UserNameToShare.Text.Trim();
            if (userName == "")
            {
                lb_ShareResult.ForeColor = System.Drawing.Color.Red;
                lb_ShareResult.Text = "This user-name is empty";
                return;
            }
            int result = MyProxy.getProxy().ShareTab(currentTabID, userName);
            switch (result)
            {
                case 0:
                    {
                        lb_ShareResult.ForeColor = System.Drawing.Color.Green;
                        lb_ShareResult.Text = "Done!";
                        break;
                    }
                case 1:
                    {
                        lb_ShareResult.ForeColor = System.Drawing.Color.Red;
                        lb_ShareResult.Text = "Different owner!";
                        break;
                    }
                case 2:
                    {
                        lb_ShareResult.ForeColor = System.Drawing.Color.Red;
                        lb_ShareResult.Text = "This tab doesn't exist!";
                        break;
                    }
                case 3:
                    {
                        lb_ShareResult.ForeColor = System.Drawing.Color.Red;
                        lb_ShareResult.Text = "Doesn't exist!";
                        break;
                    }
                case 4:
                    {
                        lb_ShareResult.ForeColor = System.Drawing.Color.Red;
                        lb_ShareResult.Text = "Can't share yourseft!";
                        break;
                    }
                case 5:
                    {
                        lb_ShareResult.ForeColor = System.Drawing.Color.Red;
                        lb_ShareResult.Text = "Fail!";
                        break;
                    }
            }
        }

        //Rename
        protected void btn_RenameTab_Click(object sender, EventArgs e)
        {
            string newTabName = tb_NewTabName.Text.Trim();
            if (newTabName == "")
            {
                lb_RenameResult.ForeColor = System.Drawing.Color.Red;
                lb_RenameResult.Text = "Tab-name is empty!";
                return;
            }
            int result = MyProxy.getProxy().RenameTab(currentTabID, newTabName);
            switch(result)
            {
                case 0:
                    {
                        lb_RenameResult.ForeColor = System.Drawing.Color.Green;
                        lb_RenameResult.Text = "Done!";
                        RestoreTreeViewState();
                        LoadUserContent();
                        break;
                    }
                case 1:
                    {
                        lb_RenameResult.ForeColor = System.Drawing.Color.Red;
                        lb_RenameResult.Text = "Different owner!";
                        break;
                    }
                case 2:
                    {
                        lb_RenameResult.ForeColor = System.Drawing.Color.Red;
                        lb_RenameResult.Text = "Doesn't exist!";
                        break;
                    }
                case 3:
                    {
                        lb_RenameResult.ForeColor = System.Drawing.Color.Red;
                        lb_RenameResult.Text = "Duplicate name!";
                        break;
                    }
                case 4:
                    {
                        lb_RenameResult.ForeColor = System.Drawing.Color.Red;
                        lb_RenameResult.Text = "Fail!";
                        break;
                    }
            }
        }
    }
}