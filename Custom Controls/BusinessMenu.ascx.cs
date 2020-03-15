using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Core;
using System.Web.UI.HtmlControls;
using System.Data;

namespace JJPro.Web.Business.Controls.ResponsiveControls
{
    public partial class BusinessMenu : System.Web.UI.UserControl
    {
        OEObject _oeObj = new OEObject();
        private Core.Authentication auth = new Core.Authentication();

        protected void Page_Init(object sender, EventArgs e)
        {
            dg.DataSource = _oeObj.FetchBizMenu();
            dg.DataBind();

            SetBizMen();
            LandingLogOutButton.Click += new EventHandler(LandingLogOutButton_Click);
            LandingCartButton.Click += LandingCartButton_Click;
        }

        private void LandingCartButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Business/Shopping/Cart.aspx");
            Response.End();
        }

        void LandingLogOutButton_Click(object sender, EventArgs e)
        {
            Session.Remove("Global.Authenticated");
            Session.Remove("Global.UserName");
            Session.Remove("Global.Sponsor");
            auth.SetRole();
            auth.LogLogoutEvent();
            Response.Redirect("/Biz/Default.aspx");
            Response.End(); 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadConsultantInfo();
            SetHeaderImage();
        }

        protected void SetBizMen()
        {
            DataTable dt = _oeObj.FetchBizMenu();

            foreach (DataRow dr in dt.Rows)
            {
                string parentGuid = dr["Key"].ToString();
                string parentMenuGuid = dr["ParentMenuGuid"].ToString();
                string parentCategoryLink = dr["LinkURL"].ToString();

                if (parentMenuGuid == String.Empty)
                {
                HtmlGenericControl liDropdown = new HtmlGenericControl("li");
                HtmlGenericControl anchorDropDown = new HtmlGenericControl("a");


                liDropdown.Controls.Add(anchorDropDown);


                liDropdown.Attributes.Add("class", "dropdown");


                
                anchorDropDown.InnerHtml = dr["Value"].ToString();

                if (parentCategoryLink != String.Empty)
                {
                    anchorDropDown.Attributes.Add("href", parentCategoryLink);
                }
                else
                {
                    anchorDropDown.Attributes.Add("class", "dropdown-toggle");
                    anchorDropDown.Attributes.Add("data-toggle", "dropdown");
                    anchorDropDown.Attributes.Add("aria-haspopup", "true");
                    anchorDropDown.Attributes.Add("aria-expanded", "false");
                    anchorDropDown.Attributes.Add("href", "#");
                }


                bizDropDown.Controls.Add(liDropdown);
                HtmlGenericControl divDropdownCategory = new HtmlGenericControl("div");
                HtmlGenericControl ulsection1 = new HtmlGenericControl("div");
                HtmlGenericControl ulDropdownCategory = new HtmlGenericControl("ul");
                ulsection1.Style.Add("float", "left");
                


                    foreach (DataRow dr2 in dt.Rows)
                    {
                        string childParentMenuGuid = dr2["ParentMenuGuid"].ToString();

                        if (childParentMenuGuid != String.Empty)
                        {
                            if (parentGuid == childParentMenuGuid)
                            {
                                string categoryName = dr2["Value"].ToString();
                                string categoryLink = dr2["LinkURL"].ToString();

                                
                                HtmlGenericControl liDropdownCategory = new HtmlGenericControl("li");

                                HtmlGenericControl hardCodedChildAcnhor = new HtmlGenericControl("a");
                                liDropdown.Controls.Add(divDropdownCategory);
                                divDropdownCategory.Controls.Add(ulsection1);
                                ulsection1.Controls.Add(ulDropdownCategory);
                                ulDropdownCategory.Controls.Add(liDropdownCategory);
                                liDropdownCategory.Controls.Add(hardCodedChildAcnhor);

                                divDropdownCategory.Attributes.Add("class", "dropdown-menu");


                                hardCodedChildAcnhor.Attributes.Add("href", categoryLink);
                                hardCodedChildAcnhor.InnerHtml = categoryName;
                            }
                        }


                    }
                    HtmlGenericControl ulsection2 = new HtmlGenericControl("div");
                    HtmlGenericControl ulDropdownCategory2 = new HtmlGenericControl("ul");
                    HtmlGenericControl liDropdownCategory2 = new HtmlGenericControl("li");
                    HtmlGenericControl liMegaAnchor = new HtmlGenericControl("a");
                    HtmlGenericControl liImg = new HtmlGenericControl("img");
                    HtmlGenericControl liSpan = new HtmlGenericControl("span");

                    ulsection2.Style.Add("float", "left");
                    liImg.Attributes.Add("src", "https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Landing/Quick%20Links/2017_FW_biz_portal_quick_links_weekly_updates.jpg");
                    liImg.Attributes.Add("width", "150px");
                    liSpan.InnerHtml = "Test Mega Link";
                    //liDropdown.Controls.Add(ulDropdownCategory2);
                    //divDropdownCategory.Controls.Add(ulsection2);
                    ulsection2.Controls.Add(ulDropdownCategory2);
                    ulDropdownCategory2.Controls.Add(liDropdownCategory2);
                    liDropdownCategory2.Controls.Add(liMegaAnchor);
                    liMegaAnchor.Controls.Add(liImg);
                    liMegaAnchor.Controls.Add(liSpan);
                }   

            }

                

        }

        protected void LoadConsultantInfo()
        {
            try
            {
                JJPro.Web.Page page = (JJPro.Web.Page)this.Page;
                HeaderConsultantName.Text = page.CurrentConsultant.FirstName + " " + page.CurrentConsultant.LastName;
                HeaderConsultantTitle.Text = page.CurrentConsultant.LeadershipLevel;
                HeaderConsultantID.Text = "Consultant ID:" + " " + page.CurrentConsultant.ConsultantId;
                pnlConsData.Visible = true;
            }
            catch (Exception ex)
            {
                HeaderConsultantName.Text = "";
                HeaderConsultantID.Text = "";
                pnlConsData.Visible = false;
            }
        }

        protected void SetHeaderImage()
        {

            DataTable dt = _oeObj.FetchAdvertisement("BIZLOGOHEADER");
            string headerImageUrl = "";
            string clickedImageUrl = "";
            string tooltipImageUrl = "";
            string productNumber = "";
            string redirectUrl = "";
            bool isAddToCart = false;

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                headerImageUrl = dr["AdImageUrl"].ToString();
                clickedImageUrl = dr["ClickedImageUrl"].ToString();
                tooltipImageUrl = dr["TooltipImageUrl"].ToString();
                productNumber = dr["ProductNumber"].ToString();
                redirectUrl = dr["RedirectUrl"].ToString();

                isAddToCart = Convert.ToBoolean(dr["IsAddToCart"]);
            }

            if (dt.Rows.Count == 0)
            {
                LogoImage.ImageUrl = "http://8f2270e13c3e0703baa2-6c2aaf386ecfc7423d48bccd1a997322.r2.cf1.rackcdn.com/Logos/JJ-Purple-cropped.png";
            }

            else
            {
                LogoImage.ImageUrl = headerImageUrl;
            }
        }
    }
}