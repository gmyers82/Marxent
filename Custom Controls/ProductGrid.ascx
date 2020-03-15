<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductGrid.ascx.cs" Inherits="JJPro.Web.Controls.ResponsiveControls.ProductGrid" %>
<script type="text/javascript">
    $(window).load(function () {
        var w = $(window).width();
        if (w >= 768) {
            $('#products .item').removeClass('list-group-item');
            $('#products .item').addClass('grid-group-item');
        }
        else {
            $('#products .item').addClass('list-group-item');
            $('#products .item').removeClass('grid-group-item');
            var prevBtn = $('.prevbtn');
            var nextBtn = $('.nextbtn');
            prevBtn.html("&larr; Previous");
            prevBtn.parent().addClass("previous");
            nextBtn.html("Next<span aria-hidden='true'>&rarr;</span>");
            nextBtn.parent().addClass("next");
        }
    });

    $(window).resize(function () {
        var w = $(window).width();
        if (w >= 768) {
            $('#products .item').removeClass('list-group-item');
            $('#products .item').addClass('grid-group-item');
        }
        else {
            $('#products .item').addClass('list-group-item');
            $('#products .item').removeClass('grid-group-item');
            var prevBtn = $('.prevbtn');
            var nextBtn = $('.nextbtn');
            prevBtn.html("&larr; Previous");
            prevBtn.parent().addClass("previous");
            nextBtn.html("Next<span aria-hidden='true'>&rarr;</span>");
            nextBtn.parent().addClass("next");
        }
    });
    $(document).ready(function () {
        $("span.rad-add-span").parent().click(function () {
            $(this).html('<i class="fa fa-spinner fa-spin"></i>Adding. . .');

        });


        var minimized_elements = $('p.minimize');


        minimized_elements.each(function () {
            var t = $(this).text();
            var productNumLink = $(this).prev("a").attr("href");
            if (t.length < 150) return;

            $(this).html(
            t.slice(0, 100) + '<span>... </span><a href="' + productNumLink + '" class="more">More</a></span>'
        );

        });


        $('ul.pager li').click(function (e) {
            $("ul.pager li").removeClass("active");
            $(this).addClass("active");
        });


        var listElement = $('#products');
        var perPage = 60;
        var numItems = listElement.children().length;
        var numPages = Math.ceil(numItems / perPage);

        $('.pager').data("curr", 0);

        var curr = 0;



        if (numPages > 1) {
            $('<li class="prev"><a href="#" aria-label="Previous" class="page_link prevbtn disabled"><span aria-hidden="true">&laquo;</span></a></li>').appendTo('.pager');
        }
        while (numPages > curr) {
            $('<li class="page_num hidden-xs"><a href="#" class="page_link list' + curr + '">' + (curr + 1) + '</a></li>').appendTo('.pager');
            curr++;
        }

        if (numPages > 1) {
            $('<li class="next"><a href="#" aria-label="Next" class="page_link nextbtn"><span aria-hidden="true">&raquo;</span></a></li>').appendTo('.pager');
        }

        if ($('#pageNumberHiddenField').val() != "") {
            var pageNum = $('#pageNumberHiddenField').val();
            var pageNumControl = $('.pager li').find('a').filter(function () {
                return $(this).text() === pageNum;
            });
            goTo(pageNum - 1);
            $('.pager li a').removeClass('active');
            var t = $('.list' + (pageNum - 1).toString());
            t.addClass('active');

            if (pageNum > 1) {
                $('.prevbtn').removeClass('disabled');
            }
            else {
                $('.prevbtn').addClass('disabled');
            }
            if (pageNum < numPages - 1) {
                $('.nextbtn').removeClass('disabled');
            }
            else {
                $('.nextbtn').addClass('disabled');
            }
        }
        else {

            var pageNum = 1;
            var pageNumControl = $('.pager li').find('a').filter(function () {
                return $(this).text() === pageNum;
            });
            listElement.children().css('display', 'none');
            listElement.children().slice(0, perPage).css('display', 'block');
            $('.pager li a').removeClass('active');
            var t = $('.list' + (pageNum - 1).toString());
            t.addClass('active');
        }








        $('.pager li a').click(function () {
            if ($.isNumeric($(this).html().valueOf())) {

                var split = $(this).attr("class").split(' ');
                var listPage = '.' + split[1];
                var clickedPage = $(this).html().valueOf() - 1;
                if (clickedPage > 0) {
                    $('.prevbtn').removeClass('disabled');
                }
                else {
                    $('.prevbtn').addClass('disabled');
                }
                if (clickedPage < numPages - 1) {
                    $('.nextbtn').removeClass('disabled');
                }
                else {
                    $('.nextbtn').addClass('disabled');
                }
                goTo(clickedPage, perPage);
                $('.pager li a').removeClass('active');
                $('.pager').find(listPage).addClass('active');
                var pageNumControl = $('#pageNumberHiddenField');
                pageNumControl.val(clickedPage + 1);
            }
            else {
                var pageNumControl = $('#pageNumberHiddenField');
                if ($(this).hasClass("prevbtn")) {
                    if ($('.prevbtn').hasClass('disabled')) return false;
                    $('.nextbtn').removeClass('disabled');
                    var clickedPage = parseInt($('.pager').attr("curr"), 10) - 1;
                    if (clickedPage <= 0) {
                        $('.prevbtn').addClass('disabled');
                    }
                    else {
                        $('.prevbtn').removeClass('disabled');
                    }
                    goTo(clickedPage, perPage);
                    pageNumControl.val(clickedPage + 1);
                    $('.pager li a').removeClass('active');
                    var pageNum = parseInt($(pageNumControl).val(), 10);
                    var pagerNumControl = $('.pager li').find('a').filter(function () {
                        return $(this).text() === pageNum.toString();

                    });
                    pagerNumControl.addClass('active');
                }

                if ($(this).hasClass("nextbtn")) {
                    var clickedPage = parseInt($('.pager').attr("curr"), 10) + 1;
                    if (isNaN(clickedPage)) {
                        clickedPage = 1;
                    }
                    if ($('.nextbtn').hasClass('disabled')) return false;
                    $('.prevbtn').removeClass('disabled');
                    if (clickedPage == numPages - 1) {
                        $('.nextbtn').addClass('disabled');
                    }
                    else {
                        $('.nextbtn').removeClass('disabled');
                    }
                    goTo(clickedPage, perPage);
                    pageNumControl.val(clickedPage + 1);
                    $('.pager li a').removeClass('active');
                    var pageNum = parseInt($(pageNumControl).val(), 10);
                    var pagerNumControl = $('.pager li').find('a').filter(function () {
                        return $(this).text() === pageNum.toString();

                    });
                    pagerNumControl.addClass('active');
                }


            }
        });



        function goTo(page) {
            var startAt = page * perPage,
            endOn = startAt + perPage;
            listElement.children().css('display', 'none').slice(startAt, endOn).css('display', 'block');
            $('.pager').attr("curr", page);
        }



    });

    function showLoadingGif() {

        alert($(this).id.toString());
        $(this).addClass('animatedBackground');

    }

    function testAdd(guid) {
        alert(guid.toString());
    }



    function animateAddToBag() {
        var btnText = $(this).closest('span.rad-add-span');

    }

    function completeAddToBag(clientID) {
        var btn = $("#" + clientID.id);
        var btnText = btn.find('span.rad-add-span');
        btnText.html('<span><i class="fa fa-check"></i>Got it!</span>').fadeIn().delay(3000);
        setTimeout(function () {
            btnText.html('Add To Cart').fadeIn().delay(3000);
        }, 3000);

        $("span.rad-add-span").parent().click(function () {
            $(this).html('<i class="fa fa-spinner fa-spin"></i>Adding. . .');

        });


    }


