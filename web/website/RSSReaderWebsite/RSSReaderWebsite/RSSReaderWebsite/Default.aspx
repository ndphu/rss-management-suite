<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="RSSReaderWebsite._Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div align="center" id="register_page" >
        <div align="center" id="register_dialog" style="height:90%">
            <asp:UpdatePanel ID="up_Content" runat="server" onload="Page_Load">
            <ContentTemplate>
                <label style="font-size: large">
                    Create new account</label>
                <br />
                <label style="font-size: x-small">
                    All information is required</label>
                <br />
                <hr style="width:50%" />
                <div align="center" style="width: 100%">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 47%">
                                <div align="right">
                                    <label>
                                        User name: 
                                    </label>
                                </div>
                            </td>
                            <td style="width: 53%">
                                <div align="left"">                     
                                    <asp:TextBox ID="tb_UserName" name="tb_UserName" runat="server" MaxLength="14"></asp:TextBox>
                                     <label id="lb_TestUN"></label>
                                    <asp:RequiredFieldValidator ID="vdc_TestUserNameEmpty" runat="server" 
                                        ErrorMessage="(*)The user-name is empty"
                                        ControlToValidate="tb_UserName"
                                        ForeColor="Red"
                                        ValidationGroup="Login"></asp:RequiredFieldValidator>
                                   
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:47%">
                                <div align="right">
                                    <label>
                                        Create a password: 
                                    </label>
                                </div>
                            </td>
                            <td style="width:53%">
                                <div align="left"">
                                    <asp:TextBox ID="tb_PassWord" runat="server" MaxLength="14" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="vdc_TestPassWordEmpty" runat="server" 
                                        ErrorMessage="(*)The password is empty"
                                        ControlToValidate="tb_PassWord"
                                        ForeColor="Red"
                                        ValidationGroup="Login"></asp:RequiredFieldValidator>
                                </div>               
                            </td>
                        </tr>
                        <tr>
                            <td style="width:47%">
                                <div align="right">
                                    <label>
                                        Retype password:
                                    </label>
                                </div>
                            </td>
                            <td style="width:53%">
                                <div align="left"">
                                    <asp:TextBox ID="tb_RePassWord" runat="server" MaxLength="14" TextMode="Password"></asp:TextBox>      
                                    <asp:CompareValidator ID="vdc_TestPW" 
                                    runat="server" ErrorMessage="(*)The retyped password is not correct"
                                    ForeColor="Red"
                                    Operator="Equal"
                                    ControlToCompare="tb_PassWord"
                                    ControlToValidate="tb_RePassWord"
                                    ValidationGroup="Login"></asp:CompareValidator>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            <hr style="width:50%"/>
            <asp:Label runat="server" ID="lb_UNConflictStateMent" ForeColor="Red"></asp:Label>
            <br />
            <asp:Button ID="btn_CreateAccount"  runat="server" Text="Create account" 
                    BorderStyle="Ridge" onclick="btn_CreateAccount_Click" ValidationGroup="Login"/>
            </ContentTemplate>
        </asp:UpdatePanel>
        </div>
    </div>
    <div></div>
</asp:Content>
