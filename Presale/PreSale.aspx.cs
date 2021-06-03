using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JJPro.Web.Business.Shopping
{
    public partial class PreSale : JJPro.Web.ConsultantPage
    {
        private OEObject _oeObj = new OEObject();
        private List<string> partyIDList = new List<string>();
        private List<string> specialListPA = new List<string>();

        protected void Page_Init(object sender, EventArgs e)
        {

            CheckoutButton.Click += new EventHandler(CheckoutButton_Click);
            //addIDsToList();
            //specialList();
        }

        void CheckoutButton_Click(object sender, EventArgs e)
        {
            string basic = PresaleItem1TextBox.Value.ToString(); //Kit 1
            string advanced = PresaleItem2TextBox.Value.ToString(); //Kit 2
            string ultimate = PresaleItem3TextBox.Value.ToString(); //Kit 3
            string catalog = PresaleItem4TextBox.Value.ToString(); //Catalogs
            string sets1 = PresaleItem5TextBox.Value.ToString(); // Hostess 1 
            string sets2 = PresaleItem6TextBox.Value.ToString(); // Hostess 2

            Response.Redirect(string.Format("PresaleOrder.aspx?param1={0}&param2={1}&param3={2}&param4={3}&param5={4}&param6={5}", basic, advanced, ultimate, catalog, sets1, sets2));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string countryCode = this.CurrentConsultant.CountryCode;

            CountryHiddenField.Value = countryCode;

            DateTime endDate = Convert.ToDateTime("4/05/2019 2:59AM");

            shipDateSpan.InnerHtml = "*Lookbook ordering ends 04/04/19 at 11:59 pm EST.";

            if (DateTime.Now >= endDate)
            {
                PresaleAvailablePanel.Visible = false;
                PresaleEndedPanel.Visible = true;
            }

            if (!IsPostBack)
            {
                Core.Metadata md = new Core.Metadata();
                string orderName = md.GetSystemString("PRESALEORDERNAME");
                _oeObj.DeleteOrder(this.CurrentConsultant.ConsultantsGuid, orderName);

                PresaleItem1TextBox.Value = 0;
                PresaleItem2TextBox.Value = 0;
                PresaleItem3TextBox.Value = 0;
                PresaleItem4TextBox.Value = 0;
                PresaleItem5TextBox.Value = 0;
                PresaleItem6TextBox.Value = 0;

            }

            foreach (string consultantID in specialListPA)
            {
                if (this.CurrentConsultant.ConsultantId == consultantID)
                {
                    Session.Add("SpecialTaxItem", true);
                }
            }

            //foreach (string consultantID in partyIDList)
            //{
            //    if (this.CurrentConsultant.ConsultantId == consultantID)
            //    {
            //        //itemstop.Style.Add("float", "left!important");
            //        //itemstop.Style.Add("margin-left", "auto!important");
            //        //itemstop.Style.Add("margin-right", "auto!important");
            //        //itemstop.Style.Add("width", "100%!important");

            //        //Kit1.Attributes.Remove("col-md-6");
            //        //Kit1.Attributes.Add("class", "col-md-4 col-xs-12 presaleItemDiv");
            //        //Kit2.Attributes.Remove("col-md-6");
            //        //Kit2.Attributes.Add("class", "col-md-6 col-xs-12 presaleItemDiv");
            //        Kit2.Style.Add("display", "none");
            //        Catalog2Pack.Style.Add("display", "none");
            //        jewelryBoutiqueKit.Style.Add("display", "none");
            //        Kit3.Style.Add("display", "inline-block");
            //        vipOnlyTotal.Style.Add("display", "table");
            //        catalog2PackTotal.Style.Add("display", "none");
            //        DoublePointsImage.Visible = false;

            //        shipDateSpan.InnerHtml = "*Pre-sale items ship no later than Friday, Februrary 16th. <br /> **Free Shipping not included on catalog case.";


            //        //*** ***** *** Special Case for 2018 Spring Summer Leaders Used on AddressOnlyCheckoutPage for zero dollar shipping**** *** ***//
            //        Session.Add("LEADERHOSTESS", true);

            //        return;
            //    }
            //    else
            //    {
            //        Kit3.Style.Add("display", "none");
            //        Catalog2Pack.Style.Add("display", "inline-block");
            //        vipOnlyTotal.Style.Add("display", "none");
            //        Kit2.Style.Add("display", "inline-block");
            //        jewelryBoutiqueKit.Style.Add("display", "table");
            //        catalog2PackTotal.Style.Add("display", "table");
            //        shipDateSpan.InnerHtml = "*Pre-sale items ship no later than Friday, Februrary 23rd.";
            //    //{
            //    //    itemstop.Style.Add("float", "none!important");
            //    //    itemstop.Style.Add("margin-left", "auto!important");
            //    //    itemstop.Style.Add("margin-right", "auto!important");
            //    //    itemstop.Style.Add("width", "80%!important");

            //    //    Kit1.Attributes.Remove("col-md-4");
            //    //    Kit1.Attributes.Add("class", "col-md-6 col-xs-12 presaleItemDiv");
            //    //    Kit2.Attributes.Remove("col-md-4");
            //    //    Kit2.Attributes.Add("class", "col-md-6 col-xs-12 presaleItemDiv");
            //    //    Kit3.Style.Add("display", "none");
            //    }

            //}


            

            //if (countryCode == "US")
            //{
            //    PresaleItem4PriceLabel.Text = "99.00US";
            //}

            //if (countryCode == "CA")
            //{
            //    PresaleItem4PriceLabel.Text = "99.00CA";
            //}
        }

        //protected void specialList()
        //{
        //    specialListPA.Add("30377");
        //}

        //protected void addIDsToList()
        //{
        //    partyIDList.Add("14433");
        //    partyIDList.Add("40716");
        //    partyIDList.Add("40658");
        //    partyIDList.Add("40649");
        //    partyIDList.Add("30377");
        //    partyIDList.Add("40653");
        //    partyIDList.Add("20296");
        //    //partyIDList.Add("38350");
        //    partyIDList.Add("38529");
        //    partyIDList.Add("38530");
        //    partyIDList.Add("38526");
        //    partyIDList.Add("38525");

        //}


    }
}