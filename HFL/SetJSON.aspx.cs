using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;

namespace HFL
{
    [ScriptService]
    public partial class SetJSON : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string SetAddData(object allData)
        {
            string deserial = HttpUtility.UrlDecode(allData.ToString()); //get a string from the object passed in and make the characters normal

            List<string> newData = deserial.Split('|').ToList<string>(); //split the data by "|"
            newData.RemoveAt(newData.Count - 1); //remove the last element, which is ""

            return SaveData(newData); //return the result of calling this method
        }

        private static string SaveData(List<string> dataToSave)
        {
            //try... cause so much can go wrong
            try
            {
                string iYear = dataToSave[0], iWeek = dataToSave[1]; //get the year and week
                dataToSave.RemoveRange(0, 2); //remove year and week so just the people/scores are left

                //open the xml file for the passed in year, get the teams, weeks, the "weeks" node, the first node, and a clone of the first node
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\" + iYear + ".xml");
                XmlNodeList xmlNLTeams = xDoc.GetElementsByTagName("team"), xmlNLWeeks = xDoc.GetElementsByTagName("week");
                XmlNode parentNode = xDoc.SelectSingleNode("hfl/weeks"), childNode = parentNode.ChildNodes[0], newNode = childNode.Clone();

                //add the week as the id of the new node
                newNode.Attributes["id"].Value = iWeek;

                //get the name and score from each element in the list, edit the attribute of name with the score value
                for (int i = 0; i < dataToSave.Count; i++)
                {
                    string[] temp = dataToSave[i].Split(' ');
                    newNode.Attributes[temp[0]].Value = temp[1];
                }

                //if the week already exists, overwrite it by deleting the old one first
                for (int i = 0; i < xmlNLWeeks.Count; i++)
                    if (xmlNLWeeks[i].Attributes["id"].Value == iWeek)
                    {
                        parentNode.RemoveChild(parentNode.ChildNodes[i]);
                        break;
                    }

                //add the new node and save the xml file
                parentNode.AppendChild(newNode);
                xDoc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\" + iYear + ".xml");
                return "Success";
            }
            //otherwise, return the error message if something went wrong *sigh*
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod]
        public static string SetAddSeasonData(object allData)
        {
            string deserial = HttpUtility.UrlDecode(allData.ToString()); //get a string from the object passed in and make the characters normal

            List<string> newData = deserial.Split('|').ToList<string>(); //split the data by "|"
            newData.RemoveAt(newData.Count - 1); //remove the last element, which is ""

            return CreateNewSeason(newData);
        }

        private static string CreateNewSeason(List<string> newSeasonData)
        {
            //try... cause so much can go wrong
            try
            {
                int iYear = Convert.ToInt16(newSeasonData[0]);
                string yahooURL = newSeasonData[1];
                newSeasonData.RemoveRange(0, 2);

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\Settings.xml");
                xDoc.SelectSingleNode("settings/year").InnerText = iYear.ToString();
                xDoc.SelectSingleNode("settings/yahooURL").InnerText = yahooURL;
                xDoc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\Settings.xml");                

                System.IO.File.Copy(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\" + (iYear - 1) + ".xml", HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\" + iYear + ".xml");

                xDoc.Load(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\" + iYear + ".xml");

                XmlNode parentTeamNode = xDoc.SelectSingleNode("hfl/teams"), parentWeekNode = xDoc.SelectSingleNode("hfl/weeks"),
                        childTeamNode = parentTeamNode.ChildNodes[0].Clone(), childWeekNode = parentWeekNode.ChildNodes[0].Clone();
                childTeamNode.RemoveAll();
                childWeekNode.RemoveAll();

                XmlAttribute weekID = xDoc.CreateAttribute("id");
                weekID.Value = "1";
                childWeekNode.Attributes.Append(weekID);

                parentTeamNode.RemoveAll();
                parentWeekNode.RemoveAll();

                for (int i = 0; i < newSeasonData.Count; i++)
                {
                    List<string> team = newSeasonData[i].Split('^').ToList<string>();
                    XmlNode tempTeamNode = childTeamNode.Clone();
                    XmlAttribute id = xDoc.CreateAttribute("id"), name = xDoc.CreateAttribute("name"),
                        owner = xDoc.CreateAttribute("owner"), yahooID = xDoc.CreateAttribute("yahooID"),
                        weekScore = xDoc.CreateAttribute(team[0]);
                    
                    id.Value = (i + 1).ToString();
                    name.Value = team[1];
                    owner.Value = team[0];
                    yahooID.Value = team[2];
                    weekScore.Value = "0";

                    tempTeamNode.Attributes.Append(id);
                    tempTeamNode.Attributes.Append(name);
                    tempTeamNode.Attributes.Append(owner);
                    tempTeamNode.Attributes.Append(yahooID);
                    childWeekNode.Attributes.Append(weekScore);

                    parentTeamNode.AppendChild(tempTeamNode);
                }

                parentWeekNode.AppendChild(childWeekNode);

                xDoc.Save(HttpContext.Current.Request.PhysicalApplicationPath + "\\xml\\" + iYear + ".xml");
                
                return "Success";
            }

            //catch an exception if there is one
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}