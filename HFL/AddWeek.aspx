<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddWeek.aspx.cs" Inherits="HFL.AddWeek" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Week</title>
    <link href="HFL.css" rel="stylesheet" type="text/css" />
</head>
<body style="margin: 10px 10px 10px 10px">
    <form id="form1" runat="server">
        <span id="dPredWeek" class="tableTitle" runat="server"></span>
        <asp:DropDownList ID="selWeek" runat="server" AutoPostBack="true" OnSelectedIndexChanged="selWeek_Change"></asp:DropDownList>

        <br /><br />

        <table id="tabTeams" style="font-size: 18px; font-family: Trebuchet MS" runat="server">
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person0" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person1" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person2" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person3" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person4" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person5" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person6" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person7" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person8" visible="false" /></td> <td style="color: Red"></td> </tr>
            <tr> <td></td> <td><input type="text" class="scoreText" runat="server" id="person9" visible="false" /></td> <td style="color: Red"></td> </tr>
        </table>
        
        <br />
        
        <asp:Button runat="server" ID="btnSubmit" Text="Submit" Visible="false" OnClick="weekSubmit_Click" />
        <br />
        <div id="dSuccess" class="tableTitle" runat="server"></div>
    </form>
</body>
</html>
