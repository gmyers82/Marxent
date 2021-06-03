using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace JJPro.Web.Business.NationalConference //JJPro.Web.ConsultantPage
{
    public partial class FashionWeekend : JJPro.Web.ConsultantPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            PurchaseTicketButton.Click += new EventHandler(PurchaseTicketButton_Click);
        }



        protected void Page_Load(object sender, EventArgs e)
        {

            PurchaseQuantityDDL.Items[0].Attributes.Add("class", "dropdown-item");
            PurchaseQuantityDDL.Items[1].Attributes.Add("class", "dropdown-item");
            PurchaseQuantityDDL.Items[2].Attributes.Add("class", "dropdown-item");
            PurchaseQuantityDDL.Items[3].Attributes.Add("class", "dropdown-item");
            PurchaseQuantityDDL.Items[4].Attributes.Add("class", "dropdown-item");

            if (DateTime.Now >= Convert.ToDateTime("06-01-2018"))
            {
                TicketPriceLabel.Text = "- $179";
            }
            else
            {
                TicketPriceLabel.Text = "- $149";

            }

            if (DateTime.Now > Convert.ToDateTime("07-01-2018"))
            {
                TicketPriceLabel.Text = "- No Longer On Sale";
                PurchasePanelDiv.Style.Add("display", "none");
                
            }


        }

        void PurchaseTicketButton_Click(object sender, EventArgs e)
        {
            string productGuid = "48EC4927-CC02-41A9-A7F9-C450163F020B";

            Session["OrderRemarks"] = null;
            if (VegetarianMealCheckBox.Checked)
            {
                Session["OrderRemarks"] = "Vegetarian Option";
            }
            else
            {
                Session.Remove("OrderRemarks");
            }

                OEObject oe = new OEObject();
                int quantity = Convert.ToInt16(PurchaseQuantityDDL.SelectedValue);

                int i = 1;
                Guid newOrder = oe.CreateNewOrder(this.CurrentConsultant.ConsultantsGuid, "2018 Fashion Weekend");
                while (newOrder == Guid.Empty)
                {
                    newOrder = oe.CreateNewOrder(this.CurrentConsultant.ConsultantsGuid, "2018 Fashion Weekend" + i);
                    i++;
                }
                CurrentOrderGuid = newOrder.ToString();
                oe.AddToBag(newOrder, new Guid(productGuid), quantity);
                Response.Redirect("~/Business/Shopping/ConsultantCheckout.aspx");
                Response.End();
        }

        
    }
}