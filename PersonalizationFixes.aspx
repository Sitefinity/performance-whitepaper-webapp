<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PersonalizationFixes.aspx.cs" Inherits="SitefinityWebApp.PersonalizationFixes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="GenerationForm" runat="server">
        <div>
            <asp:Label runat="server" ID="Description" Text="Personalization fix"></asp:Label>
        </div>
        <div>
            <asp:Button runat="server" ID="Fix" Text="Run" OnClick="Fix_Click" />
        </div>
        <div>
            <asp:Label runat="server" ID="SuccessMessage" Visible="false">Completed</asp:Label>
            <asp:Label runat="server" ID="FailMessage" Visible="false"></asp:Label>
            <br />
            <asp:Literal runat="server" ID="Message"></asp:Literal>
        </div>
    </form>
</body>
</html>
