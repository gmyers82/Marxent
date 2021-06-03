<%@ Page Title="" Language="C#" MasterPageFile="~/Business/BusinessResponsive.Master"
    AutoEventWireup="true" CodeBehind="FashionWeekend.aspx.cs" Inherits="JJPro.Web.Business.NationalConference.FashionWeekend" %>

<asp:Content ContentPlaceHolderID="head" runat="server" ID="head">
        <link href="../../Scripts/agency/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet"
        type="text/css" />
            <link href="../../Scripts/agency/vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet"
        type="text/css">
        <link href="../../Scripts/agency/css/agency.min.css" rel="stylesheet" type="text/css" />
            <link rel="stylesheet" href="https://use.typekit.net/xzw6lod.css">
    <script type="text/javascript">        try { Typekit.load(); } catch (e) { }</script>
<style>

        header.masthead
        {
            text-align:inherit!important;
        }
        .video-container
        {
            position: absolute;
            top: 0%;
            left: 0%;
            height: 100%;
            width: 100%;
            overflow: hidden; 
        }
        
        video
        {
            width: 100%;
        }
        
        .video-container video
        {
            position: absolute;
            z-index: -1;
            opacity: 0.78;
            width: 100%;
            min-height: 500px;
        }
        
        h3
        {
            z-index: 1;
            text-align: center;
            margin-left: auto;
            margin-right: auto;
            margin-top: 10%;
            padding: 10px;
            letter-spacing: 3px;
            font-size: 25px;
        }
        
        header.masthead .intro-text
        {
            text-align:center!important;
            color:#3D3D3D;
        }
        

        .btn-xl
        {
            padding:20px 40px !important;
            background-color: #3D3D3D!important;
            border-color: #3D3D3D!important;
            border-radius: .25rem!important;
            color:White!important;
            font-size:18px!important;
            z-index: 1;
            position:relative;

        }
        
        .section-divider hr
        {
            margin-right: 10%;
            margin-left: 10%;
            margin-top: 3%;
            margin-bottom: 3%;
        }
        
        .conference-top-info
        {
            text-align:center;
            font-weight:bold;
            font-size:36px;
            color:Black;
            margin-top:2%;
        }
        
        .conference-location
        {

            text-align:center;
            font-weight:bold;
            font-size:20px;
            color:#545454;
            margin-top:2%;
            
        }
        
        .conference-section
        {
            margin-top:2%;
            color:Black;
            text-align:center;
            font-family:din-condensed;
            margin-left: 15%;
            margin-right: 15%;
            flex: auto;
            
        }
        
        .conference-section h3
        {
            font-size:30px!important;
            text-transform:uppercase;
        }
        
        .conference-paragraph
        {
            font-family:proxima-nova;
            font-size:19px;
        }
        
        .about-section h3
        {
            
          margin-left:10%;
          margin-right:10%;  
          margin-top:0;  
        }
        
        .agenda-button
        {
            margin-top:2%;
        }
        
        .consultant-name
        {
            font-family:din-condensed;
            font-size:30px;
        }
        
        .consultant-title
        {
            font-family:proxima-nova;
             
        }
        
        .giant-quotes
        {
            font-family:scriptorama-markdown-jf;
            font-size:24px;
        }
        
        .consultant-quote
        {
            font-family: proxima-nova;
            font-size: 18px !important;
            line-height:26px;
        }
        
        .portfolio-item
        {
            padding-top:15px!important;
            padding-bottom:15px!important;
        }
        
        .logo-img
        {

        }
        
        .things-to-do-place
        {
           font-size:20px; 
        }
        
        .things-to-do-type
        {
           font-size:14px; 
        }
        
        .ticketPurchaseHeading
        {
        font-size: 60px;
        font-weight: 700;
        font-family: cantoni-pro;
        text-align:center;
        }
        
        .ticket-quantity-span
        {
            font-size:14px;
        }
        
        .buy-tickets-top
        {
            font-size:16px;
        }
        
        .buy-ticket-btn
        {
            padding:5px 20px !important;
            background-color: #3D3D3D!important;
            border-color: #3D3D3D!important;
            border-radius: .25rem!important;
            color:White!important;
            font-size:16px!important;
            z-index: 1;
            position:relative;
        }
        
        .notice
        {
            font-style:italic;
            font-size:12px;
        }

    </style>



