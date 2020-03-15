<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessMenu.ascx.cs" 
Inherits="JJPro.Web.Business.Controls.ResponsiveControls.BusinessMenu" %>
<div class="old-header">
<div id="consultant-info">
    <asp:Label ID="HeaderConsultantName" runat="server" CssClass="HeaderName"></asp:Label>
    <br />
    <asp:Label ID="HeaderConsultantTitle" runat="server" CssClass="HeaderName"></asp:Label>
    <br />
    <asp:Label ID="HeaderConsultantID" runat="server" CssClass="HeaderConsultantID"></asp:Label>
    <br />
    <a href="#gotoTotals">
        <asp:Label ID="MyTotalsLabel" runat="server" Text="My Totals" CssClass="HeaderMyTotals"></asp:Label>
    </a>
</div>
<div id="jj-logo">
    <a href="/Biz/Default.aspx">
        <asp:Image ID="LogoImage" runat="server" ImageUrl="http://8f2270e13c3e0703baa2-6c2aaf386ecfc7423d48bccd1a997322.r2.cf1.rackcdn.com/Logos/JJ-Purple-cropped.png"
            Height="96px" Width="150px" />
    </a>
</div>
<asp:Panel ID="pnlConsData" runat="server">
<div id="consultant-data">
    <div id="consultant-data-float">
        <div id="notification">
        </div>
        <div id="cart-logout">
            <div id="consultant-buttons">
                <div class="consultant_buttons_float">
                    <telerik:RadButton ID="LandingCartButton" ButtonType="ToggleButton" runat="server"
                        Text="Cart" CssClass="consultant_buttons">
                    </telerik:RadButton>
                </div>
                <div class="consultant_buttons_float">
                    <span class="consultant_buttons_divider">|</span>
                </div>
                <div class="consultant_buttons_float">
                    <telerik:RadButton ID="LandingLogOutButton" ButtonType="ToggleButton" runat="server"
                        Text="Logout" CssClass="consultant_buttons">
                    </telerik:RadButton>
                </div>
            </div>
            <div class="clear-fix"></div>
            <div id="current-order">
                <JJ:OrderSelect ID="LandingOrderSelect" runat="server" CurrentViewType="SimpleView"
                    TextColor="#B5B5B5" />
            </div>
        </div>
    </div>
</div>
</asp:Panel>
<div class="clear-fix">
</div>
</div>
<nav class="navbar navbar-default bizMenu" style="">
  <div class="container-fluid">
    <!-- Brand and toggle get grouped for better mobile display -->
    <div class="navbar-header">
      <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
        <span class="sr-only">Toggle navigation</span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </button>
      <a class="navbar-brand" href="/Business/"><img height="30px" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/HostessParty/Just%20Jewelry%20Logo%20alternative-White.png" /></a>
    </div>

    <!-- Collect the nav links, forms, and other content for toggling -->
    <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
      <ul class="nav navbar-nav bizNavBar" runat="server" id="bizDropDown">
        <li class="dropdown">
          <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Dropdown <span class="caret"></span></a>
          <ul class="dropdown-menu">
            <li><a href="#">Action</a></li>
            <li><a href="#">Another action</a></li>
            <li><a href="#">Something else here</a></li>
            <li role="separator" class="divider"></li>
            <li><a href="#">Separated link</a></li>
            <li role="separator" class="divider"></li>
            <li><a href="#">One more separated link</a></li>
          </ul>
        </li>
      </ul>
    </div><!-- /.navbar-collapse -->
  </div><!-- /.container-fluid -->
</nav>

<asp:DataGrid ID="dg" runat="server" Visible="false">

</asp:DataGrid>