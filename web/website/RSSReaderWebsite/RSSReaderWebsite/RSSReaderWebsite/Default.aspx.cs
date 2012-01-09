using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RSSReaderWebsite
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_CreateAccount_Click(object sender, EventArgs e)
        {
            string username = tb_UserName.Text;
            string password = tb_PassWord.Text;
            if (!MyProxy.getProxy().CheckValidUsername(username))
            {
                lb_UNConflictStateMent.Text = "This user-name was already used!";
            }
            else
            {
                if (MyProxy.getProxy().Register(username, password))
                {
                    
                    Response.Redirect("RegisterResult.aspx");
                }
            }
        }

        protected void up_Content_Load(object sender, EventArgs e)
        {

        }
    }
}
