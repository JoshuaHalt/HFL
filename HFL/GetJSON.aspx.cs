using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Script.Serialization;
using System.IO;

namespace HFL
{
    public partial class GetJSON : System.Web.UI.Page
    {
        public string oldHFLSiteLocation = "", yahooURL = "";
        public int iYearFromVariable = -1, currentYear = -1;
        List<object> lsAllDataJSON = new List<object>(), lsHallOfFame = new List<object>(), lsAddData = new List<object>(), lsYear = new List<object>();

        class Team
        {
            public string sTeamName;
            public string sOwner;
            public List<double> weeklyScores;
            public List<double> weeklyStandings;
            public double i15Points;
            public double i16Points;
            public double iTotalPoints;
            public double dTotalStandings;
            public double dPointsBack;
            public double iAveragePoints;
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
                weeklyScores = new List<double>();
                weeklyStandings = new List<double>();
                yahooID = 0;
            }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            //get the config settings variables from the Settings.xml file, all stored in globals
            GetVarsFromSettings();

            //if there is no command variable, don't do anything
            if (Request.QueryString["command"] == null)
                return;

            //if the command is "getdata", load the data and send it through JSON
            if (Request.QueryString["command"].ToString().ToLower() == "getdata")
            {
                //get the year and load the data, send the data
                iYearFromVariable = Convert.ToInt16(Request.QueryString["year"].ToString().ToLower());
                LoadData(1, 14);
                SendJSON(lsAllDataJSON);
            }
            else if (Request.QueryString["command"].ToString().ToLower() == "gethall")
            {
                GetHall();
                SendJSON(lsHallOfFame);
            }

            else if (Request.QueryString["command"].ToString().ToLower() == "getadddata")
            {
                GetAddData(Convert.ToInt16(Request.QueryString["year"]));
                SendJSON(lsAddData);
            }

            else if (Request.QueryString["command"].ToString().ToLower() == "getyear")
            {
                GetYear();
                SendJSON(lsYear);
            }
        }

