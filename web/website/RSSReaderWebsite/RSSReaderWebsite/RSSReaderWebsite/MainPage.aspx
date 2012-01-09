<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MainPage.aspx.cs" Inherits="RSSReaderWebsite.MainPage" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <link href="Styles/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/jqueryUI.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:UpdatePanel ID="up_RssContent" runat="server" OnLoad="up_TabContent_OnLoad">
            <ContentTemplate>
                <asp:Label ID="Label1" runat="server" Text="Your current target: "></asp:Label>
                <asp:Label ID="lb_currentPath" runat="server" Text="" Font-Bold="true"></asp:Label>
                <div>
                    <asp:Label ID="lb_addnewRssLink" runat="server" Text="New RSS-link:"></asp:Label>
                    <asp:Label ID="lb_addResult" runat="server" Text="" Font-Bold="true"></asp:Label>
                    <table style="width: 50%">
                        <tr>
                            <td align="left" style="width: 75%">
                                <asp:TextBox ID="tb_newRssLink" runat="server" Width="95%"></asp:TextBox>
                            </td>
                            <td align="left" style="width: 25%">
                                <asp:Button ID="btn_addNewRssLink" runat="server" Text="Add" Width="100%" BorderStyle="Ridge"
                                    ValidationGroup="CheckRSS" OnClick="btn_addNewRssLink_Click" />
                            </td>
                        </tr>
                    </table>
                    <asp:RequiredFieldValidator ID="vdc_TestUserNameEmpty" runat="server" ErrorMessage="The RSS-link is empty"
                        ControlToValidate="tb_newRssLink" ForeColor="Red" ValidationGroup="CheckRSS"></asp:RequiredFieldValidator>
                </div>
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 20%" align="left" valign="top">
                            <div style="width: 100%">
                                <div>
                                    <asp:Label ID="lb_addNewTab" runat="server" Text="New tab name:"></asp:Label>
                                    <asp:Label ID="lb_addTabResult" runat="server" Font-Bold="true"></asp:Label>
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="left" style="width: 75%">
                                                <asp:TextBox ID="tb_newTabNew" runat="server" Width="95%"></asp:TextBox>
                                            </td>
                                            <td align="left" style="width: 25%">
                                                <asp:Button ID="btn_addNewTab" runat="server" Text="Add" Width="100%" BorderStyle="Ridge"
                                                    ValidationGroup="CheckTabNew" onclick="btn_addNewTab_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Tab-name is empty"
                                        ControlToValidate="tb_newTabNew" ForeColor="Red" ValidationGroup="CheckTabNew"></asp:RequiredFieldValidator>
                                </div>
                                <div>
                                    <asp:TreeView ID="tv_Content" runat="server" ShowLines="true" CssClass="tv_Content"
                                        OnSelectedNodeChanged="tv_Content_SelectedNodeChanged" >
                                    </asp:TreeView>
                                </div>
                            </div>
                        </td>
                        <td style="width: 80%" valign="top">
                            <table width="100%">
                                <tr>
                                    <td style="width: 50%">
                                        <div style="text-align: left; vertical-align: top">
                                            <asp:LinkButton ID="lbtn_shareTab" runat="server" Font-Bold="true" ForeColor="Green" Visible="false">Share this TAB</asp:LinkButton>
                                            <br />
                                        </div>
                                    </td>
                                    <td style="width: 50%">
                                        <div style="text-align: right; vertical-align: top">
                                            <asp:LinkButton ID="lbtn_deleteRssItem" runat="server" Font-Bold="true" ForeColor="Red"
                                                OnClick="lbtn_deleteRssItem_Click" Visible="false">Delete this GADGET</asp:LinkButton>
                                            <asp:LinkButton ID="lbtn_deleteTab" runat="server" Font-Bold="true" 
                                                ForeColor="Red" onclick="lbtn_deleteTab_Click" Visible="false">Delete this TAB</asp:LinkButton>
                                            <br />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <asp:DataList ID="dtl_tabContent" runat="server" Width="100%" BackColor="White" BorderColor="#CCCCCC"
                                BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Both"
                                OnItemCommand="dtl_tabContent_ItemCommand">
                                <FooterStyle BackColor="#CCCC99" ForeColor="Black" />
                                <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White" />
                                <ItemTemplate>
                                    <table style="width: 100%;">
                                        <tr>
                                            <div>
                                                <asp:LinkButton ID="lbtn_rss_name" runat="server" Font-Size="Large" ForeColor="Firebrick"></asp:LinkButton>
                                            </div>
                                        </tr>
                                        <tr>
                                            <div>
                                                <asp:HyperLink ID="hpl_rss_1" runat="server"></asp:HyperLink>
                                                <br />
                                                <asp:HyperLink ID="hpl_rss_2" runat="server"></asp:HyperLink>
                                                <br />
                                                <asp:HyperLink ID="hpl_rss_3" runat="server"></asp:HyperLink>
                                                <br />
                                            </div>
                                        </tr>
                                    </table>
                                    <br />
                                </ItemTemplate>
                                <SelectedItemStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White" />
                            </asp:DataList>
                            <asp:DataList ID="dtl_RssContent" runat="server" Width="100%" BackColor="White" BorderColor="#CC9966"
                                BorderStyle="None" BorderWidth="1px" CellPadding="4" GridLines="Both">
                                <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                <ItemStyle BackColor="White" ForeColor="#330099" />
                                <ItemTemplate>
                                    <table style="width: 100%;">
                                        <tr>
                                            <div>
                                                <td align="left" style="width: 70%">
                                                    <asp:HyperLink ID="hl_title" runat="server" Font-Size="Large"></asp:HyperLink>
                                                </td>
                                                <td align="right" style="width: 30%">
                                                    <asp:Label ID="lb_time" runat="server" Text=""></asp:Label>
                                                </td>
                                            </div>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div id="div_newsContent" runat="server" style="width: 100%">
                                                    <asp:Literal ID="lt_newsContent" runat="server"></asp:Literal>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </ItemTemplate>
                                <SelectedItemStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                            </asp:DataList>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