</asp:Content>
<asp:Content ID="Header" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-top">
        <!-- Header -->
        <div class="header-container">
            <nav class="navbar navbar-expand-lg navbar-dark fixed-top" id="mainNav">
                <div class="container">
                    <a class="navbar-brand js-scroll-trigger" href="/Business"><img class="img-responsive logo-img" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Just%20Jewelry%20logo.png" alt="Just Jewelry" /></a>
                    <button class="navbar-toggler navbar-toggler-right" type="button" data-toggle="collapse"
                        data-target="#navbarResponsive" aria-controls="navbarResponsive" aria-expanded="false"
                        aria-label="Toggle navigation">
                        Menu <i class="fa fa-bars"></i>
                    </button>
                    <div class="collapse navbar-collapse" id="navbarResponsive">
                        <ul class="navbar-nav text-uppercase ml-auto">
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" href="#page-top">Home</a>
                            </li>
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" href="#about">About</a>
                            </li>
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" href="#gallery">Gallery</a>
                            </li>
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" href="#hotel">Hotel</a>
                            </li>
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" href="#thingstodo">Things To Do</a>
                            </li>
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" href="#nationalachievers">National Achievers</a>
                            </li>
                            <li class="nav-item"><a class="nav-link js-scroll-trigger" style="font-family:source-sans-pro; cursor:pointer;" data-toggle="modal" data-target="#PurchaseTicketModal">Buy Tickets</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

        </div>
        <!-- Header -->
        <header class="masthead">
            <div class="container">
                <div class="video-container">
                    <video id="video-background" preload muted autoplay loop>
                        <source src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/2017_conference_loop.mp4"
                            type="video/mp4">
                        <source src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/2017_conference_loop.ogv"
                            type="video/ogg">
                        <source src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/2017_conference_loop.webm"
                            type="video/webm">
                    </video>
                </div>
                <div class="intro-text">
                    <div class="intro-lead-in">
                        Just Jewelry's</div>
                    <div class="intro-heading">
                        Fashion Weekend 2018</div>
                    <div class="intro-heading text-uppercase">
                    </div>
                    <a class="btn btn-primary btn-xl text-uppercase js-scroll-trigger" data-toggle="modal" data-target="#PurchaseTicketModal">
                        Buy Tickets</a>
                </div>
            </div>
            <div class="conference-top-info">
            <span>July 27th-28th 2018</span>
            <br />
            <span class="conference-location">Hilton Garden Inn - Miamisburg, Ohio</span>
            </div>
        </header>
    </div>
    <div class="col-12 section-divider">
    <hr />
    </div>
    <section id="about">
    <div class="row">
            <div class="col-lg-12 text-center">
                <h2 class="section-heading text-uppercase">
                    About</h2>
            </div>
            <div class="conference-section about-section section-subheading text-muted col-12">
                <h3>Fashion Weekend is this July and you won’t want to miss your opportunity to be on the cutting edge of what’s trending this fall and winter.</h3>
                <br />
                <span class="conference-paragraph">Just Jewelry’s Fashion Weekend is an event that YOU
                    AND YOUR PROSPECTS do not want to miss! You’ll see all the fashion trends for the
                    season ahead and learn all the details for styling your customers from head to toe.
                    You’ll be empowered to serve your customers with the fashion know-how of a true
                    fashionista. Learn great ways to change a look from basic to basically-WOW, and
                    how to turn your customer into your Hostess or new Recruit. You’ll have time to
                    hang out with your fellow Style-Sisters and strut the runway wearing your favorite
                    creative look for all to admire. We’ll celebrate successes and cheer for our new
                    team members all while receiving gifts and winning prizes! You’ll be challenged
                    to style your life with excitement and creativity as you take your business off
                    the runway and to your customers’ front door. Fashion Weekend is your and your prospects
                    only opportunity this year to meet with the Founders and see the entire new collection
                    in person, all while making new friends and growing your business. Just Jewelry’s
                    Conference activities begin on Friday at 3:00 and won't stop until you do on Saturday
                    night! It's time to take the runway and get all the areas of your life in style.
                    Get the details and find out all the benefits of attending by clicking the menu
                    options above. We can't wait to see you July 27th and 28th at Just Jewelry's Fashion
                    Weekend Style Your Life Conference! </span>
            </div>

            <div class="col-lg-12 text-center agenda-div">
                <button type="button" class="btn btn-primary btn-xl text-uppercase js-scroll-trigger agenda-button"
                    data-toggle="modal" data-target="#exampleModal">
                    View Weekend Agenda
                </button>
            </div>
        </div>
    </section>
        <div class="col-12 section-divider">
    <hr />
    </div>
