<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HFL Old.aspx.cs" Inherits="HFL.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HFL</title>
    <link href="HFL.css" rel="stylesheet" type="text/css" />
</head>
<body style="margin: 10px 0px 20px 30px;">
    <form id="form1" runat="server">
        
        <table id="tabTitle" style="background-color: #e1f2fe" runat="server"> <tr id="rowone">
            <td>
                <div id="dTitle" runat="server"></div>
            </td>
            <td id="colTwo"></td>
            <td>
                <label for="selYear" class="pageTitle" style="font-size: 20px">View other year:</label>
                <select id="selYear" onchange="selYear_SelectedIndexChanged(this.value)">
                <option></option>
                <option>Current Year</option>
                </select>
            </td>
        </tr> </table>

        <script type="text/javascript">
            select = document.getElementById('selYear');
            for (i = 1997; i <= "<%= currentYear %>"; i++) {
                var opt = document.createElement('option');
                opt.value = i;
                opt.innerHTML = i;
                select.appendChild(opt);
            }

            function selYear_SelectedIndexChanged(selVal)
            {
                if (selVal == "")
                    return;
                else if (selVal == "Current Year")
                    window.open("HFL.aspx?year=" + "<%= currentYear %>", '_blank');
                else if (selVal >= 2014)
                    window.open("HFL.aspx?year=" + selVal, '_blank');
                else {
                    var request = new XMLHttpRequest();
                    request.open("GET", "xml/HallOfFame.xml", false);
                    request.send();
                    var xml = request.responseXML;
                    var years = xml.getElementsByTagName("year");
                    for (var i = 0; i < years.length; i++) {
                        var year = years[i];
                        if (year.getAttribute('id') == selVal) {
                            window.open("<%= oldHFLSiteLocation %>" + year.getAttribute('address'), '_blank');
                        }
                    }
                }
            }
        </script>

        <br />

        <table id="showWeeks" runat="server">
            <tr class="tableTitle">
                <td>
                    <div id="dScores" runat="server"></div>
                </td>
                <td>
                    <span style="font-size: 18px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Show scores between weeks&nbsp;</span>
                    <asp:DropDownList id="selWeekStart" runat="server" autopostback="true" OnSelectedIndexChanged="selWeekStart_SelectedIndexChanged">
                    </asp:DropDownList>
                    <span style="font-size: 18px;">&nbsp;and&nbsp;</span>
                    <asp:DropDownList id="selWeekEnd" runat="server" autopostback="true" OnSelectedIndexChanged="selWeekEnd_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>

        <table id="tabScores" class="table" runat="server"></table>

        <br />

        <div id="dStandings" class="tableTitle" runat="server"></div>
        <table id="tabStandings" class="table" runat="server"></table>

        <br />

        <div id="dSuperbowl" class="tableTitle" runat="server"></div>
        <table id="tabSuperbowl" class="table" runat="server"></table>
        <br />
        <div id="dWinner" class="tableTitle" style="color: Red; font-size: 24px" runat="server"></div>

        <br />

        <div id="dVictorySpeech" class="tableTitle" style="font-size: 20px"><a href="http://youtu.be/9tRX_ofqpIc">View Guy's victory speech</a></div><br />

        <div id="dDraftOrder" class="tableTitle" runat="server"></div>


        <br />

        <div class="tableTitle">Formations</div>
        <table id="tabFormations" class="formationTable">
            <tr class="regRow"><td class="colTeamName">Standard</td><td>2 QB &nbsp;3 RB &nbsp;3 WR/TE&nbsp; 1 K &nbsp;1 DEF</td></tr>
            <tr class="altRow"><td class="colTeamName">Wishbone</td><td>2 QB &nbsp;4 RB &nbsp;1 WR 1 TE&nbsp; 1 K &nbsp;1 DEF</td></tr>
            <tr class="regRow"><td class="colTeamName">Run and Shoot</td><td>2 QB &nbsp;1 RB &nbsp;5 WR&nbsp; 1 K &nbsp;1 DEF</td></tr>
            <tr class="altRow"><td class="colTeamName">West Coast</td><td>3 QB &nbsp;2 RB &nbsp;1 WR  2 TE &nbsp;1 K &nbsp;1 DEF</td></tr>
        </table>
    </form>
</body>
</html>
