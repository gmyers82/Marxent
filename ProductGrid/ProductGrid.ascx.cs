using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using System.Timers;
using Core;

namespace JJPro.Web.Controls.ResponsiveControls
{
    public partial class ProductGrid : System.Web.UI.UserControl
    {
        OECustObject _oeCustObj = new OECustObject();
        OEObject _oeConsObj = new OEObject();



        private String _menuType;
        private Guid _orderGuid;

        public String MenuType
        {
            get
            {
                return _menuType;
            }

            set
            {
                _menuType = value;

            }
        }

        public Guid OrderGuid
        {
            get
            {
                return _orderGuid;
            }

            set
            {

                _orderGuid = value;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            GetProductData();

        }



        protected void GetProductData()
        {
            
            string qs = Request.QueryString["Menu"];
            string search = Request.QueryString["Search"];
            string menuGuid = Request.QueryString["Guid"];
            DataTable dt = null;

            if (MenuType == "Customer")
            {


                if (Request.QueryString.Count == 0)
                {   //Default to new and featured
                    Resources res = new Resources();
                    DataTable resDT = res.ResourceList("CUSTOMERDEFAULTMENU");
                    string defaultMenu = resDT.Rows[0]["ContentUrl"].ToString();
                    dt = _oeCustObj.FetchProductsByMenu(new Guid(defaultMenu));

                    dt.DefaultView.Sort = "SortOrder desc";
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        CategoryNameLabel.Text = dr["MenuFullName"].ToString();
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No items to display";
                    }
                }

                if (qs != null)
                {
                    dt = _oeCustObj.FetchProductsByMenu(new Guid(qs));
                    dt.DefaultView.Sort = "SortOrder desc";
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        CategoryNameLabel.Text = dr["MenuFullName"].ToString();
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No items to display";
                    }

                }

                if (menuGuid != null)
                {
                    Guid mpptGuid = _oeCustObj.FetchMenuGuidFromMPTT(new Guid(menuGuid));
                    if (mpptGuid != Guid.Empty)
                    {
                        dt = _oeCustObj.FetchProductsByMenu(mpptGuid);
                        dt.DefaultView.Sort = "ProductNum desc";
                        dt = dt.DefaultView.ToTable();
                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            CategoryNameLabel.Text = dr["MenuFullName"].ToString();
                        }
                        else
                        {
                            CategoryNameLabel.Text = "No items to display";
                        }
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No items to display";
                    }

                }