</script>


<style type="text/css">
    
    .custom-container
    {
        max-width:1300px;
    }
    
    .animatedBackground
    {
        background-color:Yellow!important;
        background-image: url('../../Images/loading.gif');
        background-repeat:no-repeat;

        z-index:9999;
    }
    
    a.disabled { color:gray!important; }
    
    a.disabled:hover
    {
        background-color: transparent!important;
    }
    
    a.disabled:focus
    {
        background-color: transparent!important;
    }
    
    .pager-container
    {
        padding-bottom: 35px;
        padding-top: 5px;
        z-index:10;
    }
    
.pagination
{
    margin:0;
    font-family:proxima-nova;
    float:right;
}

.pager
{
    margin:0;
}
    .page_link
    {
        color:#6c0066!important;
        border-radius:0!important;
    }
    
    
    .page_link.active
    {
        
        background-color:#e1cce0!important;
    }
    
    .item {
  min-height:425px;
}

    
    .custom-pager-selected
    {
        background-color:#eee;
    }
.glyphicon { margin-right:5px; }
.thumbnail
{
    margin-bottom: 50%;
    padding: 0px;
    -webkit-border-radius: 0px;
    -moz-border-radius: 0px;
    border-radius: 0px;
}

.item.list-group-item
{
    float: none;
    width: 100%;
    background-color: #fff;
    margin-bottom: 10px;
}
.item.list-group-item:nth-of-type(odd):hover,.item.list-group-item:hover
{
   
}

