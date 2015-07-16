<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSuperbowl.aspx.cs" Inherits="HFL.AddSuperbowl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Superbowl</title>
    <link href="HFL.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <table id="tabSuperbowl" style="font-size: 18px; font-family: Trebuchet MS" runat="server">
            <tr> <td class="tableTitle">Owner</td> <td class="tableTitle">Week 15</td> <td></td> <td class="tableTitle">Week 16</td></tr>
            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_0" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_0" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_1" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_1" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_2" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_2" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_3" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_3" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_4" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_4" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_5" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_5" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_6" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_6" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_7" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_7" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_8" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_8" visible="false" /></td> <td style="color: Red"></td> </tr>

            <tr> <td></td>
                <td><input type="text" class="scoreText" runat="server" id="person15_9" visible="false" /></td> <td style="color: Red"></td>
                <td><input type="text" class="scoreText" runat="server" id="person16_9" visible="false" /></td> <td style="color: Red"></td> </tr>
        </table>

        <br />

        <div class="tableTitle">Do you want to show the superbowl?</div>
        <asp:RadioButtonList ID="rbs" runat="server" CssClass="tableTitle">
            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:RadioButtonList>

        <br />

        <asp:Button runat="server" ID="btnSubmit" Text="Submit" Visible="true" OnClick="supSubmit_Click" />

        <br />
        
        <div id="dSuccess" class="tableTitle" runat="server"></div>
    </form>
</body>
</html>