<%--    <section id="agenda">
        <div class="row">
            <div class="col-lg-12 text-center">
                <h2 class="section-heading text-uppercase">
                    Agenda</h2>
            </div>
        </div>

    </section>--%>
    <section id="gallery">
        <div class="container">
            <div class="row">
                <div class="col-lg-12 text-center">
                    <h2 class="section-heading text-uppercase">
                        Gallery</h2>
                </div>
            </div>
            <div class="row">
                        <div class="col-lg-12 text-center agenda-div">
                <button type="button" class="btn btn-primary btn-xl text-uppercase js-scroll-trigger agenda-button"
                    data-toggle="modal" data-target="#videoModal">
                    Watch Video
                </button>
            </div>
            </div>
            <div class="row">
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="galleryModal" href="#notused">

                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/1.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/2.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/3.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">

                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/4.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/5.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/6.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">

                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/7.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/8.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/9.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">

                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/10.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/11.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/12.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">

                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/13.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/14.jpg" /></a>
                </div>
                <div class="col-md-4 col-sm-6 portfolio-item">
                    <a class="portfolio-link galleryImage" data-toggle="modal" href="#notused">
                        <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/GalleryImages/15.jpg" /></a>
                </div>
            </div>
        </div>
    </section>
        <div class="col-12 section-divider">
    <hr />
    </div>
    <section id="hotel">
        <div class="row">
            <div class="col-lg-12 text-center">
                <h2 class="section-heading text-uppercase">
                    Hotel</h2>
            </div>
            <div class="col-12">
                <img class="img-responsive" width="100%" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Hotel/HOTEL%20IMAGE.jpg" />
            </div>
            <div class="col-12 conference-section section-subheading text-muted">
                <h3>
                    The Hilton Garden Inn Dayton South/ Austin Landing</h3>
                <p class="conference-paragraph">
                    A block of hotel rooms are reserved especially for Just Jewelry Consultants! <a href="https://secure3.hilton.com/en_US/gi/reservation/book.htm?inputModule=HOTEL&ctyhocn=DAYDSGI&spec_plan=JJM&arrival=20180725&departure=20180729&cid=OM,WW,HILTONLINK,EN,DirectLink&fromId=HILTONLINKDIRECT">Click
                    here. Use group code JJM</a> to book your reservation. Space is limited, so we recommend that you book in
                    advance! Find everything you need for work and relaxation in our inviting hotel
                    in Dayton, OH. Start your day right with a cooked-to-order breakfast in The Garden
                    Grille & Bar. Gather with colleagues for a relaxing night cap in the bar. Work out
                    your way – at the fitness center, in the heated indoor pool or with a complimentary
                    Stay Fit Kit® in your guest room or suite. Keep in touch with complimentary high-speed
                    internet access in your guest room or with complimentary WiFi in all public areas
                    of our Dayton, OH hotel and use the business center to stay connected on the road.
                    At the end of a hectic day, enjoy a sound night's rest in the adjustable Garden
                    Sleep System bed®.
                </p>
            </div>
        </div>
    </section>
        <div class="col-12 section-divider">
    <hr />
    </div>
    <section id="thingstodo">
    <div class="col-12 text-center">
                    <h2 class="section-heading text-uppercase">
                    Things To Do</h2>
                    </div>
    <div class="container">
        <div class="row">
            <div class="col-md-4 col-sm-6 portfolio-item">
                <a class="portfolio-link" target="_blank" href="http://austinlanding.com/">
                    <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Places/Austin%20Landing.JPG" /></a>
                <div class="portfolio-caption">
                    <h4 class="things-to-do-place">
                        Austin Landing</h4>
                    <p class="things-to-do-type text-muted">
                        Food/Shopping</p>
                </div>
            </div>
            <div class="col-md-4 col-sm-6 portfolio-item">
                <a class="portfolio-link" target="_blank" href="https://www.metroparks.org/places-to-go/cox-arboretum//">
                    <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Places/Cox-Arboretum-Rock-Gardens.jpg" /></a>
                <div class="portfolio-caption">
                    <h4 class="things-to-do-place">
                        Cox Arboretum</h4>
                    <p class="things-to-do-type text-muted">
                        Nature Park</p>
                </div>
            </div>
            <div class="col-md-4 col-sm-6 portfolio-item">
                <a class="portfolio-link" target="_blank" href="http://www.nationalmuseum.af.mil/">
                    <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Places/US-Airforce.png" /></a>
                <div class="portfolio-caption">
                    <h4 class="things-to-do-place">
                        National US Airforce Museum</h4>
                    <p class="things-to-do-type text-muted">
                        Historic Museum</p>
                </div>
            </div>
            <div class="col-md-4 col-sm-6 portfolio-item">
                <a class="portfolio-link" target="_blank" href="https://www.ikea.com/us/en/store/west_chester">
                    <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Places/IKEA.jpg" /></a>
                <div class="portfolio-caption">
                    <h4 class="things-to-do-place">
                        IKEA</h4>
                    <p class="things-to-do-type text-muted">
                        Shopping</p>
                </div>
            </div>
            <div class="col-md-4 col-sm-6 portfolio-item">
                <a class="portfolio-link" target="_blank" href="http://www.daytonartinstitute.org/">
                    <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Places/Dayton-Art-Institute.jpg" /></a>
                <div class="portfolio-caption">
                    <h4 class="things-to-do-place">
                        Dayton Art Institute</h4>
                    <p class="things-to-do-type text-muted">
                        Art Museum</p>
                </div>
            </div>
            <div class="col-md-4 col-sm-6 portfolio-item">
                <a class="portfolio-link" target="_blank" href="http://www.premiumoutlets.com/outlet/cincinnati">
                    <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/Places/cincinnati-premium-outlets.jpg" /></a>
                <div class="portfolio-caption">
                    <h4 class="things-to-do-place">
                        Cincinnati Premium Outlets</h4>
                    <p class="things-to-do-type text-muted">
                       Shopping</p>
                </div>
            </div>
        </div>
        </div>
    </section>
        <div class="col-12 section-divider">
    <hr />
    </div>
    <section id="nationalachievers">
        <div id="portfolio" class="bg-light">
            <div class="container">
                <div class="row">
                    <div class="col-lg-12 text-center">
                        <h2 class="section-heading text-uppercase">
                            National Acheivers</h2>
                        <h3 class="section-subheading text-muted">
                            Consultants at ALL levels have the opportunity to earn our prestigious National Achievers Award. This annual award is given to any Consultant who recruits 15 or more new Qualified Consultants from July 1 through June 30 of the given year. This award is also given to any Consultant who has $15,000 or more qualifying volume (QV) from July 1 through June 30 of the given year. Just Jewelry will be hosting a special gala in their honor Annual Conference weekend! Our glamorous Shining Star National Achievers Gala will be held on Friday, July 27, 2018. This gala is by invitation only for those Consultants who have met the criteria for the National Achievers Club. <br /></h3>
                        <h3 class="conference-location"> Introducing Just Jewery's 2017-2018 National Acheivers. Click on their image to learn more about them!</h3>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#PamelaAnderson">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Pamela%20Anderson.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Pamela Anderson</h4>
                            <p class="text-muted consultant-title">
                                Regional Sales Coordinator</p>
                        </div>
                    </div>
                    <%--Pamela Anderson--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#BrendaBoozer">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Brenda%20Boozer.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Brenda Boozer</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Brenda Boozer--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#LoriDore">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Lori%20Dore.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Lori Dore</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Lori Dore--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#DebbyForehand">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Debby%20Forehand.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Debby Forehand</h4>
                            <p class="text-muted consultant-title">
                                Team Leader</p>
                        </div>
                    </div>
                    <%--Debby Forehand--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#TammyHaines">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Tammy%20Haines.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Tammy Haines</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Tammy Haines--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#BobbiJoHeinze">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Bobbi%20Jo%20Heinze.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Bobbi Jo Heinze</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--BobbiJo Heinze--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#JillJones">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/JillJones.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Jill Jones</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Jill Jones--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#keriKazebeer">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Keri%20Kazebeer.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Keri Kazebeer</h4>
                            <p class="text-muted consultant-title">
                                Team Leader</p>
                        </div>
                    </div>
                    <%--keri Kazebeer--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#AmyLemer">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/amy%20lemer.JPG"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Amy Lemer</h4>
                            <p class="text-muted consultant-title">
                                Senior Team Leader</p>
                        </div>
                    </div>
                    <%--Amy Lemer--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#ConnieMcallister">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Connie%20MCALLISTER.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Connie McAllister</h4>
                            <p class="text-muted consultant-title">
                                Recruiting Consultant</p>
                        </div>
                    </div>
                    <%--Connie Mcallister--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#courtneyPerkins">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Courtney%20Perkins.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Courtney Perkins</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Courtney Perkins--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#LisaPropps">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Lisa%20Propps.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Lisa Propps</h4>
                            <p class="text-muted consultant-title">
                                Team Leader</p>
                        </div>
                    </div>
                    <%--Lisa Propps--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#JesicaRice">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Jesica%20Rice.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Jesica Rice</h4>
                            <p class="text-muted consultant-title">
                                Consultant</p>
                        </div>
                    </div>
                    <%--Jesica Rice--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#ChristineRiffel">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Christine%20Riffel.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Christine Riffel</h4>
                            <p class="text-muted consultant-title">
                                District Coordinator</p>
                        </div>
                    </div>
                    <%--Christine Riffel--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#DonaSmith">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/dona%20smith.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Dona Smith</h4>
                            <p class="text-muted consultant-title">
                                Regional Sales Coordinator</p>
                        </div>
                    </div>
                    <%--Dona Smith--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#LeaAnnTaylor">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Lea%20Ann%20Taylor%20%281%29.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Lea Ann Taylor</h4>
                            <p class="text-muted consultant-title">
                                District Coordinator</p>
                        </div>
                    </div>
                    <%--LeaAnn Taylor--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#MonicaWieers">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Monica%20Wieers.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Monica Wieers</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Monica Wieers--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#AudraWilburn">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Audra%20Wilburn.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Audra Wilburn</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Audra Wilburn--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#JulieGenson">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/image_place_holder_logo.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Julie Genson</h4>
                            <p class="text-muted consultant-title">
                                Team Leader</p>
                        </div>
                    </div>
                    <%--Sherry Kluth--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#SherryKluth">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/image_place_holder_logo.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Sherry Kluth</h4>
                            <p class="text-muted consultant-title">
                                Consultant</p>
                        </div>
                    </div>
                    <%--Sherry Kluth--%>
                    <div class="col-md-4 col-sm-6 portfolio-item">
                        <a class="portfolio-link" data-toggle="modal" href="#MelindaWells">
                            <div class="portfolio-hover">
                                <div class="portfolio-hover-content">
                                    <%--<i class="fa fa-plus fa-3x"></i>--%> View Bio
                                </div>
                            </div>
                            <img class="img-fluid" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/image_place_holder_logo.jpg"
                                alt="">
                        </a>
                        <div class="portfolio-caption">
                            <h4 class="consultant-name">
                                Melinda Wells</h4>
                            <p class="text-muted consultant-title">
                                Senior Consultant</p>
                        </div>
                    </div>
                    <%--Melinda Wells--%>
                </div>
            </div>
        </div>
    </section>
    <section id="pageFooter">
    <h3 class="conference-location"> Just Jewelry's Fashion Weekend 2018 | <a style="color:#545454!important;" href="/Business">Go back to Business Portal</a></h3>
    </section>
        <div class="portfolio-modal modal fade" id="courtneyPerkins" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Courtney Perkins</h2>
                                <p class="item-intro text-muted">
                                    Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Courtney%20Perkins.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Courtney Perkins was born and raised in Macon, MO.  She earned both a B.S. and a M.A. in Criminal Justice and has been working in the field of Child Abuse and Neglect Investigations for 15 years.  Courtney has been married for 8 years and has 3 beagles, a golden retriever and a cat.  Her favorite tradition is getting together with her family on Christmas Eve.  Her favorite movie is Love & Basketball and, after being coerced by her nieces, just finished a 3 month binge of watching 13 seasons of Grey’s Anatomy!  Her favorite food is cheese and she hopes to some day visit Bora Bora.  She loves the opportunity Just Jewelry has given her to create her own business and be her own boss all while building relationships with her amazing customers and fellow Consultants. 
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="keriKazebeer" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Keri Kazebeer</h2>
                                <p class="item-intro text-muted">
                                    Team Leader</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Keri%20Kazebeer.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Keri Kazebeer was born in Ontario, California.  She majored in Diagnostic Imaging at Gulf Coast State College and Accounting at the University of Maryland.  She has been married for almost 17 years and has 2 sons and 2 dogs.  She loves Saturday movie nights with her boys curled up on the couch under the blankets sharing snacks.  Her favorite book is The Alchemist and her favorite movie is There’s Something About Mary.  Keri loves cupcakes so much, she has used the icon as part of her business logo.  She even named her mannequin Cupcake!  Someday Keri hopes to visit the Galapagos Islands.  Keri loves that JJ gives back a portion of the proceeds to charity.  She also loves the way the clothing makes her customers feel.  To see women feel beautiful and light up with confidence is so rewarding to her.  She also enjoys watching her team members grow their businesses and fulfill their dreams.  More than anything though, Keri loves all the opportunities her JJ business has given her to share the love of Jesus to others. 
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
<div class="portfolio-modal modal fade" id="LoriDore" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here --> 
                                <h2 class="text-uppercase">
                                    Lori Dore</h2>
                                <p class="item-intro text-muted">
                                    Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Lori%20Dore.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Lori Dore was adopted at just 3 months old and raised in Sidney, Montana.  She attended Dickinson State College in Dakota for nursing and has been a registered nurse for 38 years.  Lori has been married for an incredible 41 years!  She has 3 children and 4 grandchildren. Lori lives on a farm and has a large variety of animals.  Lori loves the movie White Christmas.  Her favorite tradition since she was a little girl is enjoying a family Christmas prime rib dinner.  Her favorite food is lobster and she hopes to one day visit France.  Lori loves how Just Jewelry has enabled her to do something totally different from nursing.  She has met so many wonderful people, made lifelong friendships and loves that she is helping women be the BEST that they can be through Just Jewelry!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="PamelaAnderson" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Pamela Anderson</h2>
                                <p class="item-intro text-muted">
                                    Regional Sales Coordinator</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Pamela%20Anderson.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Pamela Anderson was born in Oklahoma, but raised in Garden City, Kansas.  She majored in Business and Computer Information Science at Kansas State University.  She has been married to Mike for 12 ½ years and has 2 sons and 3 grandsons.  Her favorite family tradition is baking Christmas cookies while listening to Christmas music.  Her favorite movie is Facing the Giants and her favorite book is The Bible.  She enjoys Mexican food and would love to someday visit Italy.  She loves being a Just Jewelry leader because it gives her the opportunity to watch her team members succeed.  She finds tremendous joy celebrating their accomplishments with them.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
