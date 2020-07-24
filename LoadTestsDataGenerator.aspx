<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoadTestsDataGenerator.aspx.cs" Inherits="SitefinityWebApp.LoadTestsDataGenerator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="GenerationForm" runat="server">
        <div>
            <asp:Label runat="server" ID="Description" Text="This page creates Blogs, Events, News, Users, Dynamic modules for TESTING PURPOSES"></asp:Label>
        </div>
        <div>
            <asp:Button runat="server" ID="GenerateData" Text="Generate" OnClick="Generate_Click" />
        </div>
        <div>
            <asp:Label runat="server" ID="SuccessMessage" Visible="false">Completed</asp:Label>
            <asp:Label runat="server" ID="FailMessage" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>
