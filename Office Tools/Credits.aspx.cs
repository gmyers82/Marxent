using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using Core;

namespace JJPro.OfficeTools.Admin
{
    public partial class Credits : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            SubmitTextBoxButton.Click += new EventHandler(SubmitTextBoxButton_Click);
            AddCreditButton.Click += new EventHandler(AddCreditButton_Click);
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        void AddCreditButton_Click(object sender, EventArgs e)
        {
            Provider.Consultants cons = new Provider.Consultants();
            Provider.Credits credits = new Provider.Credits();
            GeneralLists gl = new GeneralLists();
            credits.Connect();
            cons.Connect();
            string consultantID = AddConsultantID.Text.Trim();
            Guid consultantGuid = cons.GetConsultantsGuidFromId(consultantID);
            string dateYear = DateTime.Now.Year.ToString();
            string dateDay = DateTime.Now.Day.ToString();
            string dateMonth = DateTime.Now.Month.ToString();
            string dateHourMinuteSecond = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            string creditsId = dateYear + "_" + dateDay + "_" + dateMonth + "_" + dateHourMinuteSecond;
            DataTable dt = gl.CommonList("CreditTypes");
            string creditTypeValue = CreditTypeRadioButtonList.SelectedValue;
            DataRow[] creditTypeGuidSelect = dt.Select("Value = '"+creditTypeValue+"'");
            Guid creditsTypeGuid = Guid.Empty;
            foreach(DataRow creditType in creditTypeGuidSelect)
            {
                creditsTypeGuid = new Guid(creditType["Key"].ToString());
            }

            credits.CreditsId = creditsId;
            credits.ConsultantsGuid = consultantGuid;
            credits.CustomersGuid = Guid.Empty;
            credits.OrdersGuid = Guid.Empty;
            credits.CreditTypesGuid = creditsTypeGuid;
            credits.Amount = Convert.ToDecimal(AddAmount.Text);
            credits.StartDate = Convert.ToDateTime(AddStartDate.Text);
            credits.EndDate = Convert.ToDateTime(AddEndDate.Text);
            credits.DateApproved = DateTime.Now;
            credits.IsApproved = true;
            credits.ApprovedBy = "CreditTool";

            switch(creditTypeValue)
            {
                case "Return":

                    credits.NoVolumes = true;
            credits.CanApplyToShipping = false;
            credits.CanApplyToProduct = false;
            credits.CanApplyToQVProduct = false;
            credits.CanApplyToSupply = false;
            credits.CanBeCash = false;
                    break;

                case "Incentive":
                    credits.NoVolumes = true;
            credits.CanApplyToShipping = false;
            credits.CanApplyToProduct = false;
            credits.CanApplyToQVProduct = false;
            credits.CanApplyToSupply = false;
            credits.CanBeCash = false;
                    break;

                case "Commission":
             credits.NoVolumes = false;
            credits.CanApplyToShipping = false;
            credits.CanApplyToProduct = true;
            credits.CanApplyToQVProduct = false;
            credits.CanApplyToSupply = false;
            credits.CanBeCash = false;
                    break;

                case "PURCHASED":
                    credits.NoVolumes = false;
            credits.CanApplyToShipping = false;
            credits.CanApplyToProduct = false;
            credits.CanApplyToQVProduct = true;
            credits.CanApplyToSupply = false;
            credits.CanBeCash = false;
                    break;
                    
            }


            
            credits.Remarks = AddRemarks.Text;

            credits.CreateCredit();

            getCreditHistory();


        }


        void SubmitTextBoxButton_Click(object sender, EventArgs e)
        {
            getCreditHistory();
        }