<div class="portfolio-modal modal fade" id="ConnieMcallister" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Connie McAllister</h2>
                                <p class="item-intro text-muted">
                                    Regional Sales Coordinator</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Connie%20MCALLISTER.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Connie McAllister was born and raised in Williamsport, Maryland.  She graduated from beauty college, opened her own salon in her home in 1981 and is still running it today after 42 years.  Connie added JJ to her home business 2 years ago and her customers love it!  Connie has been married for an amazing 40 years and has 2 grown children.  Connie loves Just Jewelry’s mission.  She also loves how JJ has made her home beauty business even more enjoyable.  Connie has gained a wonderful, trendy wardrobe while adding to her financial success!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
<div class="portfolio-modal modal fade" id="ChristineRiffel" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Christine Riffel</h2>
                                <p class="item-intro text-muted">
                                    District Coordinator</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Christine%20Riffel.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Christine Riffel was born and raised in Stockton, Kansas.  She has a Paralegal Degree from Washburn University.  She lives with her short-haried black cat Kiana.  Her favorite tradition is spending Christmas Eve with her family.  She loves any Nicholas Sparks book and enjoys comfort food.  Christine would love to visit Alaska and Europe someday. She loves the trendy clothing and accessories Just Jewelry offers and that she is able to be her own boss.  As a leader, Christine loves being able to share ideas and information with her team in hopes of inspiring them to climb the Ladder of Dreams.  She believes their success is her success and she loves watching what they do with their own business!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="AudraWilburn" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Audra Wilburn</h2>
                                <p class="item-intro text-muted">
                                    Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Audra%20Wilburn.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Audra Wilburn was born and raised in Brookfield, Missouri.  She graduated from the University of Missouri with a Doctorate in Pharmacy.  Audra got engaged on the Just Jewelry Incentive Trip in 2017 and will be getting married this September!  She has 2 cats, Lola and Leah.  Audra has a tradition of chanting “Hey, Hey, It’s the 1st of May.  Outdoor swimming begins today,” as she takes that first plunge into the pool on May 1st.  Her favorite movie is Twister and she is currently re-watching Scandal from beginning to end.  Audra’s favorite food is tacos and she would love to visit Europe and Italy someday.  Audra loves being able to offer affordable, stylish clothing and accessories to the women in her small town.  She also loves that JJ allows each Consultant to run their business how it works best for them!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="BobbiJoHeinze" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Bobbie Jo Heinze</h2>
                                <p class="item-intro text-muted">
                                    Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Bobbi%20Jo%20Heinze.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Bobbi Jo Heinze was born and raised in North Dakota.  She has been married for 31 years and has three daughters (1 who is their guardian angel), two grandchildren and is expecting her third grandchild this year.  Bobbi Jo also has a Chiweenie dog.  She enjoys Christmas time because she gets a chance to see her grandkids faces as they open gifts.  They get so excited over the simplest things and she loves that!  She loves the movie Finding Forrester and enjoys watching Flea Market Flip.  Her favorite food is chips and salsa and she hopes to visit Italy someday.  She loves the super cute jewelry and clothing that she is able to offer her customers through JJ.  She also loves that she can work at her own pace with no pressure.  Bobbi Jo loves the long lasting friendships she has developed with other Consultants from all over the US as well.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="MonicaWieers" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Monica Wieers</h2>
                                <p class="item-intro text-muted">
                                    Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Monica%20Wieers.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Monica Wieers was born and raised on a dairy farm near Fertile, Minnesota.  At the age of 42 she decided to go to college in Grand Forks, North Dakota and received her degree as a Surgical Technologist.  Monica has three children and a ½ Shizu and ½ Bichon Frise dog that is a total diva!  Her favorite family tradition is getting together to make lefse.  Monica would love to visit Norway or Sweden someday.  She loves everything about Just Jewelry & JJ Boutique including the clothing line, interacting with people and seeing how excited customers get about the products.  She loves emailing customers about new products and she has a great time doing vendor shows.  As a leader, Monica loves the support and advice she shares with her team.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="JesicaRice" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Jesica Rice</h2>
                                <p class="item-intro text-muted">
                                   Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Jesica%20Rice.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Jesica Rice was born in raised in Kansas.  She received a Criminal Justice Degree from Hutchinson Community College.  She has been married for over 8 years and has 7 children and 1 grandaughter.  She has a pet Plecostomus named Squidward.  Jesica has a large family, so time all together is extra special.  Therefore, a tradition they try to uphold each week is a family Sunday dinner.  Jesica loves the movie It’s a Wonderful Life and Forrest Gump.  Her current Netflix binge is Bates Motel.  When it comes to food, it’s a toss up for Jesica between leftover spaghetti for breakfast or a bowl of Frosted Flakes any time of day.  Jesica would enjoy the opportunity to visit Ireland someday.  She has made so many new friends in the past year through her Just Jewelry business and loves to wear the clothing!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%> 
    <div class="portfolio-modal modal fade" id="BrendaBoozer" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Brenda Boozer</h2>
                                <p class="item-intro text-muted">
                                   Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Brenda%20Boozer.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Brenda Boozer is from Bristol, Tennessee.  She loves sharing the holidays with her blended family consisting of 3 children, 7 grandchildren and a new great-grandbaby!  She also has a 16 year old Min Pin and a 26 year old parrot.  Brenda loves salmon, prime rib and hot glazed Krispy Kreme doughnuts.  Her favorite movie is Steele Magnolias and she hopes to someday visit Italy.  Brenda is inspired by the fact that Just Jewelry is a female founded and run company that empowers other women by offering them incredible opportunities while giving back to five missions.  She is amazed by the sisterhood and feels that the lifelong friendships she has made are truly a gift.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="LeaAnnTaylor" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Lea Ann Taylor</h2>
                                <p class="item-intro text-muted">
                                  District Coordinator</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Lea%20Ann%20Taylor%20%281%29.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Lea Ann Taylor was born and raised in Baton Rouge, Louisiana.  She graduated from Drury University with a Bachelor’s in Psychology.  She has been married for almost 16 years and has 2 daughters.  Her family also includes a 200 pound English Mastiff, a 6 pound Chihuahua and 2 rescue cats.  She loves to travel with her family and is looking forward to taking her daughters on their first cruise this summer.  If she could visit anywhere, Lea Ann would go to Italy.  She loves Mexican food and chips and salsa are definitely her weakness!  Lea Ann loves all the friendships she has made through her JJ business.  She loves that she can take her business anywhere with her and that her business allows her to help others around the world.  As a leader, Lea Ann enjoys watching others succeed and knowing she played a small part in their success.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="DebbyForehand" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Debby Forehand</h2>
                                <p class="item-intro text-muted">
                                   Team Leader</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Debby%20Forehand.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Debby Forehand was raised in Norfolk, Virginia.  She majored in Business Education from Old Dominion University.  She has been married for 15 years and has 1 son and 2 grandchildren and an 11 year old Maltese.  Her favorite movie is The Wizard of Oz and she is currently into Grace & Frankie on Netflix.  Brenda hopes to one day visit Australia.  She loves all the wonderful people she has met through her JJ business and loves that as a leader she often gets first-hand information about the company, launches and opportunities.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="AmyLemer" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Amy Lemer</h2>
                                <p class="item-intro text-muted">
                                   Senior Team Leader</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/amy%20lemer.JPG"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Amy Lemer was born and raised in Kansas.  Amy is a registered nurse and a certified Emergency Medical Intensive Care Technician.  She has been married for 9 years and has three children ranging in age from 25 down to 6.  She is currently also taking care of her daughters goldfish Finley.  Amy enjoys watching My Little Pony Movie with her youngest child.  She is currently watching Miraculous: Tales of Ladybug & Cat Noir on Netflix.  Amy loves anything with pasta in it.  Amy was an exchange student when she was just 12 years old in Japan and would love to return someday for a visit.  Amy loves how giving Just Jewelry is to Consultants as well as the community.  She believes that the quality and the price point of the products make it easy to sell.  Amy also loves being a leader and enabling others to
