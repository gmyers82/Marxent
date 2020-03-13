<%@ Page Title="" Language="C#" MasterPageFile="~/Business/Business.Master" AutoEventWireup="true" CodeBehind="IncentiveTracker2018_2019.aspx.cs" Inherits="JJPro.Business.IncentiveTracker2018_2019" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainPlaceHolder" runat="server">
<asp:Panel ID="OldTrackerPanel" runat="server">
<div class="clear-fix" style="font-family:'garamond-premier-pro-display';">
<table width="930" cellpadding="0" cellspacing="0" border="0" align="center"><tr>
    <td>

    <table width="100%" cellpadding="10" cellspacing="0" border="0" align="center" style="margin-top:70px;">
    
<%--    <tr>
         <td>
     <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2019/2019%20Incentive%20Trip%20Banner-.jpg" alt"Escape Banner" style="max-width:1000px;" />
     </td>
    </tr>--%>

     <tr>
    
     <td>
    
    <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/2020_trip_incentive_header.png" alt="Just Jewelry 2020 Escape To Paradise" border="0" />
    
    
    </td></tr>



   </table>

   <asp:Panel ID="BadgePanel" runat="server">
   <br /><br />
   

   <table width="50%" cellpadding="10" cellspacing="0" border="0" align="center"><tr><td>
    <hr align="left" width="725" />
   <strong><em>Prizes earned&mdash;</em></strong><br />
   <asp:Image id="LevelOneBadgeImage" imageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/buttons-1.png" Width="75" Height="75" ToolTip="Your 15000 Points earned this Free Logo Beach Towel!" runat="server" />
   <asp:Image id="LevelTwoBadgeImage" imageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/buttons-2.png" Width="75" Height="75" ToolTip="Your 26000 Points earned your 2019 Escape to Paradise Trip in Ocean view Cabin!" runat="server" />
   <asp:Image id="LevelThreeBadgeImage" imageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/buttons-3.png" Width="75" Height="75" ToolTip="Your 52000 Points earned a companion to come with you!" runat="server" />
   <asp:Image id="LevelFourBadgeImage" imageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/buttons-4.png" Width="75" Height="75" ToolTip="Your 58000 Points earned your Upgraded Balcony Cabin!" runat="server" />
   <asp:Image id="LevelFiveBadgeImage" imageUrl="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/buttons-5.png" Width="75" Height="75" ToolTip="Your 62000 Points earned youyou $50 cruise dollars for every 5000 points above this level!" runat="server" />
   <hr align="left" width="725" />
   </td></tr></table>
      
