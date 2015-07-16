using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI.HtmlControls;

namespace HFL
{
    public partial class AddWeek : System.Web.UI.Page
    {
        List<HtmlInputText> tBoxes = new List<HtmlInputText>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            LoadWeekDropdown();
        }

        //loads the dropdown and predicts the current week
        private void LoadWeekDropdown()
        {
            selWeek.Items.Clear();
            selWeek.Items.Add("");
            for (int i = 1; i <= 14; i++)
                selWeek.Items.Add(i.ToString());
            selWeek.SelectedIndex = 0;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");
            XmlNodeList xmlNL = xDoc.GetElementsByTagName("week"), xmlNLTeams = xDoc.GetElementsByTagName("team");

            if (xmlNL.Count == 1 && Convert.ToInt32(xmlNL[0].Attributes[xmlNLTeams[0].Attributes["owner"].Value].Value) == 0) //if a week 1 score is 0, it's week 1
                dPredWeek.InnerText = "Select a week to edit (should be week 1): ";
            else if (xmlNL.Count == 1 && Convert.ToInt32(xmlNL[0].Attributes[xmlNLTeams[0].Attributes["owner"].Value].Value) != 0) //if a week 1 score isn't 0, it's week 2
                dPredWeek.InnerText = "Select a week to edit (should be week 2): ";
            else if (xmlNL.Count == 14) //if there's 14 weeks, it's the superbowl
                dPredWeek.InnerText = "Select a week to edit (should be in the superbowl): ";
            else //otherwise it's week (week count + 1)
                dPredWeek.InnerText = "Select a week to edit (should be week " + (xmlNL.Count + 1).ToString() + "): ";
        }

        //when a week is chosen the owner names are inserted and textboxes with their scores that week (if it's there) are made visible
        protected void selWeek_Change(object sender, EventArgs e)
        {
            XmlDocument xDoc = new XmlDocument();
            int iTeamCount;

            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");
            XmlNodeList xmlNLTeams = xDoc.GetElementsByTagName("team"), xmlNLWeeks = xDoc.GetElementsByTagName("week");
            iTeamCount = xmlNLTeams.Count;

            //load the owner names
            for (int i = 0; i < xmlNLTeams.Count; i++)
            {
                HtmlInputText tempText = (HtmlInputText)tabTeams.Rows[i].Cells[1].Controls[0];
                tempText.Visible = true;
                tabTeams.Rows[i].Cells[0].InnerText = xmlNLTeams[i].Attributes["owner"].Value + ": ";
            }

            //load scores into textboxes
            for (int i = 0; i < xmlNLWeeks.Count; i++)
                if (xmlNLWeeks[i].Attributes["id"].Value == selWeek.SelectedValue)
                    for (int j = 0; j < iTeamCount; j++)
                    {
                        HtmlInputText tempText = (HtmlInputText)tabTeams.Rows[j].Cells[1].Controls[0];
                        tempText.Value = xmlNLWeeks[i].Attributes[xmlNLTeams[j].Attributes["owner"].Value].Value;
                        tempText.Visible = true;
                    }
            btnSubmit.Visible = true;
        }

        protected void weekSubmit_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");
            XmlNodeList xmlNLTeams = xDoc.GetElementsByTagName("team"), xmlNLWeeks = xDoc.GetElementsByTagName("week");
            XmlNode parentNode = xDoc.SelectSingleNode("hfl/weeks"), childNode = parentNode.ChildNodes[0], newNode = childNode.Clone();
            int testInt; //for data testing

            //put textbox data into xml
            newNode.Attributes["id"].Value = selWeek.SelectedValue;
            for (int i = 0; i < xmlNLTeams.Count; i++)
            {
                HtmlInputText currentText = (HtmlInputText)tabTeams.Rows[i].Cells[1].Controls[0];
                tabTeams.Rows[i].Cells[2].InnerText = "";

                if (int.TryParse(currentText.Value, out testInt))
                    newNode.Attributes[xmlNLTeams[i].Attributes["owner"].Value].Value = currentText.Value;
                else
                {
                    tabTeams.Rows[i].Cells[2].InnerText = "*error: wrong input format";
                    return;
                }
            }

            //if the week already exists, overwrite it by deleting the old one first
            for (int i = 0; i < xmlNLWeeks.Count; i++)
                if (xmlNLWeeks[i].Attributes["id"].Value == selWeek.SelectedValue)
                {
                    parentNode.RemoveChild(parentNode.ChildNodes[i]);
                    break;
                }

            //add the new node and save the xml file
            parentNode.AppendChild(newNode);
            xDoc.Save(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");

            dSuccess.InnerHtml = "Scores changed successfully. <a href=\"HFL.aspx\">View updated site</a>";
        }
    }
}