be their own boss, set their own hours and not have the pressure of quotas other direct selling companies have. 
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="LisaPropps" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Lisa Propps</h2>
                                <p class="item-intro text-muted">
                                   Team Leader</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Lisa%20Propps.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Lisa Propps was born and raised in Topeka, Kansas.  She graduated from Washburn University with a BA in Sociology and a minor in Criminal Justice.  She has been married for 24 years and has 2 children and a dog.  Her favorite family tradition is their yearly Championship family mini-golf game.  She loves any book by James Patterson and her favorite movies are Office Space and Legally Blonde.  The last show she watched on Netflix was Ozark.  Lisa’s favorite food is tacos and she hopes to visit Italy one day.  She loves getting Just Jewelry clothing and jewelry at a discount and being a part of the Sisterhood of Consultants.  She appreciates that the owners ask for input and are so encouraging to her.  Although she is a fairly new leader, Lisa feels inspired by her team members’ success stories. She loves hearing about all the positive things JJ has brought to their lives.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>  <%--check--%>
    <div class="portfolio-modal modal fade" id="TammyHaines" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Tammy Haines</h2>
                                <p class="item-intro text-muted">
                                   Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/Tammy%20Haines.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Tammy Haines was born in Pennsylvania and has an Associate’s Degree in Management and a Bachelor’s degree in Accounting from Goldey-Beacom College in Delaware.  She and her husband Marc will be celebrating 12 years of marriage this August.  They have 2 twin 9 year old daughters, 2 puppies and 2 guinea pigs.  Her favorite tradition is enjoying cupcakes and singing Happy Birthday to Jesus at Christmas.  Tammy loves the movies Dumb and Dumber and Ever After.  She enjoys most types of food, but her favorites would probably be Italian and Seafood.  If she could visit anywhere, she would choose to travel to Ireland.  What Tammy loves most about Just Jewelry is the connection she has with women and watching their reaction as they put on clothing or accessories that make them feel beautiful.  
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="DonaSmith" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Dona Smith</h2>
                                <p class="item-intro text-muted">
                                   Regional Sales Coordinator</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/dona%20smith.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Dona Smith was born and raised in Columbia, Kentucky.  She graduated from Georgetown College with a degree in Marketing and Finance.  Dona has one son and she loves traveling to new destinations with him.  She also enjoys spending holidays with her extended family.  Her favorite movie is Big Fish because it reminds her of her dad.  She is currently watching all 13 seasons of Criminal Minds.  Dona enjoys an incredible steak and Graeter’s Ice Cream.  One day she hopes to visit Greece and return to Italy.  Dona loves the camaraderie that she shares with Just Jewelry Consultants. She also loves the variety and quality of our product line and the lifestyle that her business allows her to experience.  As a leader for several years now, she loves sharing her experiences and knowledge with others to help them build not only their business, but to also build their personal abilities and strengths.  She also loves the connections that being a leader allows her to make and the relationships that she has built throughout the years!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="JillJones" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Jill Jones</h2>
                                <p class="item-intro text-muted">
                                   Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/JillJones.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
                                    Jill was born and raised in Bolivar, New York. She attended both Southeastern Academy of Travel and
