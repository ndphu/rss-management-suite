<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RegisterResult.aspx.cs" Inherits="RSSReaderWebsite.RegisterResult" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="register_result_dialog" align="center">
        <label style="font-size: larger">
            Your account has been created successfully</label>
        <br />
        <label>
            You can login now!</label>
        <hr style="width: 50%" />
        <br />
        <asp:LinkButton runat="server" ID="lbtn_GoToHP" Text="Return to register page" OnClick="lbtn_GoToHP_Click"></asp:LinkButton>
    </div>
</asp:Content>
