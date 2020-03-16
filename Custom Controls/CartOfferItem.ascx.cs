using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Core;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.Services;

namespace JJPro.Web.Controls
{
    public partial class CartOfferItem : System.Web.UI.UserControl
    {

        int _productsCount;
        int _quantityAvailable;
        string _productNumber;
        string _cartOfferID;
        string _successMessageID;
        int _itemIndex;
        bool _hasOptions = false;


        public int ProductsCount
        {
            get
            {
                return _productsCount;

            }

            set
            {
                _productsCount = value;


            }
        }

        public int QuantityAvailable
        {
            get
            {
                return _quantityAvailable;

            }

            set
            {
                _quantityAvailable = value;


            }
        }

        public string ProductNumber
        {
            get
            {
                return _productNumber;

            }

            set
            {
                _productNumber = value;
                AddToCartButton.Attributes.Add("ProductNum", value.ToString());

            }
        }


        public string CartOfferID
        {
            get
            {
                return _cartOfferID;

            }

            set
            {
                _cartOfferID = value;
                AddToCartButton.ClientIDMode = ClientIDMode.Static;
                AddToCartButton.ID = value;


            }
        }


        public string SuccessMessageID
        {
            get
            {
                return _successMessageID;

            }

            set
            {
                _successMessageID = value;
                SuccessMessage.ClientIDMode = ClientIDMode.Static;
                SuccessMessage.ID = value; 


            }
        }

        public int ItemIndex
        {
            get
            {
                return _itemIndex;

            }

            set
            {
                _itemIndex = value;

                AddToCartButton.Attributes.Add("Index", value.ToString());
                ProductOptions.Attributes.Add("Index", value.ToString());
                SuccessMessage.Attributes.Add("Index", value.ToString());

            }
        }

        public bool ProductHasOptions
        {
            get
            {
                return _hasOptions;
            }

            set
            {
                _hasOptions = value;
                AddToCartButton.Attributes.Add("HasOptions", value.ToString());
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //AddToCartButton.ServerClick += AddToCartButton_ServerClick;
        }
        


        protected void Page_Load(object sender, EventArgs e)
        {
            OEObject oe = new OEObject();

            DataTable ProductDetailsTable = oe.FetchProductDetails(ProductNumber);
            if (ProductDetailsTable.Rows.Count > 0)
            {
                DataRow ProductDetailsRow = ProductDetailsTable.Rows[0];
                string hasOptions = ProductDetailsRow["HasOptions"].ToString(); 
                int productsCount = ProductsCount;
                int quantityAvailable = Convert.ToInt16(ProductDetailsRow["UnallocatedCount"]); 
                string baseLinkConsultant =  "http://jjpro.justjewelry.com/Business/Shopping/"; 
                

                QuantityAvailable = quantityAvailable;
                ProductName.InnerHtml = ProductDetailsRow["Description"].ToString();
                ProductImageLink.Attributes.Add("href", baseLinkConsultant + "Item.aspx?Item=" + ProductDetailsRow["ProductNumber"].ToString());
                ProductImageLink.Attributes.Add("target", "_blank");
                ProductImage.Src = ProductDetailsRow["Image1Url"].ToString();
                ProductRetailPrice.InnerHtml = ProductDetailsRow["Price"].ToString();
                ProductSalePrice.InnerHtml = ProductDetailsRow["SalePrice"].ToString();
                if(ProductDetailsRow["SalePrice"].ToString() == String.Empty)
                {
                    ProductRetailPrice.Attributes.Remove("class"); 
                }
                if(hasOptions == "false")
                {

                }

                if (hasOptions == "true")
                {
                    ProductHasOptions = true;
                    DataTable ProductOptionsTable = oe.FetchProductOptions(new Guid(ProductDetailsRow["ProductsGuid"].ToString()));
                    ListItem defaultOption = new ListItem();
                    int availableQuantity = 0;
                    defaultOption.Attributes.Add("value", "default");
                    defaultOption.Text = "Select Option";
                    ProductOptions.Items.Add(defaultOption);

                    if (ProductOptionsTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in ProductOptionsTable.Rows)
                        {
                            ListItem option = new ListItem();
                            option.Attributes.Add("value", row["Key"].ToString());
                            option.Value = row["Key"].ToString();
                            option.Text = row["Value"].ToString();
                            ProductOptions.Items.Add(option);
                            availableQuantity =  Convert.ToInt16(row["QuantityAvailable"]);
                            quantityAvailable = quantityAvailable + availableQuantity; 
                        }

                        QuantityAvailable = quantityAvailable;
                        if (availableQuantity <= 0)
                        {
                            ProductContainer.Style.Add("display", "none");
                        }
                    }
                    else
                    {
                        ProductContainer.Style.Add("display", "none");
                    }

                }
                else
                {
                    ProductOptions.Style.Add("display", "none");
                }

                if (quantityAvailable <= 0)
                {
                    ProductContainer.Style.Add("display", "none");
                }

                switch (ProductsCount)
                {
                    case 1:
                        ProductContainer.Attributes.Add("class", "col-lg-12 col-sm-12 text-center CartOfferProducts");

                        break;

                    case 2:
                        ProductContainer.Attributes.Add("class", "col-lg-6 col-sm-12 text-center CartOfferProducts");

                        break;

                    case 3:
                        ProductContainer.Attributes.Add("class", "col-lg-4 col-sm-12 text-center CartOfferProducts");

                        break;

                    case 4:
                        ProductContainer.Attributes.Add("class", "col-lg-3 col-sm-12 text-center CartOfferProducts");

                        break; 
                }
            }
        }
    }
}