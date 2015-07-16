using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Collections;
using System.Web.UI.HtmlControls;
namespace HFL
{

    class Team
    {
        public string sTeamName;
        public string sOwner;
        public List<int> weeklyScores;
        public List<double> weeklyStandings;
        public int i15Points;
        public int i16Points;
        public int iTotalPoints;
        public double dTotalStandings;
        public double dPointsBack;
        public int iAveragePoints;
        public int yahooID;

        public Team()
        {
            sTeamName = "";
            sOwner = "";
            iTotalPoints = 0;
            iAveragePoints = 0;
            dTotalStandings = 0;
            dPointsBack = 0;
            i15Points = 0;
            i16Points = 0;
            weeklyScores = new List<int>();
            weeklyStandings = new List<double>();
            yahooID = 0;
        }
    };


    public partial class WebForm1 : System.Web.UI.Page
    {
        public string currentYear = System.Configuration.ConfigurationManager.AppSettings["Default.Year"],
                      oldHFLSiteLocation = System.Configuration.ConfigurationManager.AppSettings["Old.HFL.Site.Location"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            LoadData(1, 14);
            LoadWeekDropdown();
        }

        //puts options in "selWeekStart" and "selWeekEnd" dropdowns
        private void LoadWeekDropdown()
        {
            selWeekStart.Items.Clear();
            selWeekEnd.Items.Clear();
            for (int i = 1; i <= 14; i++)
            {
                selWeekStart.Items.Add(i.ToString());
                selWeekEnd.Items.Add(i.ToString());
            }
            selWeekStart.SelectedIndex = 0;
            selWeekEnd.SelectedIndex = 13;
        }

        //Used to initialize array of Team objects
        T[] InitializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }

        protected void LoadData(int iStartWeek, int iEndWeek)
        {
            int iWeek, iWeekTotal, iTeamCount, iTeam, iDefaultYear = -1;
            XmlDocument xDoc = new XmlDocument();
            XmlNodeList xmlNL;
            XmlAttribute attrib;
            XmlNode docNode;
            string YahooID;
            bool bShowSuperbowl = false;

            List<Team> HFLTeams = new List<Team>();
            Team totTeam = new Team();


            //if it's a past year get the site address for that year from the hall of fame
            if (Convert.ToInt32(Request.QueryString["year"]) < 2014 && Convert.ToInt32(Request.QueryString["year"]) > 1996)
            {
                xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\HallOfFame.xml");
                xmlNL = xDoc.GetElementsByTagName("year");
                for (int i = 0; i < xmlNL.Count; i++)
                {
                    if (xmlNL[i].Attributes["id"].Value == Request.QueryString["year"])
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["Old.HFL.Site.Location"] + xmlNL[i].Attributes["address"].Value);
                }
            }
            else if (Convert.ToInt32(Request.QueryString["year"]) >= 2014) //else if the year in the address is greater than 2014, load the data for that year
                iDefaultYear = Convert.ToInt32(Request.QueryString["year"]);
            else //otherwise use the default
                iDefaultYear = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Default.Year"]);

            this.Title = "HFL " + iDefaultYear.ToString();

            //title bar
            colTwo.InnerHtml = "<span class=\"pageTitle\">HFL " + iDefaultYear.ToString() + "</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            "<a class=\"mainlink\" href=\"HallOfFame.aspx\">HFL Hall of Fame</a>&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;" +
            "<a class=\"mainlink\" target=\"_blank\" href=\"http://football.fantasysports.yahoo.com/\">Jump to Yahoo</a>&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;" +
            "<a class=\"mainlink\" target=\"_blank\" href=\"Draft" + iDefaultYear.ToString() + ".htm\">" + iDefaultYear.ToString() + " Draft</a>&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;" +
            "<a class=\"mainlink\" href=\"HFL Rules.docx\">HFL Rules</a>&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;" +
            "<a class=\"mainlink\" target=\"_blank\" href=\"http://espn.go.com/nfl/injuries\">NFL Injuries</a>&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;" +
            "<a class=\"mainlink\" target=\"_blank\" href=\"http://espn.go.com/nfl/schedule/_/seasontype/2/week/1\">NFL Schedule</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