        protected void getCreditHistory()
        {
            Provider.Credits credits = new Provider.Credits();
            Provider.Consultants cons = new Provider.Consultants();
            string consultantID = consultantIDTextBox.Text.Trim();
            credits.Connect();
            cons.Connect();
            cons.Initialize(cons.GetConsultantsGuidFromId(consultantID));
            ConsultantName.Text = cons.ReportName;
            ConsultantIDGuid.Text = "ID: " + cons.ConsultantId + " ConsultantGuid: " + cons.ConsultantsGuid.ToString();
            DataTable dt = credits.Initialize(Provider.Credits.Selection.ByConsultantId, consultantID, "");
            DataView dv = dt.DefaultView;
            dv.Sort = "StartDate desc";
            DataTable dt2 = dv.ToTable();

            foreach (DataRow row in dt2.Rows)
            {

                HtmlGenericControl row1 = new HtmlGenericControl("div");
                HtmlGenericControl div1 = new HtmlGenericControl("div");
                HtmlGenericControl div2 = new HtmlGenericControl("div");
                HtmlGenericControl div3 = new HtmlGenericControl("div");
                HtmlGenericControl div4 = new HtmlGenericControl("div");
                HtmlGenericControl div5 = new HtmlGenericControl("div");
                HtmlGenericControl div6 = new HtmlGenericControl("div");
                HtmlGenericControl div7 = new HtmlGenericControl("div");

                row1.Attributes.Add("class", "row custom-div");
                div1.Attributes.Add("class", "col-md-2");
                div2.Attributes.Add("class", "col-md-2");
                div3.Attributes.Add("class", "col-md-2");
                div4.Attributes.Add("class", "col-md-2");
                div5.Attributes.Add("class", "col-md-2");
                div6.Attributes.Add("class", "col-md-2");
                div7.Attributes.Add("class", "col-md-12");

                HtmlGenericControl CreditID = new HtmlGenericControl("span");
                HtmlGenericControl CreditType = new HtmlGenericControl("span");
                HtmlGenericControl OrderNumber = new HtmlGenericControl("span");
                HtmlGenericControl Amount = new HtmlGenericControl("span");
                HtmlGenericControl StartDate = new HtmlGenericControl("span");
                HtmlGenericControl EndDate = new HtmlGenericControl("span");
                HtmlGenericControl Remarks = new HtmlGenericControl("span");

                CreditID.InnerHtml = "<p><b>CreditsID</b> <br/>" + "<span class='credit-data'>" + row["CreditsId"].ToString() + "</span></p>";
                div1.Controls.Add(CreditID);
                CreditType.InnerHtml = "<p><b>CreditType</b> <br/>" + "<span class='credit-data'>" + row["CreditType"].ToString() + "</span></p>";
                div2.Controls.Add(CreditType);
                OrderNumber.InnerHtml = "<p><b>OrderNumber</b> <br/>" + "<span class='credit-data'>" + row["OrderNumber"].ToString() + "</span></p>";
                div3.Controls.Add(OrderNumber);
                Amount.InnerHtml = "<p><b>Amount</b> <br/>" + "<span class='credit-data'>" + row["Amount"].ToString() + "</span></p>";
                div4.Controls.Add(Amount);
                StartDate.InnerHtml = "<p><b>StartDate</b> <br/>" + "<span class='credit-data'>" + row["StartDateRaw"].ToString() + "</span></p>";
                div5.Controls.Add(StartDate);
                EndDate.InnerHtml = "<p><b>EndDate</b> <br/>" + "<span class='credit-data'>" + row["EndDateRaw"].ToString() + "</span></p>";
                div6.Controls.Add(EndDate);
                Remarks.InnerHtml = "<p><b>Remarks</b> <br/>" + "<span class='credit-data'>" + row["Remarks"].ToString() + "</span></p>";
                div7.Controls.Add(Remarks);

                row1.Controls.Add(div1);
                row1.Controls.Add(div2);
                row1.Controls.Add(div3);
                row1.Controls.Add(div4);
                row1.Controls.Add(div5);
                row1.Controls.Add(div6);
                row1.Controls.Add(div7);

                CreditsPlaceHolder.Controls.Add(row1);

            }
            credits.Disconnect();
        }

    }
}