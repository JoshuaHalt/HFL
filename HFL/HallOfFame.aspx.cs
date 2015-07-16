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
    public partial class HallOfFame : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadHall();
        }

        private void LoadHall()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNodeList xmlNL;
            List<int> years = new List<int>();
            List<string> owners = new List<string>(), teams = new List<string>(), rosters = new List<string>(), addresses = new List<string>(), drafts = new List<string>();
            string sLine;

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
                        rosters.Add("<a href=\"" + System.Configuration.ConfigurationManager.AppSettings["Old.HFL.Site.Location"] + xmlNL[i].Attributes["roster"].Value + "\">Roster</a>");
                    else
                        rosters.Add("<a href=\"http://www.dharmarevelation.com/hfl2/" + xmlNL[i].Attributes["roster"].Value + "\">Roster</a>");
                }
                else
                    rosters.Add("");
                
                //if the address is there
                if (xmlNL[i].Attributes["address"].Value != "")
                {
                    if (years[i] < 2014) //old website
                        addresses.Add("<a href=\"" + System.Configuration.ConfigurationManager.AppSettings["Old.HFL.Site.Location"] + xmlNL[i].Attributes["address"].Value + "\">Summary</a>");
                    else
                        addresses.Add("<a href=\"http://www.dharmarevelation.com/hfl2/" + xmlNL[i].Attributes["address"].Value + "\">Summary</a>");
                }
                else
                    addresses.Add("");
                
                //if the draft is there
                if (xmlNL[i].Attributes["draft"].Value != "")
                {
                    if (years[i] < 2014) //old website
                        drafts.Add("<a href=\"" + System.Configuration.ConfigurationManager.AppSettings["Old.HFL.Site.Location"] + xmlNL[i].Attributes["draft"].Value + "\">Draft</a>");
                    else
                        drafts.Add("<a href=\"http://www.dharmarevelation.com/hfl2/" + xmlNL[i].Attributes["draft"].Value + "\">Draft</a>");
                }
                else
                    drafts.Add("");
            }


            HtmlTableRow newRow = new HtmlTableRow();
            HtmlTableCell cellPts = new HtmlTableCell();

            sLine = "";

            //load the data into the table
            for (int i = 0; i < years.Count; i++)
            {
                if (i % 2 == 1)
                    sLine += "<tr class=\"altRow\">";
                else
                    sLine += "<tr class=\"regRow\">";

                sLine += "<td style=\"padding-right: 15px\">" + years[i].ToString() + "</td>";
                sLine += "<td style=\"padding-right: 15px\">" + owners[i] + "</td>";
                sLine += "<td style=\"padding-right: 15px\">" + teams[i] + "</td>";
                sLine += "<td style=\"text-align: center\">" + rosters[i] + "</td>";
                sLine += "<td style=\"text-align: center\">" + addresses[i] + "</td>";
                sLine += "<td style=\"text-align: center\">" + drafts[i] + "</td>";
                sLine += "</tr>";
            }

            cellPts.InnerHtml = sLine;
            newRow.Cells.Add(cellPts);
            tabHall.Rows.Add(newRow);
            
            newRow = null;
            cellPts = null;
        }
    }
}