            totTeam.sTeamName = "Total";
            HFLTeams.Add(totTeam);

            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + iDefaultYear.ToString() + ".xml");
            xmlNL = xDoc.GetElementsByTagName("team");
            iTeamCount = xmlNL.Count;

            //get all of the owner's data from the team nodes in the xml file
            for (iTeam = 1; iTeam <= iTeamCount; iTeam++)
            {
                Team temp = new Team();
                docNode = xDoc.DocumentElement.SelectSingleNode("teams/team[" + iTeam.ToString() + "]");
                attrib = docNode.Attributes["name"];
                YahooID = docNode.Attributes["yahooID"].Value;
                temp.yahooID = Convert.ToInt32(YahooID);
                temp.sTeamName = "<a href=\"" + System.Configuration.ConfigurationManager.AppSettings["HFL.Yahoo.LeaguePage"] + YahooID.ToString() + "\" target=\"_blank\">" + attrib.Value + "</a>";
                attrib = docNode.Attributes["owner"];
                temp.sOwner = attrib.Value;
                HFLTeams.Add(temp);
            }
            xmlNL = xDoc.GetElementsByTagName("week");
            int iTotalWeeksSoFar = xmlNL.Count;
            if (iTotalWeeksSoFar < iEndWeek)
                iEndWeek = iTotalWeeksSoFar;

            //compiler doesn't know that docNode will always be assigned in the next if statement, so this line is necessary though it doesn't do anything
            docNode = xmlNL[0];

            //get all the data from each week (based on id) that exists in the xml file
            for (iWeek = iStartWeek; iWeek <= iEndWeek; iWeek++)
            {
                iWeekTotal = 0;
                for (int i = 0; i < xmlNL.Count; i++) //xmlNL.Count could be a problem
                    if (xmlNL[i].Attributes["id"].Value == iWeek.ToString())
                    {
                        docNode = xmlNL[i];
                        break;
                    }
                for (iTeam = 1; iTeam <= iTeamCount; iTeam++)
                {
                    attrib = docNode.Attributes[HFLTeams[iTeam].sOwner];
                    HFLTeams[iTeam].weeklyScores.Add(Convert.ToInt32(attrib.Value));
                    iWeekTotal += Convert.ToInt32(attrib.Value);
                }
                HFLTeams[0].weeklyScores.Add(iWeekTotal);
            }
            
            //get the total and average for each team
            for (iTeam = 0; iTeam <= iTeamCount; iTeam++)
                SumTeam(HFLTeams[iTeam]);

            //get superbowl data from the xml file
            docNode = xDoc.SelectSingleNode("hfl/superbowl");
            if (docNode.Attributes["show"].Value == "true")
            {
                bShowSuperbowl = true;

                for (iTeam = 1; iTeam <= iTeamCount; iTeam++)
                {
                    docNode = xDoc.SelectSingleNode("hfl/superbowl/week15");
                    attrib = docNode.Attributes[HFLTeams[iTeam].sOwner];
                    HFLTeams[iTeam].i15Points = Convert.ToInt32(attrib.Value);

                    docNode = xDoc.SelectSingleNode("hfl/superbowl/week16");
                    attrib = docNode.Attributes[HFLTeams[iTeam].sOwner];
                    HFLTeams[iTeam].i16Points = Convert.ToInt32(attrib.Value);
                }
            }

            RenderPoints(iStartWeek, iEndWeek, HFLTeams, bShowSuperbowl);
        }

        //Computes the total points for all the weeks stored in each Team's list
        //Stores the total and average in the Team class's data members
        private void SumTeam(Team myTeam)
        {
            int i, x = 0;
            for (i = 0; i < myTeam.weeklyScores.Count; i++)
                x += myTeam.weeklyScores[i];

            myTeam.iTotalPoints = x;
            myTeam.iAveragePoints = x / myTeam.weeklyScores.Count;
        }

        private void RenderPoints(int iStartWeek, int iEndWeek, List<Team> HFLTeams, bool bShowSup)
        {
            int i, j, iTeamCount, iHi, iHiCell, iVal;
            double dSum, dHi;

            Team currentTeam = new Team(), highestTeam = new Team();
            List<Team> sortedTeams = new List<Team>(), game = new List<Team>(), game2 = new List<Team>(), standings = new List<Team>();


            iTeamCount = HFLTeams.Count - 1;

            //sort the teams by total points
            for (i = 1; i <= iTeamCount; i++)
            {
                iHi = -1;
                iHiCell = 0;
                for (j = 1; j < HFLTeams.Count; j++) //First one is the total
                {
                    currentTeam = HFLTeams[j];
                    if (iHi < currentTeam.iTotalPoints)
                    {
                        iHi = (int)currentTeam.iTotalPoints;
                        highestTeam = currentTeam;
                        iHiCell = j;
                    }
                }
                HFLTeams.Remove(highestTeam);
                sortedTeams.Add(highestTeam);
            }


            //display the first table
            dScores.InnerText = "Overall Points";

            HtmlTableRow newRow = new HtmlTableRow();
            HtmlTableCell cellPts = new HtmlTableCell();
            
            //title line
            string sLine = "<tr class = \"titleLine\"><td class=\"colTeamName\">Owner</td><td class=\"colTeamName\">Team Name</td><td>Total</td><td>Avg.</td>";

            for (i = 1; i < 15; i++)
                sLine += "<td>" + i.ToString() + "</td>";
            sLine += "</tr>";

            //create a row for each team and put in the weekly scores
            for (j = 0; j < iTeamCount; j++)
            {
                currentTeam = sortedTeams[j];
                if (j % 2 == 1)
                    sLine += "<tr class=\"altRow\">";
                else
                    sLine += "<tr class=\"regRow\">";

                sLine += "<td class=\"colTeamName\">" + currentTeam.sOwner + "</td>";
                sLine += "<td class=\"colTeamName\" style=\"width: 230px\">" + currentTeam.sTeamName + "</td>";
                sLine += "<td style=\"width:40px\">" + currentTeam.iTotalPoints.ToString() + "</td>";
                sLine += "<td style=\"width:50px\">" + currentTeam.iAveragePoints.ToString() + "</td>";

                for (i = 0; i < iStartWeek - 1; i++)
                    sLine += "<td>&nbsp;</td>";

                for (i = iStartWeek; i <= iEndWeek; i++)
                    sLine += "<td>" + currentTeam.weeklyScores[i - iStartWeek].ToString() + "</td>";

                for (i = iEndWeek; i < 14; i++)
                    sLine += "<td>&nbsp;</td>";

                sLine += "</tr>";
            }

            //total line
            currentTeam = HFLTeams[0];
            sLine += "<tr class = \"totalLine\">";
            sLine += "<td class=\"colTeamName\">TOTAL</td><td></td>";
            sLine += "<td>" + currentTeam.iTotalPoints.ToString() + "</td>";
            sLine += "<td>" + currentTeam.iAveragePoints.ToString() + "</td>";

            for (i = 0; i < iStartWeek - 1; i++)
                sLine += "<td>&nbsp;</td>";

            for (i = iStartWeek; i <= iEndWeek; i++)
                sLine += "<td>" + currentTeam.weeklyScores[i - iStartWeek].ToString() + "</td>";

            for (i = iEndWeek; i < 14; i++)
                sLine += "<td>&nbsp;</td>";

            sLine += "</tr></table>";
            cellPts.InnerHtml = sLine;
            newRow.Cells.Add(cellPts);
            tabScores.Rows.Clear();
            tabScores.Rows.Insert(0, newRow);
            newRow = null;
            cellPts = null;




            //put sortedTeams into game
            for (i = 0; i < sortedTeams.Count; i++)
            {
                currentTeam = sortedTeams[i];
                game.Add(currentTeam);
                currentTeam = null;
            }

            //go week by week and assign standing value to each
            for (i = 0; i < sortedTeams[0].weeklyScores.Count; i++) ///////////////////////////
            {
                //sort by weekly points
                game2.Clear();
                while (true)
                {
                    iHi = -1;
                    for (j = 0; j < game.Count; j++)
                    {
                        currentTeam = game[j];
                        if (iHi < currentTeam.weeklyScores[i])
                        {
                            iHi = currentTeam.weeklyScores[i];
                            highestTeam = currentTeam;
                        }
                    }
                    game2.Add(highestTeam);
                    game.Remove(highestTeam);
                    if (game.Count == 0)
                        break;
                }

                //assign weekly standing points to each team
                iVal = iTeamCount;
                j = 0;
                while (true)
                {
                    iVal -= 1;
                    currentTeam = game2[j];
                    if (j < game2.Count - 1)
                    {
                        //2 way tie
                        if (currentTeam.weeklyScores[i] == game2[j + 1].weeklyScores[i])
                        {
                            //3 way tie
                            if (j + 2 < game2.Count && game2[j + 1].weeklyScores[i] == game2[j + 2].weeklyScores[i]) //3 way tie!
                            {
                                currentTeam.weeklyStandings.Add((iVal + iVal + iVal - 3) / 3);
                                game2[j + 1].weeklyStandings.Add((double)(iVal + iVal + iVal - 3) / 3);
                                game2[j + 2].weeklyStandings.Add((double)(iVal + iVal + iVal - 3) / 3);
                                j += 3;
                                iVal -= 2;
                            }
                            else
                            {
                                currentTeam.weeklyStandings.Add((double)(iVal + iVal - 1) / 2);
                                game2[j + 1].weeklyStandings.Add((double)(iVal + iVal - 1) / 2);
                                j += 2;
                                iVal -= 1;
                            }

                        }
                        else
                        {
                            currentTeam.weeklyStandings.Add((double)iVal);
                            j += 1;
                        }
                    }
                    else
                    {
                        currentTeam.weeklyStandings.Add((double)iVal);
                        j += 1;
                    }

                    if (j >= game2.Count)
                        break;
                }

                //put game2 back into game
                for (j = game2.Count - 1; j >= 0; j--)
                {
                    currentTeam = game2[j];
                    game2.Remove(currentTeam);
                    game.Add(currentTeam);
                }
            }

            //total the standings for each team
            for (j = 0; j < game.Count; j++)
            {
                dSum = 0;
                currentTeam = game[j];
                for (i = 0; i < currentTeam.weeklyStandings.Count; i++) ////////////////////
                    dSum += currentTeam.weeklyStandings[i];
                currentTeam.dTotalStandings = dSum;
            }
            
            //rank the teams based on their total standings
            game2.Clear();
            while (true)
            {
                dHi = -1;
                for (j = 0; j < game.Count; j++)
                {
                    currentTeam = game[j];
                    if (dHi < currentTeam.dTotalStandings)
                    {
                        dHi = currentTeam.dTotalStandings;
                        highestTeam = currentTeam;
                    }
                }
                game2.Add(highestTeam);
                game.Remove(highestTeam);
                if (game.Count == 0)
                    break;
            }

            //calculate points back
            for (j = 0; j < game2.Count; j++)
            {
                currentTeam = game2[j];
                currentTeam.dPointsBack = game2[0].dTotalStandings - currentTeam.dTotalStandings;
            }

            //put game2 back into game
            for (j = 0; j < game2.Count; j++)
            {
                currentTeam = game2[j];
                game.Add(currentTeam);
                standings.Add(currentTeam); //used for next year's draft order
            }


            //display the second table
            dStandings.InnerText = "Standings";

            newRow = new HtmlTableRow();
            cellPts = new HtmlTableCell();

            //title line
            sLine = "<tr class = \"titleLine\"><td class=\"colTeamName\">Owner</td><td class=\"colTeamName\">Team Name</td><td>Points</td><td>Back</td>";

            for (i = 1; i < 15; i++)
                sLine += "<td>" + i.ToString() + "</td>";
            sLine += "</tr>";

            //create a row for each team and put in the weekly standings
            for (j = 0; j < iTeamCount; j++)
            {
                currentTeam = game[j];
                if (j % 2 == 1)
                    sLine += "<tr class=\"altRow\">";
                else
                    sLine += "<tr class=\"regRow\">";

                sLine += "<td class=\"colTeamName\">" + currentTeam.sOwner + "</td>";
                sLine += "<td class=\"colTeamName\" style=\"width: 230px\">" + currentTeam.sTeamName + "</td>";
                sLine += "<td class=\"ptsBack\" style=\"width:60px\">" + currentTeam.dTotalStandings.ToString() + "</td>";
                sLine += "<td class=\"ptsBack\" style=\"width:60px\">" + currentTeam.dPointsBack.ToString() + "</td>";

                for (i = 0; i < iStartWeek - 1; i++)
                    sLine += "<td>&nbsp;</td>";

                for (i = iStartWeek; i <= iEndWeek; i++)
                    sLine += "<td>" + currentTeam.weeklyStandings[i - iStartWeek].ToString() + "</td>";

                for (i = iEndWeek; i < 14; i++)
                    sLine += "<td>&nbsp;</td>";

                sLine += "</tr>";
            }

            sLine += "</table>";
            cellPts.InnerHtml = sLine;
            newRow.Cells.Add(cellPts);
            tabStandings.Rows.Clear();
            tabStandings.Rows.Insert(0, newRow);
            newRow = null;
            cellPts = null;




            //superbowl
            if (bShowSup)
            {
                //sort teams by superbowl total points so far
                game2.Clear();
                while (true)
                {
                    dHi = -1000; //assumes points back is not more than -1000
                    for (j = 0; j < game.Count; j++)
                    {
                        currentTeam = game[j];
                        if (dHi < (currentTeam.i15Points + currentTeam.i16Points - (currentTeam.dPointsBack * 2)))
                        {
                            dHi = currentTeam.i15Points + currentTeam.i16Points - (currentTeam.dPointsBack * 2);
                            highestTeam = currentTeam;
                        }
                    }
                    game2.Add(highestTeam);
                    game.Remove(highestTeam);
                    if (game.Count == 0)
                        break;
                }

                //put game2 back into game
                for (j = 0; j < game2.Count; j++)
                {
                    currentTeam = game2[j];
                    game.Add(currentTeam);
                }


                //superbowl table
                dSuperbowl.InnerHtml = "Superbowl";

                newRow = new HtmlTableRow();
                cellPts = new HtmlTableCell();

                //title line
                sLine = "<tr class = \"titleLine\"><td class=\"colTeamName\">Owner</td><td class=\"colTeamName\">Team Name</td><td>Week 15</td><td>Deduction</td><td>Week 16</td><td>Deduction</td><td>Superbowl Net Total</td></tr>";

                //insert table data
                for (i = 0; i < iTeamCount; i++)
                {
                    currentTeam = game[i];
                    if (i % 2 == 1)
                        sLine += "<tr class=\"altRow\">";
                    else
                        sLine += "<tr class=\"regRow\">";

                    sLine += "<td class=\"colTeamName\">" + currentTeam.sOwner + "</td>";
                    sLine += "<td class=\"colTeamName\" style=\"width: 230px\">" + currentTeam.sTeamName + "</td>";
                    sLine += "<td>" + currentTeam.i15Points.ToString() + "</td>";
                    sLine += "<td>" + currentTeam.dPointsBack.ToString() + "</td>";
                    sLine += "<td>" + currentTeam.i16Points.ToString() + "</td>";
                    sLine += "<td>" + currentTeam.dPointsBack.ToString() + "</td>";
                    sLine += "<td class=\"colWinner\">" + (currentTeam.i15Points + currentTeam.i16Points - (currentTeam.dPointsBack * 2)).ToString() + "</td>";

                    sLine += "</tr>";
                }

                sLine += "</table>";
                cellPts.InnerHtml = sLine;
                newRow.Cells.Add(cellPts);
                tabSuperbowl.Rows.Clear();
                tabSuperbowl.Rows.Insert(0, newRow);
                newRow = null;
                cellPts = null;

                //detects if the superbowl is done, and if it is displays the winner and next year's draft order
                for (i = 0; i < iTeamCount; i++)
                {
                    if (game[i].i16Points != 0) //assumes nobody scored a 0 in week 16
                    {
                        dWinner.InnerText = game[0].sTeamName.Substring(game[0].sTeamName.IndexOf('>') + 1, game[0].sTeamName.IndexOf("</a>") - game[0].sTeamName.IndexOf('>') - 1) + " (" + game[0].sOwner + ") has won the superbowl!";

                        string sText = (Convert.ToInt32(currentYear) + 1).ToString() + " Draft Order:<br />";
                        j = 0;
                        for (i = iTeamCount - 1; i >= 0; i--)
                        {
                            j++;
                            if (standings[i].sOwner != game[0].sOwner)
                                sText += j.ToString() + ". " + standings[i].sOwner + "<br />";
                        }
                        sText += j.ToString() + ". " + game[0].sOwner;
                        dDraftOrder.InnerHtml = sText;

                        break;
                    }
                }
            }
        }

        //if the week is changed, data is reloaded with the selected number of weeks
        protected void selWeekStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(selWeekStart.SelectedValue) <= Convert.ToInt32(selWeekEnd.SelectedValue))
                LoadData(Convert.ToInt32(selWeekStart.SelectedValue), Convert.ToInt32(selWeekEnd.SelectedValue));
            else
                LoadData(Convert.ToInt32(selWeekEnd.SelectedValue), Convert.ToInt32(selWeekStart.SelectedValue));
        }

        protected void selWeekEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(selWeekStart.SelectedValue) <= Convert.ToInt32(selWeekEnd.SelectedValue))
                LoadData(Convert.ToInt32(selWeekStart.SelectedValue), Convert.ToInt32(selWeekEnd.SelectedValue));
            else
                LoadData(Convert.ToInt32(selWeekEnd.SelectedValue), Convert.ToInt32(selWeekStart.SelectedValue));
        }
    }
}