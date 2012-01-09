using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RSSReaderWebsite
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Login_Click(object sender, EventArgs e)
        {
            string userName = tb_UserName.Text.Trim();
            string passWord = tb_Password.Text.Trim();
            if (MyProxy.getProxy().Login(userName, passWord))
            {
                Session["UserName"] = userName;
                Response.Redirect("MainPage.aspx");
            }
        }

        protected void lbn_LogOut_Click(object sender, EventArgs e)
        {
            MyProxy.getProxy().Logout();
            Response.Redirect("Default.aspx");
        }

        
    }
}