Oauchita Baptist University. Jill has been married for 23 years and has 4 children ranging in age from 10-
19, 4 dogs and 2 cats. Her favorite family tradition is attending Christmas Eve church service and then
going home to let each family member open just one gift, which are always new pajamas. Jill’s favorite
book is also one of our owners’ favorites, Redeeming Love by Francine Rivers. Jill loves chocolate,
cupcakes and pizza and is currently into watching The Middle on Netflix. One day Jill hopes to have the
opportunity to visit both Hawaii and Africa. Jill loves that JJ has given her a platform to be able to reach
out to women in a way that can make them feel beautiful on the outside while sharing with them the
love of Jesus, which makes them feel beautiful on the inside too! Jill is excited to see her business
growing and she continues to pray that she is able to assist her downlines as they grow their businesses
in a way that will enhance their lives both financially and spiritually.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="MelindaWells" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Melinda Wells</h2>
                                <p class="item-intro text-muted">
                                   Senior Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/image_place_holder_logo.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
Melinda was born and raised in Hays, Kansas. She graduated with a Master of Science in Nursing from
Fort Hays State University. She has been married for 23 years and has 1 son, 2 step-daughters and 4
grandchildren. She also has a 15 year old Miniature Doxy named Princess Maddie. Melinda’s favorite
family tradition is spending Christmas morning with her family; opening Santa presents and enjoying
breakfast together! Melinda is currently watching How to Get Away With Murder on Netflix and her
favorite movie is Nightmare Before Christmas. Her favorite food, hands down, is Mexican and she loves
being with her family wherever she goes! Melinda is excited that Just Jewelry has allowed her to travel
all over Kansas and discover new places while creating new friendships along the way.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="SherryKluth" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Sherry Kluth</h2>
                                <p class="item-intro text-muted">
                                   Consultant</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/image_place_holder_logo.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