.item.list-group-item .list-group-image
{
    margin-right: 10px;
}
.item.list-group-item .thumbnail
{
    margin-bottom: 0px;
}
.caption
{
        font-family: proxima-nova;
    letter-spacing: 1px;
}
.item.list-group-item .caption
{

    padding: 9px 9px 0px 9px;
}
.item.list-group-item:nth-of-type(odd)
{
   
}

.item.list-group-item:before, .item.list-group-item:after
{
    display: table;
    content: " ";
}

.item.list-group-item img
{
    float: left;
}
.item.list-group-item:after
{
    clear: both;
}
.list-group-item-text
{
    margin: 0 0 11px;
}

.custom-product-title
{
    clear:both;
    text-align:center;
    font-size:16px;

}

.product-description-anchor
{
    color:Black!important;
}

.price-title-container
{
    text-align:center;
}

.price-title
{
    font-size:16px;
    font-weight:bold;
    text-align:center;
    display:inline;
    
}

.thumbnail
{
    border:none;
    
}

.list-group-image
{
    max-height:550px;
}

.product-number-container
{
    text-align:center;
}
.product-number
{
    height:auto;
    margin:0;
    font-size:16px;
    text-align:center;
    display:inline;
}

.product-description
{
    height:55px;
}

#loading-indicator {
  position: absolute;
  left: 10px;
  top: 10px;
  height:500px;
  background-color:Yellow;
}

.addToCartBtn
{
       line-height: 1.4em!important;
       font-size:16px!important;
    background-color: #6C0066!important;
    border-color: #6C0066!important;
    color: #fff!important;
}

.product-border
{

}

.text-header
{
    color: black;
    font-size: 32px;
    font-style: normal;
    font-weight: 700;
    letter-spacing: 6px;
    text-align: center;
    text-transform: uppercase;
    margin-bottom:20px;
}

.imageDiv
{
    max-width:500px;
    position:relative;
    margin-left:auto;
    margin-right:auto;
}

.imageDiv > .img-responsive
{
    display:inline-block!important;
}

.bubble-comment
{
    color:Red;
}

.bubbleImageDiv
{
    display:block;
    position:absolute;
    right:-20;
    top:-20;

}
.bubbleImageDiv img
{
    max-width:125px;
}

@media (max-width: 1200px) 
{
.product-number
{

}
}

@media (max-width: 768px)
{
    
    .text-header
    {
        font-size:16px;
    }
 
 .list-group-item
 {
     text-align:center;
 }
 
 .product-number
{
}

.item.list-group-item .thumbnail
{
    display:inline-block;
}

.item.list-group-item .list-group-image
{
    margin-right:auto;
}

.item.list-group-item img
{
    float:none;
}
    
}

</style>

<div class="container-fluid custom-container">
<h2 class="text-header">
<asp:Label ID="CategoryNameLabel" runat="server"></asp:Label>
</h2>
<img src="../../Images/loading.gif" id="loading-indicator" style="display:none" /><img />
    <div class="col-md-12 col-xs-12 pager-container">
        <div class="sync-pagination pagination pagination-lg"">
        <ul class="pager">
        </ul>
    </div>
    </div>
    <asp:PlaceHolder ID="ProductsPlaceHolder" runat="server"></asp:PlaceHolder>
        <div class="col-md-12 col-xs-12 pager-container">
    <div class="sync-pagination pagination pagination-lg">
        <ul class="pager">
        </ul>
    </div>
    </div>
    <br />
    <a href="#">Back To Top</a>
    <div style="display:none;">
    <input ID="pageNumberHiddenField" type="text" />
    </div>
</div>
<div>
<asp:DataGrid ID="dg2" runat="server"></asp:DataGrid>
</div>