                if (search != null)
                {
                    dt = this._oeCustObj.FetchProductData(this._oeCustObj.FetchSearchList(search));
                    dt.DefaultView.Sort = "ProductNum desc";
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        CategoryNameLabel.Text = "Search results: " + search;
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No results found.";
                    }

                }

            }

            if (MenuType == "Consultant")
            {
                if (Request.QueryString.Count == 0)
                {   //Default to new and featured
                    Resources res = new Resources();
                    DataTable resDT = res.ResourceList("CONSULTANTDEFAULTMEN");
                    string defaultMenu = resDT.Rows[0]["ContentUrl"].ToString();

                    dt = _oeConsObj.FetchProductsByMenu(new Guid(defaultMenu));

                    dt.DefaultView.Sort = "SortOrder desc";
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        CategoryNameLabel.Text = dr["MenuFullName"].ToString();
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No items to display";
                    }
                }

                if (qs != null)
                {
                    dt = _oeConsObj.FetchProductsByMenu(new Guid(qs));
                    dt.DefaultView.Sort = "SortOrder desc";
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        CategoryNameLabel.Text = dr["MenuFullName"].ToString();
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No items to display";
                    }

                }

                if (menuGuid != null)
                {
                    Guid mpptGuid = _oeConsObj.FetchMenuGuidFromMPTT(new Guid(menuGuid));
                    if (mpptGuid != Guid.Empty)
                    {
                        dt = _oeConsObj.FetchProductsByMenu(mpptGuid);
                        dt.DefaultView.Sort = "ProductNum desc";
                        dt = dt.DefaultView.ToTable();
                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            CategoryNameLabel.Text = dr["MenuFullName"].ToString();
                        }
                        else
                        {
                            CategoryNameLabel.Text = "No items to display";
                        }
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No items to display";
                    }

                }

                if (search != null)
                {
                    dt = this._oeConsObj.FetchProductData(this._oeConsObj.FetchSearchList(search));
                    dt.DefaultView.Sort = "ProductNum desc";
                    dt = dt.DefaultView.ToTable();
                    if (dt.Rows.Count > 0)
                    {
                        CategoryNameLabel.Text = "Search results: " + search;
                    }
                    else
                    {
                        CategoryNameLabel.Text = "No results found.";
                    }

                }
            }

            //DataTable dt = _oeCustObj.FetchProductData(_oeCustObj.FetchProductList(new Guid(qs)));

            HtmlGenericControl products = new HtmlGenericControl("ul");
            string baseProductLink = "";
            products.ID = "products";

            if (MenuType == "Customer")
            {
                string baseLink = "";
                try
                {
                    JJPro.Web.PersonalPage curPage = (JJPro.Web.PersonalPage)this.Page;
                    baseLink = curPage.BasePageLink;
                    if (baseLink == "/")
                    {
                        baseLink = "";
                    }
                }
                catch { }

                baseProductLink = baseLink + "Item.aspx?Item=";
            }

            if (MenuType == "Consultant")
            {

                baseProductLink = "Item.aspx?Item=";
            }

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {



                    HtmlGenericControl item = new HtmlGenericControl("li");
                    HtmlGenericControl imageThumb = new HtmlGenericControl("div");
                    HtmlGenericControl imageDiv = new HtmlGenericControl("div");
                    HtmlGenericControl image = new HtmlGenericControl("img");
                    HtmlGenericControl bubbleImageDiv = new HtmlGenericControl("div");
                    HtmlGenericControl bubbleImage = new HtmlGenericControl("img");
                    HtmlGenericControl imageLink = new HtmlGenericControl("a");
                    HtmlGenericControl caption = new HtmlGenericControl("div");
                    HtmlGenericControl bubbleTextDiv = new HtmlGenericControl("div");
                    HtmlGenericControl bubbleText = new HtmlGenericControl("div");
                    HtmlGenericControl productTitleDiv = new HtmlGenericControl("div");
                    HtmlGenericControl productTitle = new HtmlGenericControl("h4");
                    HtmlGenericControl productTitleLink = new HtmlGenericControl("a");
                    //HtmlGenericControl productDescription = new HtmlGenericControl("p");
                    HtmlGenericControl productNumberDiv = new HtmlGenericControl("div");
                    HtmlGenericControl productNumber = new HtmlGenericControl("p");
                    HtmlGenericControl priceRow = new HtmlGenericControl("div");
                    HtmlGenericControl priceDiv = new HtmlGenericControl("div");
                    HtmlGenericControl price = new HtmlGenericControl("p");
                    //HtmlGenericControl salePrice = new HtmlGenericControl("p");
                    //HtmlGenericControl addToCartDiv = new HtmlGenericControl("div");
                    //RadButton addToCartButton = new RadButton();
                    //UpdatePanel updatePanel = new UpdatePanel();
                    //AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();

                    //HtmlGenericControl addToCartButton = new HtmlGenericControl("a");
                    bool mobileDevice = Request.Browser.IsMobileDevice;

                    products.Attributes.Add("class", "row list-group");
                    if (mobileDevice)
                    {
                        item.Attributes.Add("class", "item  col-xs-12 col-md-3 col-lg-3 list-group-item");
                    }
                    else
                    {
                        item.Attributes.Add("class", "item  col-xs-3 col-md-3 col-lg-3 grid-group-item");
                    }
                    imageThumb.Attributes.Add("class", "thumbnail");
                    imageDiv.Attributes.Add("class", "imageDiv");

                    image.Attributes.Add("class", "group list-group-image img-responsive");
                    bubbleImageDiv.Attributes.Add("class", "bubbleImageDiv");
                    bubbleImage.Attributes.Add("class", "img-responsive");
                    caption.Attributes.Add("class", "caption ");
                    bubbleTextDiv.Attributes.Add("class", "col-xs-12 col-md-12");
                    bubbleText.Attributes.Add("class", "group inner list-group-item-heading custom-product-title bubble-comment");
                    productTitleDiv.Attributes.Add("class", "col-xs-12 col-md-12");
                    productTitle.Attributes.Add("class", "group inner list-group-item-heading custom-product-title");
                    //productDescription.Attributes.Add("class", "group inner list-group-item-text minimize product-description");
                    productNumberDiv.Attributes.Add("class", "col-xs-12 col-md-12 product-number-container");
                    productNumber.Attributes.Add("class", "group inner minimize product-number");
                    priceDiv.Attributes.Add("class", "col-xs-12 col-md-12 price-title-container");
                    price.Attributes.Add("class", "price-title");
                    //salePrice.Attributes.Add("class", "salePrice");
                    //addToCartDiv.Attributes.Add("class", "col-xs-12 col-md-6");
                    //addToCartButton.CssClass = "btn btn-success addToCartBtn";
                    //addToCartButton.CommandArgument = row["Key"].ToString();
                    //addToCartButton.ID = "btn" + row["ProductNum"].ToString().Replace("-", String.Empty);
                    //addToCartButton.Width = Unit.Pixel(119);
                    //addToCartButton.Height = Unit.Pixel(28);
                    //addToCartButton.Attributes.Add("bit", addToCartButton.ID);

                    //addToCartButton.Click += new EventHandler(addToCartButton_Click);
                    //trigger.ControlID = addToCartButton.UniqueID;
                    //trigger.EventName = "Click";
                    //updatePanel.ID = "pnl" + row["ProductNum"];
                    //updatePanel.Triggers.Add(trigger);
                    image.Attributes.Add("src", row["ImageUrl4"].ToString());
                    imageLink.Attributes.Add("href", baseProductLink + row["Key"]);
                    imageLink.Attributes.Add("class", "product-border");
                    if (row["BalloonImageUrl"] != null && row["BalloonImageUrl"].ToString() != String.Empty)
                    {
                        bubbleImage.Attributes.Add("src", row["BalloonImageUrl"].ToString());
                    }
                    else
                    {
                        if (row["BalloonComment"] != null && row["BalloonComment"].ToString() != String.Empty)
                        {
                            bubbleText.InnerHtml = row["BalloonComment"].ToString();
                        }
                    }
                    productTitle.InnerHtml = row["Value"].ToString();
                    productTitleLink.Attributes.Add("href", baseProductLink + row["ProductNum"]);
                    productTitleLink.Attributes.Add("class", "product-description-anchor");
                    //productDescription.InnerHtml = row["TopCopy"].ToString();
                    productNumber.InnerHtml = row["ProductNum"].ToString();
                    if (row["SalePrice"].ToString() != "")
                    {
                        price.InnerHtml = "<span style='text-decoration:line-through;'>" + row["Price"].ToString() + "</span>" + " " + "<span style='color:Red; font-weight:bold; text-decoration:none;'>" + row["SalePrice"].ToString() + "</span>";
                    }
                    else
                    {
                        price.InnerHtml = row["Price"].ToString();
                    }
                    //addToCartButton.ContentTemplate = new addtoBagTemplate();
                    //addToCartButton.Text = "Add To Cart";
                    //updatePanel.ContentTemplateContainer.Controls.Add(addToCartButton);

                    products.Controls.Add(item);
                    item.Controls.Add(imageThumb);
                    imageDiv.Controls.Add(bubbleImageDiv);
                    imageThumb.Controls.Add(imageLink);
                    imageThumb.Controls.Add(caption);
                    imageLink.Controls.Add(imageDiv);
                    imageDiv.Controls.Add(image);
                    bubbleImageDiv.Controls.Add(bubbleImage);
                    caption.Controls.Add(productTitleLink);
                    productNumberDiv.Controls.Add(productNumber);
                    caption.Controls.Add(productNumberDiv);
                    bubbleTextDiv.Controls.Add(bubbleText);
                    productTitleDiv.Controls.Add(productTitle);
                    productTitleLink.Controls.Add(productTitleDiv);
                    //caption.Controls.Add(productDescription);
                    caption.Controls.Add(priceRow);
                    caption.Controls.Add(priceDiv);
                    priceDiv.Controls.Add(price);
                    caption.Controls.Add(bubbleTextDiv);
                    //priceRow.Controls.Add(addToCartDiv);
                    //addToCartDiv.Controls.Add(updatePanel);
                }

            }

            ProductsPlaceHolder.Controls.Add(products);

        }

        void addToCartButton_Click(object sender, EventArgs e)
        {
            RadButton button = (RadButton)sender;
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "testAdd", "testAdd("+button.CommandArgument+")", true);

            if (MenuType == "Customer")
            {
                if (this.Page.GetType().BaseType.BaseType.ToString() == "JJPro.Web.PersonalPage" || this.Page.GetType().BaseType.BaseType.ToString() == "JJPro.Web.PersonalCustomerPage")
                {
                    JJPro.Web.PersonalPage curPersPage = (JJPro.Web.PersonalPage)this.Page;
                    curPersPage.PageOE.AddToBag(new Guid(button.CommandArgument));
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "completeAddToBag", "completeAddToBag(" + button.ClientID + ")", true);

                    int itemCount = curPersPage.PageOE.FetchBagPieceCount();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setCartCount", "setCartCount(" + itemCount + ");", true);
                }


            }

        }

        public class addtoBagTemplate : ITemplate
        {
            void ITemplate.InstantiateIn(Control container)
            {
                HtmlGenericControl buttonText = new HtmlGenericControl("span");
                buttonText.Attributes.Add("class", "rad-add-span");
                buttonText.InnerHtml = "Add To Cart";
                container.Controls.Add(buttonText);
            }
        }
    }
}