Sherry was born and raised in Circle, Montana. She has an Accounting degree from Eastern Montana
College. Sherry has been married for an amazing 29 years and has 1 son and 2 grandchildren. She also
has a black lab and a cat. Her favorite family tradition is Christmas and her favorite book is the Charles
Dickens classic, A Tale of Two Cities. She is currently watching Longmire on Netflix and her favorite
movie is Silverado. Sherry loves her Mom’s potato soup and she hopes to get the opportunity to visit
the colonial states someday. She loves the quality and affordability of the products she is able to offer
working women through her Just Jewelry business. She also feels good knowing that her sales are
supporting a Christian based company that donates to many worthy causes.
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    <div class="portfolio-modal modal fade" id="JulieGenson" tabindex="-1" role="dialog"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="close-modal" data-dismiss="modal">
                    <div class="lr">
                        <div class="rl">
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
                        <div class="col-lg-8 mx-auto">
                            <div class="modal-body">
                                <!-- Project Details Go Here -->
                                <h2 class="text-uppercase">
                                    Julie Genson</h2>
                                <p class="item-intro text-muted">
                                   Team Leader</p>
                                <img class="img-fluid d-block mx-auto" src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/ShiningStarPhotos/image_place_holder_logo.jpg"
                                    alt="">
                                
                                <p class="text-muted consultant-quote ">
                                
Julie Genson was born and raised in Canton, MI.  She attended college for a while, but decided to venture out and start her own cleaning and home organizing business.  She is a single mom with a 20 year old son, 2 chocolate labs and 1 cat.  Julie’s favorite family traditions are going on camping trips, visiting Cedar Point and vacationing in Florida.  Her favorite movies are the X-Men movies and her favorite foods are tomatoes and steaks.  If Julie could visit anywhere in the world, she would choose to travel to the Galapagos Islands.  Julie loves the flexibility that her Just Jewelry business allows her and she loves helping women feel confident through our clothing and accessories.  Being a Just Jewelry leader is so rewarding to her because it has allowed her to guide others towards their business and personal goals.  She also loves the amazing friends she has been able to make along the way!
                                    
                                    </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <%--check--%>
    
    
<!-- Modal -->
<div class="modal fade" id="PurchaseTicketModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
      <div class="modal-content">
          <div class="modal-header">
              <h5 class="modal-title buy-tickets-top" id="H1">
                  Fashion Weekend Tickets <asp:Label ID="TicketPriceLabel" runat="server"></asp:Label></h5>
              <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span>
              </button>
          </div>
          <div class="modal-body ticket-quantity-span">
              <div class="col-12">
                  <h4>
                      Fashion Weekend Perks For Consultants Only</h4>
                  <ul>
                      <li>Register within the first 14 days and get our new Fall/Winter Hostess Item free!</li>
                      <li>Register within the first week and get $50 in pre-selected retail jewelry (and the
                          Hostess Item) free!</li>
                      <li>First time attendees will receive a special surprise!</li>
                      <li>1 lucky winner will receive $500 each in JJ Boutique items!</li>
                      <li>Everyone has the opportunity to win FREE prizes throughout the day!</li>
                      <li>Earn 500 Escape to Paradise BONUS POINTS just for attending!</li>
                      <li>Take home your jewelry and boutique purchases from our new Collection at the end
                          of Conference!</li>
                      <li>Receive special offers and exclusive items available for purchase Conference weekend
                          only!</li>
                  </ul>
                  *Must be present at conference and a Just Jewelry Consultant to receive benefits
                  and prizes listed above
                  <br />
                  **Price increases to $179 on June 1st and June 30th is the final day to purchase
                  a ticket</div>
              <br />
              <div id="PurchasePanelDiv" runat="server">
                  <div class="col-12" style="text-align: center;">
                      <div class="col-12">
                          <span class="ticket-quantity-span">Ticket Quantity</span>
                          <asp:DropDownList ID="PurchaseQuantityDDL" runat="server">
                              <asp:ListItem Text="1" Value="1">
                              </asp:ListItem>
                              <asp:ListItem Text="2" Value="2">
                              </asp:ListItem>
                              <asp:ListItem Text="3" Value="3">
                              </asp:ListItem>
                              <asp:ListItem Text="4" Value="4">
                              </asp:ListItem>
                              <asp:ListItem Text="5" Value="5">
                              </asp:ListItem>
                          </asp:DropDownList>
                      </div>
                      <div class="form-check ">
                          <input type="checkbox" class="form-check-input" id="VegetarianMealCheckBox" style="position: relative!important;"
                              runat="server">
                          <label class="form-check-label" for="VegetarianMealCheckBox">
                              Check box for vegetarian meal-no other special diet requests can be accommodated.</label>
                      </div>
                      <br />
                      <div class="col-12">
                          <asp:Button ID="PurchaseTicketButton" runat="server" Text="Buy Ticket" CssClass="buy-ticket-btn" />
                          <br />
                          <span class="notice">*Ticket sales are final and are non-refundable.</span>
                      </div>
                  </div>
              </div>
          </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>
