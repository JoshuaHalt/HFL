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
    public partial class AddSuperbowl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;
            LoadData();
        }

        //load the info from the xml file into the textboxes
        private void LoadData()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");
            XmlNodeList xmlNLTeams = xDoc.GetElementsByTagName("team");
            XmlNode week15 = xDoc.SelectSingleNode("hfl/superbowl/week15"), week16 = xDoc.SelectSingleNode("hfl/superbowl/week16");

            for (int i = 0; i < xmlNLTeams.Count; i++)
            {
                tabSuperbowl.Rows[i + 1].Cells[0].InnerText = xmlNLTeams[i].Attributes["owner"].Value; //owner name
                tabSuperbowl.Rows[i + 1].Cells[1].Controls[0].Visible = true; //show week 15 textbox
                HtmlInputText tempText15 = (HtmlInputText)tabSuperbowl.Rows[i + 1].Cells[1].Controls[0]; //get the textbox
                tempText15.Value = week15.Attributes[xmlNLTeams[i].Attributes["owner"].Value].Value; //put in the value
                
                tabSuperbowl.Rows[i + 1].Cells[3].Controls[0].Visible = true; //show week 16 textbox
                HtmlInputText tempText16 = (HtmlInputText)tabSuperbowl.Rows[i + 1].Cells[3].Controls[0]; //get the textbox
                tempText16.Value = week16.Attributes[xmlNLTeams[i].Attributes["owner"].Value].Value; //put in the value
            }
        }

        protected void supSubmit_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");
            XmlNodeList xmlTeams = xDoc.GetElementsByTagName("team");
            XmlNode supNode = xDoc.SelectSingleNode("hfl/superbowl"),
                    week15 = xDoc.SelectSingleNode("hfl/superbowl/week15"),
                    week16 = xDoc.SelectSingleNode("hfl/superbowl/week16");
            int testInt;

            //show superbowl if show is selected
            if (rbs.Items[0].Selected)
                supNode.Attributes["show"].Value = "true";
            else if (rbs.Items[1].Selected)
                supNode.Attributes["show"].Value = "false";
            
            //put the data from the textboxes into the xml file
            for (int i = 0; i < xmlTeams.Count; i++)
            {
                HtmlInputText tempText = (HtmlInputText)tabSuperbowl.Rows[i + 1].Cells[1].Controls[0];
                tabSuperbowl.Rows[i + 1].Cells[2].InnerText = "";

                if (int.TryParse(tempText.Value, out testInt))
                    week15.Attributes[xmlTeams[i].Attributes["owner"].Value].Value = tempText.Value;
                else
                {
                    tabSuperbowl.Rows[i + 1].Cells[2].InnerText = "*error: wrong input format";
                    return;
                }

                tempText = (HtmlInputText)tabSuperbowl.Rows[i + 1].Cells[3].Controls[0];
                tabSuperbowl.Rows[i + 1].Cells[4].InnerText = "";

                if (int.TryParse(tempText.Value, out testInt))
                    week16.Attributes[xmlTeams[i].Attributes["owner"].Value].Value = tempText.Value;
                else
                {
                    tabSuperbowl.Rows[i + 1].Cells[4].InnerText = "*error: wrong input format";
                    return;
                }
            }

            xDoc.Save(Request.PhysicalApplicationPath + "\\xml\\" + System.Configuration.ConfigurationManager.AppSettings["Default.Year"] + ".xml");

            dSuccess.InnerHtml = "Scores changed successfully. <a href=\"HFL.aspx\">View updated site</a>";
        }
    }
}