        private void SendJSON(object objToSend)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                string json = serializer.Serialize(objToSend);
                Response.Write(json);
            }
            catch (Exception ex)
            {
                List<object> lsobj = new List<object>();
                lsobj.Add("Error: " + ex.Message);
                string json = serializer.Serialize(lsobj);
                Response.Write(json);
            }
        }

        //part of jQuery training
        /*private List<object> getExample()
        {
            List<object> o = new List<object>();
            try
            {

                List<string> lsNames = new List<string>();
                lsNames.Add("Joshua");
                lsNames.Add("Tom Brady");
                lsNames.Add("Russ Wilson");
                lsNames.Add("Beast Mode");
                List<int> lsValue = new List<int>();
                lsValue.Add(1);
                lsValue.Add(2);
                lsValue.Add(3);
                o.Add(lsNames);
                o.Add(lsValue);

                return o;
            }
            catch (Exception ex)
            {
                o.Add(ex.Message);
                return o;
            }
        }*/

        private void LoadData(int iStartWeek, int iEndWeek)
        {
            int iWeek, iTeamCount, iTeam;
            double iWeekTotal;
            XmlDocument xDoc = new XmlDocument();
            XmlNodeList xmlNL;
            XmlAttribute attrib;
            XmlNode docNode;
            string YahooID;
            bool bShowSuperbowl = false;

            List<Team> HFLTeams = new List<Team>();
            Team totTeam = new Team();

            //add year and old site location to JSON
            lsAllDataJSON.Add(iYearFromVariable);
            lsAllDataJSON.Add(oldHFLSiteLocation);

            totTeam.sTeamName = "Total";
            HFLTeams.Add(totTeam);

            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + iYearFromVariable.ToString() + ".xml");
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
                temp.sTeamName = "<a href=\"" + yahooURL + YahooID.ToString() + "\" target=\"_blank\">" + attrib.Value + "</a>";
                attrib = docNode.Attributes["owner"];
                temp.sOwner = attrib.Value;
                HFLTeams.Add(temp);
            }

            xmlNL = xDoc.GetElementsByTagName("week");
            if (xmlNL.Count >= 14) //if there are over 14 weeks, set to show the superbowl
                bShowSuperbowl = true;
            else //otherwise, show only the weeks up until the desired ending week
                if (xmlNL.Count < iEndWeek)
                    iEndWeek = xmlNL.Count;

            //compiler doesn't know that docNode will always be assigned in the next if statement, so this line is necessary though it doesn't do anything
            docNode = xmlNL[0];

            //get all the data from every week (based on id) that exists in the xml file
            for (iWeek = iStartWeek; iWeek <= iEndWeek; iWeek++)
            {
                iWeekTotal = 0;

                //find the week based on id
                for (int i = 0; i < xmlNL.Count; i++) //xmlNL.Count could be a problem
                    if (xmlNL[i].Attributes["id"].Value == iWeek.ToString())
                    {
                        docNode = xmlNL[i];
                        break;
                    }
                //get the score for that week
                for (iTeam = 1; iTeam <= iTeamCount; iTeam++)
                {
                    attrib = docNode.Attributes[HFLTeams[iTeam].sOwner];
                    HFLTeams[iTeam].weeklyScores.Add(Math.Round(Convert.ToDouble(attrib.Value), 1));
                    iWeekTotal += Math.Round(Convert.ToDouble(attrib.Value), 1);
                }
                HFLTeams[0].weeklyScores.Add(Math.Round(iWeekTotal, 1));
            }

            //get the total and average for each team
            for (iTeam = 0; iTeam <= iTeamCount; iTeam++)
                SumTeam(HFLTeams[iTeam]);

            //get superbowl data from the xml file
            if (xmlNL.Count == 15 || xmlNL.Count == 16)
            {
                for (iTeam = 1; iTeam <= iTeamCount; iTeam++)
                {
                    for (int i = 0; i < xmlNL.Count; i++)
                        if (xmlNL[i].Attributes["id"].Value == "15")
                        {
                            docNode = xmlNL[i];
                            break;
                        }
                    attrib = docNode.Attributes[HFLTeams[iTeam].sOwner];
                    HFLTeams[iTeam].i15Points = Convert.ToDouble(attrib.Value);
                }
            }
            if (xmlNL.Count == 16)
            {
                for (iTeam = 1; iTeam <= iTeamCount; iTeam++)
                {
                    for (int i = 0; i < xmlNL.Count; i++)
                        if (xmlNL[i].Attributes["id"].Value == "16")
                        {
                            docNode = xmlNL[i];
                            break;
                        }
                    attrib = docNode.Attributes[HFLTeams[iTeam].sOwner];
                    HFLTeams[iTeam].i16Points = Convert.ToDouble(attrib.Value);
                }
            }


            RenderPoints(iStartWeek, iEndWeek, HFLTeams, bShowSuperbowl);
        }

        //Computes the total points for all the weeks stored in each Team's list
        //Stores the total and average in the Team class's data members
        private void SumTeam(Team myTeam)
        {
            double x = 0;
            if (myTeam.weeklyScores.Count > 14)
                for (int i = 0; i < 14; i++)
                    x += myTeam.weeklyScores[i];
            else
                for (int i = 0; i < myTeam.weeklyScores.Count; i++)
                    x += myTeam.weeklyScores[i];

            myTeam.iTotalPoints = Math.Round(x, 1);
            myTeam.iAveragePoints = Math.Round(x / myTeam.weeklyScores.Count, 1);
        }

        private void RenderPoints(int iStartWeek, int iEndWeek, List<Team> HFLTeams, bool bShowSup)
        {
            int i, j, iTeamCount, iHiCell, iVal;
            double iHi;
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

            //load the data into the list to be sent in a JSON file
            //also loading the "team" with the total line in it
            lsAllDataJSON.Add(sortedTeams);
            lsAllDataJSON.Add(HFLTeams[0]);

            
            
            //put sortedTeams into game
            for (i = 0; i < sortedTeams.Count; i++)
            {
                currentTeam = sortedTeams[i];
                game.Add(currentTeam);
                currentTeam = null;
            }

            //go week by week and assign standing value to each
            for (i = 0; i < sortedTeams[0].weeklyScores.Count; i++)
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
                for (i = 0; i < currentTeam.weeklyStandings.Count; i++)
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

            
            
            //add whether or not to show superbowl data
            lsAllDataJSON.Add(bShowSup);

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



                //detects if the superbowl is done, and if it is displays the winner and next year's draft order
                bool bSupDone = false;
                for (i = 0; i < iTeamCount; i++)
                {
                    if (game[i].i16Points != 0) //assumes nobody scored a 0 in week 16
                    {
                        bSupDone = true;
                        lsAllDataJSON.Add(bSupDone);
                        
                        string sText = (iYearFromVariable + 1).ToString() + " Draft Order:<br />";
                        j = 0;
                        for (i = iTeamCount - 1; i >= 0; i--)
                        {
                            if (standings[i].sOwner != game[0].sOwner)
                            {
                                j++;
                                sText += j.ToString() + ". " + standings[i].sOwner + "<br />";
                            }
                        }
                        sText += (j + 1).ToString() + ". " + game[0].sOwner;
                        
                        //loads the draft order into JSON
                        lsAllDataJSON.Add(sText);
                        //dDraftOrder.InnerHtml = sText;

                        break;
                    }
                }
                //if the superbowl is not done, this will hold false in spot [5]
                //if the superbowl is done, this will go to spot [6] which is never used
                //spot [5] holds both no matter what the outcome is
                lsAllDataJSON.Add(bSupDone);
            }
        }

        private void GetHall()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNodeList xmlNL;
            List<int> years = new List<int>();
            List<string> owners = new List<string>(), teams = new List<string>(), rosters = new List<string>(), addresses = new List<string>(), drafts = new List<string>();

            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\HallOfFame.xml");
            xmlNL = xDoc.GetElementsByTagName("year");

            //for each year
            for (int i = 0; i < xmlNL.Count; i++)
            {
                years.Add(Convert.ToInt32(xmlNL[i].Attributes["id"].Value));
                owners.Add(xmlNL[i].Attributes["owner"].Value);
                teams.Add(xmlNL[i].Attributes["winningTeam"].Value);

                //if the roster is there
                if (xmlNL[i].Attributes["roster"].Value != "")
                {
                    if (years[i] < 2014) //old website
                        rosters.Add("<a href=\"" + oldHFLSiteLocation + xmlNL[i].Attributes["roster"].Value + "\">Roster</a>");
                    else
                        rosters.Add("<a href=\"http://www.dharmarevelation.com/hfl2/" + xmlNL[i].Attributes["roster"].Value + "\">Roster</a>");
                }
                else
                    rosters.Add("");

                //if the address is there
                if (xmlNL[i].Attributes["address"].Value != "")
                {
                    if (years[i] < 2014) //old website
                        addresses.Add("<a href=\"" + oldHFLSiteLocation + xmlNL[i].Attributes["address"].Value + "\">Summary</a>");
                    else
                        addresses.Add("<a href=\"http://www.dharmarevelation.com/hfl2/" + xmlNL[i].Attributes["address"].Value + "\">Summary</a>");
                }
                else
                    addresses.Add("");

                //if the draft is there
                if (xmlNL[i].Attributes["draft"].Value != "")
                {
                    if (years[i] < 2014) //old website
                        drafts.Add("<a href=\"" + oldHFLSiteLocation + xmlNL[i].Attributes["draft"].Value + "\">Draft</a>");
                    else
                        drafts.Add("<a href=\"http://www.dharmarevelation.com/hfl2/" + xmlNL[i].Attributes["draft"].Value + "\">Draft</a>");
                }
                else
                    drafts.Add("");
            }

            //store all the historic data in lsHallOfFame for JSON
            lsHallOfFame.Add(years);
            lsHallOfFame.Add(owners);
            lsHallOfFame.Add(teams);
            lsHallOfFame.Add(rosters);
            lsHallOfFame.Add(addresses);
            lsHallOfFame.Add(drafts);
        }

        private void GetAddData(int iYear)
        {
            int iWeekToGet = 1;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + iYear + ".xml");
            XmlNodeList xmlNL = xDoc.GetElementsByTagName("week"), xmlNLTeams = xDoc.GetElementsByTagName("team");

            //if a week 1 score is 0, it's week 1;  assumes nobody scored a 0 in week 1!
            if (xmlNL.Count == 1 && Convert.ToDouble(xmlNL[0].Attributes[xmlNLTeams[0].Attributes["owner"].Value].Value) == 0)
                iWeekToGet = 1;

            //if a week 1 score isn't 0, it's week 2
            else
                iWeekToGet = xmlNL.Count + 1;

            List<string> lsOwnerNames = new List<string>();
            List<object> lsWeeklyScores = new List<object>();

            //creating list of all the player names to send
            for (int i = 0; i < xmlNLTeams.Count; i++)
                lsOwnerNames.Add(xmlNLTeams[i].Attributes["owner"].Value);

            //getting all the weeks there are to send back
            for (int i = 0; i < iWeekToGet - 1; i++)
            {
                for (int j = 0; j < xmlNL.Count; j++) //find the right id, so the weeks sent back are sorted by id
                    if (xmlNL[j].Attributes["id"].Value == (i + 1).ToString())
                    {
                        List<string> tempOwners = new List<string>();
                        List<double> tempScores = new List<double>();
                        for (int k = 0; k < lsOwnerNames.Count; k++)
                        {
                            tempOwners.Add(lsOwnerNames[k]);
                            tempScores.Add(Convert.ToDouble(xmlNL[i].Attributes[lsOwnerNames[k]].Value));
                        }
                        List<object> tempBoth = new List<object>();
                        tempBoth.Add(tempOwners);
                        tempBoth.Add(tempScores);
                        lsWeeklyScores.Add(tempBoth);
                        break;
                    }
            }

            lsAddData.Add(iWeekToGet);
            lsAddData.Add(lsOwnerNames);
            lsAddData.Add(lsWeeklyScores);
        }

        private void GetYear()
        {
            //lsYear contains: "Add" or "Edit", the year we're working with, the list of teams, and (if "Edit") the yahoo URL

            //we're getting the data here to check if we're adding or editing
            //Add vs. Edit changes on New Year's Day
            DateTime now = DateTime.Now;

            //if the year in Settings.xml is not Now.Year (likely Now.Year - 1), we're adding
            if (currentYear != now.Year)
            {
                lsYear.Add("Add");
                lsYear.Add((currentYear + 1).ToString());
                lsYear.Add("");
            }
            //else if the year in Settings.xml is Now.Year, we're editing
            else if (currentYear == now.Year)
            {
                lsYear.Add("Edit");
                lsYear.Add(currentYear.ToString());
                lsYear.Add(yahooURL);
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + currentYear.ToString() + ".xml");

            XmlNodeList xmlNL = xDoc.GetElementsByTagName("team");
            List<List<string>> lsTeamData = new List<List<string>>();

            if (lsYear[0] == "Add")
            {
                //get just the person's name from the xml file from last year, since team names/yahoo IDs changed
                for (int iTeam = 0; iTeam < xmlNL.Count; iTeam++)
                {
                    List<string> teamData = new List<string>();
                    teamData.Add(xmlNL[iTeam].Attributes["owner"].Value);
                    lsTeamData.Add(teamData);
                }
            }
            else
            {
                //get all the owner data from the xml file for the season being edited
                for (int iTeam = 0; iTeam < xmlNL.Count; iTeam++)
                {
                    List<string> teamData = new List<string>();
                    teamData.Add(xmlNL[iTeam].Attributes["owner"].Value);
                    teamData.Add(xmlNL[iTeam].Attributes["name"].Value);
                    teamData.Add(xmlNL[iTeam].Attributes["yahooID"].Value);
                    lsTeamData.Add(teamData);
                }
            }
            lsYear.Add(lsTeamData);
        }

        //this function gets the year, old site location, and yahoo URL
        //from Settings.xml and stores them in global variables
        private void GetVarsFromSettings()
        {
            XmlDocument settingsDoc = new XmlDocument();
            settingsDoc.Load(Request.PhysicalApplicationPath + "\\xml\\Settings.xml");
            currentYear = Convert.ToInt16(settingsDoc.SelectSingleNode("settings/year").InnerText);
            oldHFLSiteLocation = settingsDoc.SelectSingleNode("settings/OldHFLSiteLocation").InnerText;
            yahooURL = settingsDoc.SelectSingleNode("settings/yahooURL").InnerText;
        }
    }
}