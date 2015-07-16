<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HallOfFame.aspx.cs" Inherits="HFL.HallOfFame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HFL Hall of Fame</title>
    <link href="HFL.css" rel="stylesheet" type="text/css" />
</head>
<body style="font-family: Trebuchet MS">
    <form id="form1" runat="server">
        <div class="pageTitle" style="font-size: 28px; text-align: center">~&nbsp;&nbsp;HFL Hall of Fame&nbsp;&nbsp;~</div>

        <br />

        <a class="mainlink" style="background-color: White" href="HFL.aspx">Return to HFL homepage</a>

        <br />

        <center>
            <table id="tabHall" class="tabHall" runat="server">
                <tr class="titleLine" style="border-top: 1px solid black">
                    <td>Year</td>
                    <td>Owner</td>
                    <td>Team Name</td>
                    <td>Rosters</td>
                    <td style="padding-left: 10px; padding-right: 10px">Year Summary</td>
                    <td>Drafts</td>
                </tr>
            </table>
        </center>
    </form>
</body>
</html>
