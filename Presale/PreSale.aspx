<%@ Page Title="" Language="C#" MasterPageFile="~/Business/Shopping/ConsultantCheckout.Master" AutoEventWireup="true" CodeBehind="PreSale.aspx.cs" Inherits="JJPro.Web.Business.Shopping.PreSale" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        body {
            color: #000000 !important;
        }

        #itemstop {
            float: left;
            clear: both;
            margin-top: 20px;
            width: inherit;
        }

        #itemsbottom {
            float: left;
            clear: both;
            margin-top: 20px;
            width: inherit;
        }

        #presaleItems {
            float: left;
            width: 100%;
            margin-top: 30px;
            color: #000000;
        }

        #presaleCalculations {
            margin-top: 30px;
        }

        #presaleCalculationsTable {
            margin-left: auto;
            margin-right: auto;
            width: 230px;
            font-family: "garamond-premier-pro-display";
            font-size: 20px;
            text-align: center;
        }

        .presaleItemDiv {
            float: left;
            font-family: "garamond-premier-pro-display";
            font-size: 18px;
        }

        .presaleItem {
            margin-left: auto;
            margin-right: auto;
            cursor: pointer;
        }


        .presaleItemTextBox {
            width: 30px !important;
            float: right;
        }

        .item-qty-container {
            margin-left: auto;
            margin-right: auto;
            width: 75%;
        }


        div.RadToolTip table.rtWrapper td.rtWrapperContent {
            background-color: none !important;
            background-image: url("http://8f2270e13c3e0703baa2-6c2aaf386ecfc7423d48bccd1a997322.r2.cf1.rackcdn.com/BusinessPortal/Presale/2016_FW_pop_up.jpg") !important;
            color: White !important;
            width: 280px !important;
            height: 280px !important;
        }

        .tooltipBackground {
            font-family: "garamond-premier-pro-display";
            font-size: 20px;
            margin-top: 20px;
        }

            .tooltipBackground a:link {
                color: #A18679;
            }

            .tooltipBackground a:hover {
                color: #F8EFE6 !important;
            }

            .tooltipBackground a:visited {
                color: #F8EFE6;
            }

        #backgroundTest {
            background: url(https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/background%20%285%29.jpg) no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }

        .item-info {
            margin-left: auto;
            margin-right: auto;
            width: 60%;
            display: block;
        }

        .presale-heading {
            font-family: bebas-neue;
            font-size: 55px;
            margin-left: 2%;
            margin-right: 2%;
        }

        .season-heading {
            font-family: tamarillo;
            font-size: 40px;
            margin-left: 25px;
        }

        .presale-kit-heading {
        }

        .presale-description {
            font-family: proxima-nova;
            font-size: 16px;
            margin-left: 2%;
            margin-right: 2%;
        }

        /*Card flip Css*/

        .card {
            position: relative;
            height: 315px;
            width: 100%;
            text-align: center;
        }

        .card__front,
        .card__back {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
        }

        .card__front,
        .card__back {
            -webkit-backface-visibility: hidden;
            backface-visibility: hidden;
            -webkit-transition: -webkit-transform 0.3s;
            transition: transform 0.3s;
        }

        .card__front {
        }

        .card__back {
            -webkit-transform: rotateY(-180deg);
            transform: rotateY(-180deg);
            color: Black !important;
        }

        .card.effect__click.flipped .card__front {
            -webkit-transform: rotateY(-180deg);
            transform: rotateY(-180deg);
        }

        .card.effect__click.flipped .card__back {
            -webkit-transform: rotateY(0);
            transform: rotateY(0);
        }

        .pdf_link {
            color: #898f82 !important;
        }

        .red {
            background-color: Red;
        }

        .card_back_image {
            background-image: url("https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2018_Fall_Winter/flip%20backgrpund.jpg") !important;
            background-position: center center;
            background-position: center;
            background-repeat: no-repeat;
        }

        .h3_color {
            color: Black !important;
            margin-top: 38px;
        }

        .h3_color_purple {
            color: #BC5BD2;
        }

        .h3_color_orange {
            color: #F7631B;
        }

        .h3_color_pink {
            color: #D24BB7;
        }

        .h3_color_blue {
            color: #31AED6;
        }

        .h3_color_red {
            color: #F43350;
        }

        .h3_color_darkblue {
            color: #596C91;
        }

        a {
            color: #31AED6 !important;
        }

        @media (max-width: 768px) {
            #presaleCalculationsTable {
                margin-left: 0;
                margin-right: 0;
            }
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopContentPlaceHolder" runat="server">
    <div style="width: 100%;">
        <JJ:ConsultantHeader ID="ConsultantHeader" runat="server"></JJ:ConsultantHeader>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="clear-fix">
    </div>
    <asp:Panel ID="PresaleAvailablePanel" runat="server" CssClass="container">
        <div id="backgroundTest">
            <div id="main_content" class="row">
                <div id="main_header" style="margin-top: 30px;" class="col-md-12 col-xs-12">
                    <h3 class="presale-heading"><span class="season-heading">summer</span><br />
                        <span class="presale-kit-heading">LOOKBOOKS</span></h3>
                    <div class="presale-description">
                        <p>
                            Just Jewelry is excited to offer a brand new updated LookBook for Consultants.  These new LookBooks highlight summer catalog items and include a few sneak peek brand new never before offered items too!  In order to guarantee you receive your new LookBooks, we are offering pre-orders on both 2 packs and cases.  Pre-orders have a flat rate shipping of just $8 and will ship automatically when the new LookBooks arrive in April.   Again, unless you pre-order we CAN NOT guarantee we will have remaining quantities to sell of these new LookBooks, so be sure to place your pre-order no later than Monday, April 1st.  
                        </p>
                        <ul>
                            <%--                    <li>
                    Purchase the <b>JEWELRY KIT</b> if you would like a sampling of what to expect from our Fall/Winter jewelry collection.
                    </li>--%>
                            <li>Purchase 2 packs of our updated LookBook for just $16.
                            </li>
                            <li>Purchase a case of our updated LookBook and receive 1 pack FREE (6 packs total) for just $40.
                            </li>
                            <%--                    <li>
                    Purchase the <b>SAMPLE JEWELRY KIT</b> if you would like a sampling of the jewelry items in the Fall/Winter collection. 
                    </li>--%>
                            <%--                    <li>
                    Our kits do not include carryover jewelry,  pre-launch items, hostess item or purses.. Purchasing multiple kits will result in duplicate items.
                    </li>--%>
                        </ul>