<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalLabel">Agenda</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
         <div class="agenda">
        <div class="table-responsive">
            <table class="table table-condensed table-bordered">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Time</th>
                        <th>Event</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- Single event in a single day -->
                    
                    <!-- Multiple events in a single day (note the rowspan) -->
                    <tr>
                        <td class="agenda-date" class="active" rowspan="4">
                            <div class="dayofmonth">27th</div>
                            <div class="dayofweek">Friday</div>
                            <div class="shortdate text-muted">July, 2018</div>
                        </td>
                        <td class="agenda-time">
                            3:00PM - 5:00PM 
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                It’s everything Fashion! See and hear about our new fall and winter jewelry and boutique collection from our Founders at the Hilton. Learn how to style coordinating pieces together and mix & match to maximize your customers styling options! This is your one opportunity to learn about the versatility of our collection and how to create complete looks for your customers, which will increase your sales this season. You are going to love all the beautiful colors, fabrics, metals and designs! Your customers are going to find it easy and affordable to rock the runway this season. It’s about the “total look” and JJ has it! 
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="agenda-time">
                            5:15PM
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                National Achiever's Dinner by Invitation Only at the Hilton
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="agenda-time">
                            5:00PM - 7:00PM
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                Dinner… it’s about your personal style, what works for you and what makes you feel fabulous! Get creative and make plans to dine out with those on your team at a local restaurant or do a “carry out” with friends at the local park. Style your dinner around the things that you enjoy and invite your fashionista friends who like your style to join you for a diva’s dinner. There are lots of trendy spots for you to see and be seen, so get out there and strut your stuff. Use this designated time to team build, meet new friends and have fun. 

                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="agenda-time">
                            9:00PM - Midnight
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                As a Conference attendee, ordering from the New Fall/Winter Collection and Catalog will be available ONLY to you and ONLY until midnight! Your orders will be ready for you to take home at the close of conference on Saturday or choose to have your order shipped on Monday.
                            </div>
                        </td>
                    </tr>
                    <!-- Saturday -->
                    <tr>
                        <td class="agenda-date" class="active" rowspan="3">
                            <div class="dayofmonth">28th</div>
                            <div class="dayofweek">Saturday</div>
                            <div class="shortdate text-muted">July, 2018</div>
                        </td>
                        <td class="agenda-time">
                            8:45PM - 9:30PM 
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                Special Breakfast for Team Leaders and Above at the Hilton
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="agenda-time">
                            10:00AM - 4:00PM (Check In Begins at 9:30) 
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                Join us for an incredible day packed full of education, motivation, awards, prizes & FUN! You are going to go ga-ga over our new Fall/Winter Catalog Reveal and you’ll be eager to be stylin’ around town marketing your business. Bring your mobile device for a special social media focused breakout session. We’ll be celebrating some amazing accomplishments as Just Jewelry National Achievers strut their stuff. You'll have discussion time and networking opportunities over a yummy lunch with friends. Be the first to hear the location of our next tropical vacation and how you can make this trip a reality for you! Don’t forget your camera as there are sure to be lots of photo opportunities as you and your fellow fashionistas are stylin’ in your business casual attire and JJ items throughout the weekend! There will be trendy gifts and prizes along with fabulous surprises all weekend long! (lunch provided)
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="agenda-time">
                            4:00PM
                        </td>
                        <td class="agenda-events">
                            <div class="agenda-event">
                                Free-Style Consultant time. Be creative!

                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>
<div id="galleryModal" class="modal fade" tabindex="-1" role="dialog">
<div class="modal-dialog">
	<div class="modal-content">
	  <div class="modal-body">
        <img src="" id="galleryModalImg" width="100%">
	  </div>
	</div>
</div>
</div>
<div class="modal fade" id="videoModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalLongTitle">Fashion Weekend</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        <video id="mainVideo" preload="auto" controls>
  <source src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/2018_conference_registration_video.mp4" type="video/mp4">
  <source src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/2018_conference_registration_video.ogv" type="video/ogg">
  <source src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/FashionWeekend/2018_conference_registration_video.webm" type="video/webm">
</video>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>

    <script src="../../Scripts/agency/vendor/jquery/jquery.min.js" type="text/javascript"></script>
    <script src="../../Scripts/agency/vendor/bootstrap/js/bootstrap.bundle.min.js" type="text/javascript"></script>
    <script src="../../Scripts/agency/vendor/jquery-easing/jquery.easing.min.js" type="text/javascript"></script>
    <script src="../../Scripts/agency/js/jqBootstrapValidation.js" type="text/javascript"></script>
    <script src="../../Scripts/agency/js/contact_me.js" type="text/javascript"></script>
        <script src="../../Scripts/agency/js/agency.js" type="text/javascript"></script>
    <!-- Custom scripts for this template -->


    <script>
        $('.galleryImage').click(function () {
            $('#galleryModalImg').attr('src', $(this).find('img').attr('src'));
            $('#galleryModal').modal({ show: true })

        });

        $(document).ready(function () {

            $('#videoModal').on('hide.bs.modal', function (e) {
                $('#mainVideo').trigger('pause');

            });

            $('#videoModal').on('shown.bs.modal', function (e) {
                $('#mainVideo').trigger('play');

            });

        });
    </script>

</asp:Content>
