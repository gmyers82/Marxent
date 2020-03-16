<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartOfferItem.ascx.cs" Inherits="JJPro.Web.Controls.CartOfferItem" %>
<div class="col-md-4 CartOfferProducts" runat="server" id="ProductContainer">
    <span runat="server" id="ProductName" class="CartOfferProductName"></span>
    <br />
    <a id="ProductImageLink" runat="server">
    <img runat="server" id="ProductImage" class="CartOfferProductImage" />
        </a>
    <br />
    <span id="ProductRetailPrice" runat="server" class="CartOfferRetailPrice"></span>
    <span id="ProductSalePrice" runat="server" class="CartOfferSalePrice"></span>
    <br />
    <select runat="server" id="ProductOptions" onchange="OptionDDLChange(this.id)">
    </select>
    <br />
    <button id="AddToCartButton" type="button" class="CartOfferAddToCart" hasoptions="false" onclick="AddToOfferCart(this.id)" runat="server">Add To Bag</button>
    <br />
    <br />
    <span id="SuccessMessage" runat="server"></span>
</div>
<script>
    //$(document).ready(function () {
    //    $(".CartOfferAddToCart").click(function () {
    //        var itemIndex = $(this).attr('index');



    //        AddToOfferCart(itemIndex);
    //    });
    //});

    function AddToOfferCart(e) {


        var buttonID = '#' + e;
        var index = $(buttonID).attr('index');
        var successID = '#SuccessMessage' + index;
        var productNum = $(buttonID).attr('productNum');
        
        $(document).ajaxStart(function () {

            $('#CartOfferAddButton' + index).html('<i class="fa fa-spinner fa-spin"></i>One Moment . . .');

        });

        $(document).ajaxComplete(function () {
            $('#CartOfferAddButton' + index).html("Add To Cart");
        });

        //    var orderGuid = $('#GuidHiddenField');
        //    var productNum = $('#QuickAddTextBox');
        //var quantity = $('#QuickAddQuantity');


        var orderGuid = $('#GuidHiddenField');
        var successMessage = $
        var quantity = '1';
        var productNumVal = productNum;
        var options = $(buttonID).attr('hasoptions');
        var hasProductOptions = options;
        var quantityVal = '1';
        var productOptionGuid = '00000000-0000-0000-0000-000000000000';
        if (hasProductOptions == 'True') {
            if ($(buttonID).attr('optionguid') == null) { }
            else {
                productOptionGuid = $(buttonID).attr('optionguid');
            }
        }

        var orderGuidVal = orderGuid.val();

        $.ajax({
            type: "POST",
            url: "Cart.aspx/CartOfferAdd",
            data: JSON.stringify({ ordersGuid: orderGuidVal, productNumber: productNumVal, quantity: quantityVal, productGuid: productOptionGuid, hasOptions: hasProductOptions }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $(successID).html(data.d);
            },
            failure: function () {
                $(successID).html("Failed");
            }

        });
        // var itemIndex = $(this).attr('index');
        //alert($(this).attr(itemIndex));

        // $('#CartOfferAddButton'+itemIndex).html('<i class="fa fa-spinner fa-spin"></i>One Moment . . .');

        //$(document).ajaxStart(function () {

        //    $('#CartOfferAddButton'+itemIndex).html('<i class="fa fa-spinner fa-spin"></i>One Moment . . .');

        //});

        //$(document).ajaxComplete(function () {
        //    $('#quickAddToCart').html("Add To Cart");
        //});

        //function quickAddSuccess(productnum) {

        //    $('#QuickAddSuccess span').text(productnum);
        //    $('#QuickAddSuccess').removeClass('hidden');
        //    $("#QuickAddSuccess").slideDown(300).delay(1500).fadeOut(400);

        //    $('#ItemsAddedList').append(productnum + "<hr /><br />");

        //}

        //function quicikAddFailure(productnum) {

        //    $('#QuickAddError span').text(productnum);
        //    $('#QuickAddError').removeClass('hidden');
        //    $("#QuickAddError").slideDown(300).delay(4500).fadeOut(400);

        //}

        //var orderGuid = $('#GuidHiddenField');
        //var productNum = $('#QuickAddTextBox');
        //var quantity = $('#QuickAddQuantity');
        //var productNumVal = productNum.val().trim();
        //var quantityVal = quantity.val().trim();
        //var orderGuidVal = orderGuid.val();
        //$.ajax({
        //    type: "POST",
        //    url: "Cart.aspx/QuickAdd",
        //    data: JSON.stringify({ ordersGuid: orderGuidVal, productNumber: productNumVal, quantity: quantityVal }),
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (data) {


        //        var text = data.d;

        //        if (text.indexOf("Error:") > -1) {
        //            quicikAddFailure(data.d);

        //        }
        //        else {

        //            quickAddSuccess(data.d);
        //            productNum.val("");
        //            quantity.val("");
        //        }
        //    },
        //    error: function (data) {

        //        if (productNumVal == "") {
        //            quicikAddFailure("Error: Please enter a valid product number.")
        //        }
        //        else {
        //            quicikAddFailure(data.d);
        //        }

        //        if (quantityVal == "") {
        //            quicikAddFailure("Error: Please enter a quantity.")
        //        }
        //        else {
        //            quicikAddFailure(data.d);
        //        }
        //    }


        //});
    }

    function OptionDDLChange(e) {

        var select = '#' + e;
        var optionSelected = $(select).find(":selected").val();
        var index = $(select).attr('index');
        var cartButton = '#CartOfferAddButton' + index;
        $(cartButton).attr('optionguid', optionSelected);
    }
</script>