<%--                        <i>Our kits do not include any carryover items</i>
                        <br />
                        <b>BE THE FIRST TO OFFER YOUR CUSTOMERS THE BEST IN SPRING FASHION FROM JUST JEWELRY...</b>--%>
                    </div>
                    <%--<asp:Image ID="PresaleHeaderImage" runat="server" style="display:none;" ImageUrl="http://8f2270e13c3e0703baa2-6c2aaf386ecfc7423d48bccd1a997322.r2.cf1.rackcdn.com/BusinessPortal/Presale/2016_FW_header_text.png" />--%>
                </div>
                <div id="presaleItems">
                    <div runat="server" id="itemstop" class="hidden">
                        <div class="col-md-2 col-xs-12">
                        </div>
                        <div runat="server" id="Kit1" class="presaleItemDiv col-md-4 col-xs-12 allKit">
                            <div class="card effect__Click">
                                <div class="card__front">
                                    <asp:Image ID="PresaleItem1Image" runat="server" CssClass="presaleItem img-responsive"
                                        ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/kitA.png" />
                                </div>
                                <div class="card__back card_back_image" style="color: Black;">
                                    <h3 class="h3_color">2019 Spring
                                <br />
                                        Jewelry + Boutique Kit</h3>

                                    <%--<span style="text-decoration: line-through;">$705</span> w/ discount - $421.20--%>
                                    <br />
                                    Retail Value: $746
                            <br />
                                    <a class="pdf_link" href="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/2019%20Spring%20Jewelry%20%2B%20Boutique%20Kit_update.pdf"
                                        target="_blank">Click here</a> to see a list of contents.
                            <br />
                                    <div style="font-size: 14px; margin-top: 10px;">
                                        <p class="item-info" style="font-size: 16px;">
                                            A sampling of both jewelry items and "one size" boutique items from our Spring collection.
                                        </p>
                                        <%--*Contains duplicate items from other kits--%>
                                    </div>
                                </div>
                            </div>
                            <div class="item-qty-container">
                                <div>
                                    <div style="float: left;">
                                        Jewelry + Boutique Kit
                                    </div>
                                    <div style="float: right;">
                                        <%--<asp:Label ID="PresaleItem1StrikedPriceLabel" runat="server" Text="$600.00" Style="text-decoration: line-through;"></asp:Label>--%>
                                        <asp:Label ID="PresaleItem1StrikedPriceLabel" runat="server" Text="746.00" Style="text-decoration: line-through;"></asp:Label>
                                        <asp:Label ID="PresaleItem1PriceLabel" runat="server" ForeColor="Darkred" Text="$452.60"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div style="float: left;">
                                        <span style="float: left;">Quantity: </span>
                                        <telerik:RadNumericTextBox ID="PresaleItem1TextBox" runat="server" ClientEvents-OnBlur="priceBlurFunction" EnableViewState="true" DisplayText="0" Type="Number"
                                            CssClass="presaleItemTextBox" NumberFormat-DecimalDigits="0" Value="0" MinValue="0" Style="float: left;">
                                        </telerik:RadNumericTextBox>
                                    </div>
                                </div>

                            </div>

                        </div>
                        <div runat="server" id="Kit2" class="presaleItemDiv col-md-4 col-xs-12 allKit">
                            <div class="card effect__Click">
                                <div class="card__front">
                                    <asp:Image ID="PresaleItem2Image" runat="server" CssClass="presaleItem img-responsive"
                                        ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/kitB.png" />
                                </div>
                                <div class="card__back card_back_image" style="color: Black;">
                                    <h3 class="h3_color">2019 Spring
                                <br />
                                        Jewelry + Ultimate Boutique Kit</h3>

                                    <%--<span style="text-decoration: line-through;">$922.00</span> w/ discount ---%> <%--$508.50--%>
                                    <br />
                                    Retail Value: $2,199
                            <br />
                                    <a class="pdf_link" href="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/2019%20Spring%20Jewelry%20%2B%20Ultimate%20Boutique%20Kit_update.pdf"
                                        target="_blank">Click Here</a> to see a list of contents.
                            <div style="font-size: 14px; margin-top: 10px;">
                                <p class="item-info" style="font-size: 16px;">
                                    A sampling of both jewelry items, "one size" boutique items and sized items from our Spring collection. Sized items will be chosen at random.
                                </p>
                                <%--*Contains duplicate items from other kits--%>
                            </div>
                                </div>
                            </div>
                            <div class="item-qty-container">
                                <div>
                                    <div style="float: left;">
                                        Jewelry+Ultimate Boutique Kit
                                    </div>
                                    <br />
                                    <div style="float: left;">
                                        <asp:Label ID="PresaleItem2StrikedPriceLabel" runat="server" Text="$2,199" Style="text-decoration: line-through;"></asp:Label>
                                        <asp:Label ID="PresaleItem2PriceLabel" runat="server" ForeColor="Darkred" Text="$1469.70"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div style="float: left;">
                                        <span style="float: left;">Quantity: </span>
                                        <telerik:RadNumericTextBox ID="PresaleItem2TextBox" runat="server" CssClass="presaleItemTextBox"
                                            ClientEvents-OnBlur="priceBlurFunction" NumberFormat-DecimalDigits="0" Value="0"
                                            Style="float: left;" MinValue="0">
                                        </telerik:RadNumericTextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-2 col-xs-12">
                        </div>
                        <div runat="server" style="display: none;" id="Kit3" class="presaleItemDiv col-md-4 col-xs-12 partyOnlyKit">
                            <div class="card effect__Click">
                                <div class="card__front">
                                    <asp:Image ID="PresaleItem3Image" runat="server" CssClass="presaleItem img-responsive"
                                        ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2018_Spring_Summer/vip%20only.png" />
                                </div>
                                <div class="card__back card_back_image" style="color: Black;">
                                    <h3 class="h3_color">2019 Spring
                                <br />
                                        Leader Hosted Kit
                                    </h3>

                                    Only: <span style="text-decoration: line-through;">$1439.20</span> Price - $1104
                            <br />
                                    Retail Value: $2208
                            <br />
                                    <a href="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2018_Spring_Summer/2018%20Spring%20Summer%20Leader%20Hosted%20Kit.pdf"
                                        target="_blank">Click Here</a> to see a list of contents.

                            <div style="font-size: 14px; margin-top: 10px;">
                                <p class="item-info" style="font-size: 16px;">
                                    A sampling of almost every jewelry item and boutique item in our new collection. Sized items will vary.
                                </p>
                                <%-- *Contains duplicate items from both sample kits--%>
                            </div>
                                </div>
                            </div>
                            <div class="item-qty-container">
                                <div>
                                    <div style="float: left; font-size: 16px;">
                                        Leader Hosted Kit
                                    </div>
                                    <div style="float: right;">
                                        <asp:Label ID="PresaleItem3StrikedPriceLabel" runat="server" Text="$1439.20" Style="text-decoration: line-through;"></asp:Label>
                                        <asp:Label ID="PresaleItem3PriceLabel" runat="server" ForeColor="Darkred" Text="$1104"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div style="float: left;">
                                        <span style="float: left;">Quantity: </span>
                                        <telerik:RadNumericTextBox ID="PresaleItem3TextBox" runat="server" CssClass="presaleItemTextBox"
                                            ClientEvents-OnBlur="priceBlurFunction" NumberFormat-DecimalDigits="0" Value="0"
                                            Style="float: left;" MinValue="0">
                                        </telerik:RadNumericTextBox>
                                    </div>
                                </div>
                            </div>
                        </div>



                    </div>
                    <div id="itemsbottom">
                        <div class="col-md-2 col-xs-12">
                        </div>
                        <div class="presaleItemDiv col-md-4 col-xs-12">
                            <div class="card effect__Click">
                                <div class="card__front">
                                    <asp:Image ID="PresaleItem4Image" runat="server" CssClass="presaleItem img-responsive"
                                        ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Summer/lookbook%20case%20image.jpg" />
                                </div>
                                <div class="card__back card_back_image" style="color: Black;">
                                    <h3 class="h3_color">2019 Summer
                                <br />
                                        Lookbook Case</h3>
                                    <br />
                                    Only: $40
                            <br />
                                    <br />
                                    <p class="item-info" style="font-size: 16px;">
                                        Stock up NOW at a discounted price! A case contains 6 packs of 20 lookbooks.
                                    </p>

                                    <br />
                                </div>
                            </div>
                            <div class="item-qty-container">
                                <div>
                                    <div style="float: left;">
                                        Look Book Case
                                    </div>
                                    <div style="float: right;">
                                        <asp:Label ID="PresaleItem4StrikedPriceLabel" runat="server" Text="$48.00" Style="text-decoration: line-through;"></asp:Label>
                                        <asp:Label ID="PresaleItem4PriceLabel" runat="server" ForeColor="Darkred" Text="40.00"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div style="float: left;">
                                        <span style="float: left;">Quantity: </span>
                                        <telerik:RadNumericTextBox ID="PresaleItem4TextBox" runat="server" CssClass="presaleItemTextBox"
                                            ClientEvents-OnBlur="priceBlurFunction" NumberFormat-DecimalDigits="0" Value="0"
                                            Style="float: left;" MinValue="0">
                                        </telerik:RadNumericTextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="Catalog2Pack" runat="server"  class="presaleItemDiv col-md-4 col-xs-12">
                            <div class="card effect__Click">
                                <div class="card__front">
                                    <asp:Image ID="PresaleItem5Image" runat="server" CssClass="presaleItem img-responsive"
                                        ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Summer/lookbook%202pack%20image.jpg" />
                                </div>
                                <div class="card__back card_back_image" style="color: Black;">
                                    <h3 class="item-info h3_color">2019 Summer<br />
                                        2 Lookbook Packs</h3>
                                    <br />
                                    Only:$22
                                                        <br />
                                    <br />
                                    <p class="item-info" style="font-size: 16px;">
                                        Purchase 2 packs of 20 lookbooks.
                                    </p>


                                </div>
                            </div>
                            <div class="item-qty-container">
                                <div>
                                    <div style="float: left; width: 150px;">
                                        2 Lookbook Packs
                                    </div>
                                    <div style="float: right; clear: right;">
                                        <asp:Label ID="PresaleItem5StrikedPriceLabel" runat="server" Text="16" style="display:none;"></asp:Label>
                                        <asp:Label ID="PresaleItem5PriceLabel" runat="server" Text="$16.00"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div style="float: left; clear: both;">
                                        <span style="float: left;">Quantity: </span>
                                        <telerik:RadNumericTextBox ID="PresaleItem5TextBox" runat="server" CssClass="presaleItemTextBox"
                                            ClientEvents-OnBlur="priceBlurFunction" NumberFormat-DecimalDigits="0" Value="0"
                                            Style="float: left;" MinValue="0">
                                        </telerik:RadNumericTextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="presaleItemDiv col-md-4 col-xs-12" style="display: none;">
                            <div class="card effect__Click">
                                <div class="card__front">
                                    <asp:Image ID="PresaleItem6Image" runat="server" CssClass="presaleItem img-responsive" ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/hostess_pack.png" />
                                </div>
                                <div class="card__back card_back_image" style="color: Black;">
                                    <h3 class="item-info h3_color">2019 Spring<br />
                                        Hostess Necklace Dozen Pack</h3>
                                    Only:$65.00
                             <br />
                                    <p class="item-info" style="font-size: 16px;">
                                        Stock up on our hostess exclusive set at this one time only discounted price. 
                             <br />
                                        Includes 12 Hostess Necklaces.
                                    </p>


                                </div>
                            </div>
                            <div class="item-qty-container">
                                <div>
                                    <div style="float: left; width: 140px; font-size: 16px;">
                                        Hostess Necklace Dozen Pack
                                    </div>
                                    <div style="float: right; clear: right;">
                                        <asp:Label ID="PresaleItem6StrikedPriceLabel" runat="server" Text="72.00" Style="text-decoration: line-through;"></asp:Label>
                                        <asp:Label ID="PresaleItem6PriceLabel" runat="server" ForeColor="Darkred" Text="$65.00"></asp:Label>
                                    </div>
                                </div>
                                <div>
                                    <div style="float: left; clear: both;">
                                        <span style="float: left;">Quantity: </span>
                                        <telerik:RadNumericTextBox ID="PresaleItem6TextBox" runat="server" CssClass="presaleItemTextBox"
                                            ClientEvents-OnBlur="priceBlurFunction" NumberFormat-DecimalDigits="0" Value="0" Style="float: left;"
                                            MinValue="0">
                                        </telerik:RadNumericTextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-2 col-xs-12">
                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-xs-12" style="margin-top: 30px;">
                    <div style="text-align: center; display:none;">
                        <asp:Image ID="DoublePointsImage" runat="server" ImageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Presale/2019_Spring/triple%20points-.jpg" />
                    </div>
                </div>
                <div id="presaleCalculations" class="col-md-6 col-xs-12">
                    <div>
                        <table id="presaleCalculationsTable">
                            <tbody>
                                <tr style="width: 100%;  display: none;">
                                    <td>
                                        <div>
                                            <div style="float: left; width: 160px;">
                                                Kit 1
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="PresaleKit1Total" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr id="jewelryBoutiqueKit" runat="server" style="width: 100%; display: none;">
                                    <td>
                                        <div>
                                            <div style="float: left; width: 160px;">
                                                Kit 2
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="PresaleKit2Total" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr id="vipOnlyTotal" runat="server" style="width: 100%; display: none;">
                                    <td>
                                        <div>
                                            <div style="float: left; width: 160px;">
                                                VIP Only Kit
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="PresaleKit3Total" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr style="width: 100%;">
                                    <td>
                                        <div>
                                            <div style="float: left; width: 160px;">
                                                Lookbook Case
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="PresaleCatalogsTotal" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr id="catalog2PackTotal" runat="server" style="width: 100%;">
                                    <td>
                                        <div>
                                            <div style="float: left; width: 160px;">
                                                Lookbook 2 Pack
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="PresalePackTotal" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr style="width: 100%;  display: none;">
                                    <td>
                                        <div>
                                            <div style="float: left; width: 160px;">
                                                Hostess Pack
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="PresaleHostess2Total" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr style="width: 100%;">
                                    <td>
                                        <div>
                                            <div style="width: 100%;">
                                                <hr />
                                            </div>
                                            <div style="float: left; width: 160px;">
                                                Subtotal
                                            </div>
                                            <div style="float: right;">
                                                <asp:Label ID="SubTotal" runat="server" Text="$0.00"></asp:Label>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <span style="font-size: 16px;">*Tax and shipping will be applied at checkout</span>
                        <telerik:RadButton ID="CheckoutButton" runat="server" Text="Proceed To Checkout" Skin="Vista">
                        </telerik:RadButton>
                    </div>
                </div>
            </div>
        </div>
        <div class="presaleItemDiv col-md-4 col-xs-12" style="width: 100%!important;">
            <span id="shipDateSpan" runat="server"></span>
        </div>
        <asp:HiddenField ID="CountryHiddenField" runat="server" />
    </asp:Panel>
    <asp:Panel ID="PresaleEndedPanel" runat="server" Visible="false">
        <div style="text-align: center;">
            Presale has ended and this page is no longer available.
        </div>
    </asp:Panel>
    <script>

        $(document).ready(function () {


            $("#<%= PresaleItem1TextBox.ClientID %>").val(0);
                $("#<%= PresaleItem1TextBox.ClientID %>").val(0);
                $("#<%= PresaleItem2TextBox.ClientID %>").val(0);
                $("#<%= PresaleItem3TextBox.ClientID %>").val(0);
                $("#<%= PresaleItem4TextBox.ClientID %>").val(0);
                $("#<%= PresaleItem5TextBox.ClientID %>").val(0);
                $("#<%= PresaleItem6TextBox.ClientID %>").val(0);
        });

        function priceBlurFunction() {


            var country = $("#<%= CountryHiddenField.ClientID %>").val();

                var kit1Qty = $("#<%= PresaleItem1TextBox.ClientID %>").val();
                var kit2Qty = $("#<%= PresaleItem2TextBox.ClientID %>").val();
                var kit3Qty = $("#<%= PresaleItem3TextBox.ClientID %>").val();
                var catalogsQty = $("#<%= PresaleItem4TextBox.ClientID %>").val();
                var hostess1Qty = $("#<%= PresaleItem5TextBox.ClientID %>").val();
                var hostess2Qty = $("#<%= PresaleItem6TextBox.ClientID %>").val();

                var kit1Total = $("#<%= PresaleKit1Total.ClientID %>");
                var kit2Total = $("#<%= PresaleKit2Total.ClientID %>");
                var kit3Total = $("#<%= PresaleKit3Total.ClientID %>");
                var catalogsTotal = $("#<%= PresaleCatalogsTotal.ClientID %>");
                var hostess1Total = $("#<%= PresalePackTotal.ClientID %>");
                var hostess2Total = $("#<%= PresaleHostess2Total.ClientID %>");
                var subtotal = $("#<%= SubTotal.ClientID %>");
            var kit1price = 452.60;
            var kit2price = 1469.70;
            var kit3price = 1104.00;
            var catalogUSprice = 40.00;
            var catalogCAprice = 40.00;
            var hostess1price = 16.00;
            var hostess2price = 65.00;


            var kit1TotalValue = kit1Qty * kit1price;
            var kit2TotalValue = kit2Qty * kit2price;
            var kit3TotalValue = kit3Qty * kit3price;
            var hostess1TotalValue = hostess1Qty * hostess1price;
            var hostess2TotalValue = hostess2Qty * hostess2price;
            var catalogsTotalValue;

            if (country == 'CA') {
                catalogsTotalValue = catalogsQty * catalogCAprice;
            }

            if (country == 'US') {
                catalogsTotalValue = catalogsQty * catalogUSprice;
            }

            var subTotalValue = (kit1TotalValue + kit2TotalValue + kit3TotalValue + catalogsTotalValue + hostess1TotalValue + hostess2TotalValue);


            kit1Total.html("$" + kit1TotalValue.toFixed(2));
            kit2Total.html("$" + kit2TotalValue.toFixed(2));
            kit3Total.html("$" + kit3TotalValue.toFixed(2));
            catalogsTotal.html("$" + catalogsTotalValue.toFixed(2));
            hostess1Total.html("$" + hostess1TotalValue.toFixed(2));
            hostess2Total.html("$" + hostess2TotalValue.toFixed(2));
            subtotal.html("$" + subTotalValue.toFixed(2));


        }

        (function () {
            var cards = document.querySelectorAll(".card.effect__click");
            for (var i = 0, len = cards.length; i < len; i++) {
                var card = cards[i];
                clickListener(card);
            }

            function clickListener(card) {
                card.addEventListener("click", function () {
                    var c = this.classList;
                    c.contains("flipped") === true ? c.remove("flipped") : c.add("flipped");
                });
            }
        })();

    </script>

    <div style="margin-bottom: 15%;">
    </div>

</asp:Content>