</asp:Panel>
  
    <br />


   <table width="100%" cellpadding="5" cellspacing="0" border="0" align="center" >
   <tr>
   <td width="320" valign="top" align="center">


        <strong><em>My Points Earned: <asp:Label ID="PointsEarnedLabel" runat="server" /></em></strong>


        <asp:Panel ID="PersonalSalesPanel" runat="server">        

        <p style="font-size:10pt;"><strong>Personal Sales</strong></p>
        <asp:repeater id="PersonalSalesRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
        <th>Month</th>
        <th width="50">Points</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Month") %></td>
        <td align="right"> <%# Eval("Points") %></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <asp:Panel ID="DirectRecruitsPanel" runat="server">
        <p style="font-size:10pt;"><strong>Direct Recruits</strong></p>

        <asp:repeater id="DirectRecruitsRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
        <th >Recruit</th>
        <th width="50">Qualified Date</th>
        <th width="50">Points</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Recruit") %></td>
        <td align="left"> <%# Eval("SignOnDate", "{0:d}")%></td>
        <td align="right"> <%# Eval("Points") %></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <asp:Panel ID="NewLaunchOrderPanel" runat="server">
       <p style="font-size:10pt;"><strong>New Launch Ordering Bonus</strong></p>
       
       <asp:repeater id="NewLaunchOrderingRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
        <th>Date</th>
        <th width="50">Points</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Date", "{0:d}")%></td>
        <td align="right"> <%# Eval("Points") %></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <asp:Panel ID="JumpStartPanel" runat="server">
       <p style="font-size:10pt;"><strong>JumpStart Bonus</strong></p>
       
       <asp:repeater id="JumpStartRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
        <th>Date</th>
        <th width="50">Points</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Date", "{0:d}")%></td>
        <td align="right"> <%# Eval("Points") %></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <asp:Panel ID="LadderOfDreamsPanel" runat="server">
       <p style="font-size:10pt;"><strong>Ladder of Dreams Bonus</strong></p>

        <asp:repeater id="LadderOfDreamsRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
        <th width="50">Date</th>
        <th>Level</th>
        <th width="50">Points</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Date", "{0:d}")%></td>
        <td align="left"> <%# Eval("Note") %></td>
        <td align="right"> <%# Eval("Points") %></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <asp:Panel ID="CorporateEventsPanel" runat="server">
        <p style="font-size:10pt;"><strong>Corporate Events Attendence Bonus</strong></p>

       <asp:repeater id="CorporateEventsRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
        <th>Date</th>
        <th width="50">Points</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Date", "{0:d}")%></td>
        <td align="right"> <%# Eval("Points") %></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>
      
      <asp:Panel ID="SpecialAwardPanel" runat="server">
        <p style="font-size:10pt;"><strong>Special Award Bonus</strong></p>

       <asp:repeater id="SpecialAwardRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
          <th width="50">Date</th>
          <th width="50">Points</th>
          <th>Comments</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Date", "{0:d}")%></td>
        <td align="right"> <%# Eval("Points") %></td>
        <td align="right"> <%# Eval("Note")%></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <asp:Panel ID="PromotionPanel" runat="server">
        <p style="font-size:10pt;"><strong>Promotion Bonus</strong></p>

       <asp:repeater id="PromotionRepeater" runat="server">
        
        <headertemplate>
        <table width="280" cellspacing="0" cellpadding="10" border="1" style="font-size:12pt;">
        <tr>
          <th width="50">Date</th>
          <th width="50">Points</th>
          <th>Comments</th>
        </tr>
        </headertemplate>
          
        <itemtemplate>
        <tr>
        <td align="left"> <%# Eval("Date", "{0:d}")%></td>
        <td align="right"> <%# Eval("Points") %></td>
        <td align="right"> <%# Eval("Note")%></td>
        </tr>
        </itemtemplate>
          
        <footertemplate>
          </table>
        </footertemplate>

      </asp:repeater>
      </asp:Panel>

      <br />
      <p style=" color:red; font-size:16px;">Note: New points and recruits may take up to 24 hours to display on your tracker.  Only recruits that have qualified in the incentive period will show on the list</p>

        </td>

        <td width="20">&nbsp;</td>


        <td width="600" valign="top">
        <div>
        <asp:Panel ID="LevelOnePanel" runat="server" >
            <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/Incentive%20Banners-1.jpg" width="600" height="150" alt="Sell Now Relax Later in Paradise!" border="0" /><br />
            <asp:Label ID="LevelOneLabel" runat="server" /><br />
        </asp:Panel>
        
        <asp:Panel ID="LevelTwoPanel" runat="server">    
            <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/Incentive%20Banners-2.jpg" width="600" height="150" alt="Congratulations" border="0" /><br />
            <asp:Label ID="LevelTwoLabel" runat="server" /><br />
        </asp:Panel>
            
        <asp:Panel ID="LevelThreePanel" runat="server">
            <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/Incentive%20Banners-3.jpg"  width="600" height="150" alt="Start Packing your Beach Bag!" border="0" /><br />
            <asp:Label ID="LevelThreeLabel" runat="server" /><br />
        </asp:Panel>

         <asp:Panel ID="LevelFourPanel" runat="server">
            <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/Incentive%20Banners-4.jpg" width="600" height="150" alt="You won't be escaping alone!" border="0" /><br />
            <asp:Label ID="LevelFourLabel" runat="server" /><br />
         </asp:Panel>   

         <asp:Panel ID="LevelFivePanel" runat="server">    
            <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2020/Incentive%20Banners-5.jpg" width="600" height="150" alt="Did we mention an upgrade?!" border="0" /><br />
            <asp:Label ID="LevelFiveLabel" runat="server" /><br />
         </asp:Panel>
            
         <asp:Panel ID="LevelSixPanel" runat="server" Visible="false">    
            <img src="https://6a8c65a7332a6ea186f2-6c2aaf386ecfc7423d48bccd1a997322.ssl.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2019/Incentive%20Banners-6.jpg" width="600" height="150" alt="Cruise dollars to spend!" border="0" /><br />
            <asp:Label ID="LevelSixLabel" runat="server" /><br />
         </asp:Panel>
         
         <asp:Panel ID="LevelSevenPanel" runat="server" Visible="false">
            <img src="http://8f2270e13c3e0703baa2-6c2aaf386ecfc7423d48bccd1a997322.r2.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2017/PrizeBanners/2015_FW_Incentive_Banners_7.jpg" width="600" height="150" alt="Cruise dollars to spend!" border="0" /><br />
            <asp:Label ID="LevelSevenLabel" runat="server" /><br />
         </asp:Panel>

         <asp:Panel ID="LevelEightPanel" runat="server" Visible="false">
            <img src="http://8f2270e13c3e0703baa2-6c2aaf386ecfc7423d48bccd1a997322.r2.cf1.rackcdn.com/BusinessPortal/Incentives/EscapeToParadise2016/Prize%20Banners/Incentive%20Banners8.jpg" width="600" height="150" alt="Jackpot! You've scored some cruise Dollars!" border="0" /><br />
            <asp:Label ID="LevelEightLabel" runat="server" /><br />
         </asp:Panel>
            
         
            <div style="font-size:12pt;color:#444444;">

            <p>
            <strong>Incentive Requirements:  </strong> <br /><br />Here are the point values associated with each reward as you work your way towards 
            earning the 2020 Escape to Paradise Incentive Trip and MORE!<br /> 
            <div>
		15,000 points: Free Logo Beach Towel<br />
		26,000 points: 2020 Escape to Paradise Trip<br />
		52,000 points: Companion Trip <br />
		58,000 points: Upgraded to Balcony Cabin<br />
		62,000+ points: $50 spending cash for every 5000 points earned after hitting 62,000 points<br />


            </div>
            <br /> <br />
	    Your point score is determined from the sum of points earned:<br />
            </p>


                <p>
                    <strong>Personal Jewelry Sales:</strong> <br /><br />Points apply to <u>regularly priced</u> jewelry & boutique purchases only. 
                    Certain deeply discounted items, items that give back, taxes, supplies and shipping charges do not qualify 
                    for points.  Consultants will earn one point for each dollar of jewelry purchased between July 1, 2018 and 
                    June 30, 2019.  On June 30, 2018, jewelry purchases will be totaled and rounded down to the nearest dollar 
                    to determine total points earned. $1 =  1 POINT. <br /> <br />
                </p>
                <p>
                    <strong>New Qualified Recruits:</strong> <br /><br /> A new qualified recruit is defined as a person who signs 
                    on as a Just Jewelry Consultant for the <u>first time</u>, meets the $399 minimum purchase requirement and all other
                    requirements in the consultant agreement.  For each new Qualified Consultant, the upline will be awarded 600 
                    points.  If the new recruit returns their qualifying purchase(s), the points will be forfeited.   
                </p>

                <p>
                    <strong>Corporate Events Attendance Bonus:</strong> <br /><br /> Consultants will earn 1000 bonus points for attending the 2018 
                    National Conference.  Consultant must be in attendance at the conference to be awarded bonus points.  
                </p>
                <p>
                    <strong>Jump Start Bonus: </strong> <br /><br /> New Qualifying Consultants who complete the Jump Start Program goal within 30 days will receive 500 bonus points. When they complete the 60 day goal they will receive 1,000 bonus points. When they complete the 90 day goal they will receive 1,500 points.
                </p>

                
                
               
		
	
            <p><strong>Rules & Restrictions:</strong><br /><br /> A maximum of two trip spots (one consultant and one companion)
            can be earned per consultant.  The companion must be 18 years of age or older and may not be a current or past Just 
            Jewelry consultant.  The consultant must be an active Qualified Consultant as of the travel date.  
            If a consultant deactivates during the incentive period or before the travel date, all points/trips are forfeited. 
            Consultants and companions are responsible for travel to and from the trip destination.  
            Points are non-transferable and hold no monetary value.  Just Jewelry reserves the right to change the trip dates, 
            hotel or destination.
            </p>
 	
            </div>
            </div>
        </td>
   </tr>
   </table>

</td></tr></table>
</div>
</asp:Panel>
</asp:Content>
