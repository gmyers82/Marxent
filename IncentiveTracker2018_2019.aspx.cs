using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using JJPro.Web;

namespace JJPro.Business
{
    public partial class IncentiveTracker2018_2019 : JJPro.Web.ConsultantPage
    {
        DataTable EventsData = new DataTable();
        DataTable LadderData = new DataTable();
        DataTable LaunchData = new DataTable();
        DataTable RecruitData = new DataTable();
        DataTable SalesData = new DataTable();
        DataTable JumpStartData = new DataTable();
        DataTable SpecialAwardData = new DataTable();
        DataTable PromotionData = new DataTable();
        string ConsultantId;
        int PointsEarned;
        LeadershipLevel ll;

        String startDate = "07/01/2018";
        String endDate = "06/30/2019";

        protected void Page_Load(object sender, EventArgs e)
        {



            ConsultantId = this.CurrentConsultant.ConsultantId;



            BadgePanel.Visible = false;
            LevelOneBadgeImage.Visible = false;
            LevelTwoBadgeImage.Visible = false;
            LevelThreeBadgeImage.Visible = false;
            LevelFourBadgeImage.Visible = false;
            LevelFiveBadgeImage.Visible = false;
            //LevelSixBadgeImage.Visible = false;
            //LevelSevenBadgeImage.Visible = false;
            //LevelEightBadgeImage.Visible = false;


            BadgePanel.Visible = false;
            LevelOnePanel.Visible = true;
            LevelTwoPanel.Visible = true;
            LevelThreePanel.Visible = true;
            LevelFourPanel.Visible = true;
            LevelFivePanel.Visible = true;
            //LevelSixPanel.Visible = true;
            //LevelSevenPanel.Visible = true;
            //LevelEightPanel.Visible = true; 


            Initialize();

            PointsEarnedLabel.Text = PointsEarned.ToString();
            LevelOneLabel.Text = LevelOneMessage(PointsEarned);
            LevelTwoLabel.Text = LevelTwoMessage(PointsEarned);
            LevelThreeLabel.Text = LevelThreeMessage(PointsEarned);
            LevelFourLabel.Text = LevelFourMessage(PointsEarned);
            LevelFiveLabel.Text = LevelFiveMessage(PointsEarned);
            //LevelSixLabel.Text = LevelSixMessage(PointsEarned);
            //LevelSevenLabel.Text = LevelSevenMessage(PointsEarned);
            //LevelEightLabel.Text = LevelEightMessage(PointsEarned);






        }




        void Initialize()
        {
            Reporting rept = new Reporting(this.CurrentConsultant.ConsultantsGuid);

            //AnnualIncentiveVolume aiv = new AnnualIncentiveVolume(Convert.ToString(ConsultantId), 2013, startDate, endDate);
            //if (aiv.Status == "Success")
            //{
            //    SalesData = aiv.IncentiveDataMonthlySales();
            //    EventsData = aiv.IncentiveDataByOrderType(AnnualIncentiveVolume.OrderType.EA);
            //    LadderData = aiv.IncentiveDataByOrderType(AnnualIncentiveVolume.OrderType.LD);
            //    LaunchData = aiv.IncentiveDataByOrderType(AnnualIncentiveVolume.OrderType.NL);
            //    JumpStartData = aiv.IncentiveDataByOrderType(AnnualIncentiveVolume.OrderType.JS);

            //    PointsEarned = aiv.PointsEarned();
            //}

            SalesData = rept.FetchIncentiveDataByMonthlySales(2018);
            EventsData = rept.FetchIncentiveDataByOrderType(Reporting.OrderType.EA, 2018);
            LadderData = rept.FetchIncentiveDataByOrderType(Reporting.OrderType.LD, 2018);
            LaunchData = rept.FetchIncentiveDataByOrderType(Reporting.OrderType.NL, 2018);
            JumpStartData = rept.FetchIncentiveDataByOrderType(Reporting.OrderType.JUMPSTART, 2018);
            SpecialAwardData = rept.FetchIncentiveDataByOrderType(Reporting.OrderType.SPECIALAWARD, 2018);
            PromotionData = rept.FetchIncentiveDataByOrderType(Reporting.OrderType.PROMOTION, 2018);



            //RecruitData = aiv.IncentiveDataNewRecruits();
            RecruitData = rept.FetchIncentiveDataByNewRecruit(2018);
            object sumObject;
            sumObject = RecruitData.Compute("Sum(Points)", "");

            if (sumObject.ToString().Length == 0)
            {
                sumObject = 0;
            }

            DataTable dtTotal = rept.FetchIncentives(this.CurrentConsultant.ConsultantsGuid, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));

            PointsEarned = Convert.ToInt32(Math.Floor(Convert.ToDecimal(dtTotal.Rows[0]["aiv"].ToString()))) + Convert.ToInt32(sumObject);

            if (PromotionData.Rows.Count > 0)
            {
                PromotionPanel.Visible = true;
                PromotionRepeater.DataSource = PromotionData;
                PromotionRepeater.DataBind();
            }
            else
            {
                PromotionPanel.Visible = false;
            }

            if (SpecialAwardData.Rows.Count > 0)
            {
                SpecialAwardPanel.Visible = true;
                SpecialAwardRepeater.DataSource = SpecialAwardData;
                SpecialAwardRepeater.DataBind();
            }
            else
            {
                SpecialAwardPanel.Visible = false;
            }

            if (EventsData.Rows.Count > 0)
            {
                CorporateEventsPanel.Visible = true;
                CorporateEventsRepeater.DataSource = EventsData;
                CorporateEventsRepeater.DataBind();
            }
            else
            {
                CorporateEventsPanel.Visible = false;
            }

            if (LadderData.Rows.Count > 0)
            {
                LadderOfDreamsPanel.Visible = true;
                LadderOfDreamsRepeater.DataSource = LadderData;
                LadderOfDreamsRepeater.DataBind();
            }
            else
            {
                LadderOfDreamsPanel.Visible = false;
            }


            if (JumpStartData.Rows.Count > 0)
            {
                JumpStartPanel.Visible = true;
                JumpStartRepeater.DataSource = JumpStartData;
                JumpStartRepeater.DataBind();
            }
            else
            {
                JumpStartPanel.Visible = false;
            }

            if (LaunchData.Rows.Count > 0)
            {
                NewLaunchOrderPanel.Visible = true;
                NewLaunchOrderingRepeater.DataSource = LaunchData;
                NewLaunchOrderingRepeater.DataBind();
            }
            else
            {
                NewLaunchOrderPanel.Visible = false;
            }

            if (RecruitData.Rows.Count > 0)
            {
                DirectRecruitsPanel.Visible = true;
                DirectRecruitsRepeater.DataSource = RecruitData;
                DirectRecruitsRepeater.DataBind();
            }
            else
            {
                DirectRecruitsPanel.Visible = false;
            }

            if (SalesData.Rows.Count > 0)
            {
                PersonalSalesPanel.Visible = true;
                PersonalSalesRepeater.DataSource = SalesData;
                PersonalSalesRepeater.DataBind();
            }
            else
            {
                PersonalSalesPanel.Visible = false;
            }

            if (LadderData.Rows.Count > 0)
            {
                LadderOfDreamsPanel.Visible = true;
                LadderOfDreamsRepeater.DataSource = LadderData;
                LadderOfDreamsRepeater.DataBind();
            }
            else
            {
                LadderOfDreamsPanel.Visible = false;
            }

        }

        String LevelOneMessage(int points)
        {
            string result;

            if (points >= 15000)
            {
                BadgePanel.Visible = true;
                LevelOneBadgeImage.Visible = true;
                result = "Congratulations! You've earned your Free Logo Beach Towel!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(15000, points) + " points away from your Free Logo Beach Towel!.<br />";
            }
            return result;
        }

        String LevelTwoMessage(int points)
        {
            string result;

            if (points >= 26000)
            {
                LevelTwoBadgeImage.Visible = true;
                LevelOnePanel.Visible = false;
                result = "Congratulations! You've earned your 2020 Escape to Paradise Trip!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(26000, points) + " points away from your Escape to Paradise Trip!<br />";
            }
            return result;
        }

        String LevelThreeMessage(int points)
        {
            string result;

            if (points >= 52000)
            {
                LevelThreeBadgeImage.Visible = true;
                LevelTwoPanel.Visible = false;
                result = "Congratulations! You can bring a companion!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(52000, points) + " points away from bringing a companion!<br />";
            }
            return result;
        }

        String LevelFourMessage(int points)
        {
            string result;

            if (points >= 58000)
            {
                LevelFourBadgeImage.Visible = true;
                LevelThreePanel.Visible = false;
                result = "Congratulations! You've earned your Oceanview Suite! <br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(58000, points) + " points away from an upgraded Balcony Cabin!<br />";
            }
            return result;
        }

        String LevelFiveMessage(int points)
        {
            string result;
            if (points >= 62000)
            {
                LevelFiveBadgeImage.Visible = true;
                LevelFourPanel.Visible = false;
                result = "Congratulations! You just will receive $50 cruise dollars for every 5000 points earned!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(62000, points) + " points away to receive $50 cruise dollars for every 5000 points earned after hitting 62,000 points.<br />";
            }
            return result;
        }

        String LevelSixMessage(int points)
        {
            string result;
            if (points >= 60000)
            {
                //LevelSixBadgeImage.Visible = true;
                LevelFivePanel.Visible = false;
                result = "Congratulations! You will receive $50 cruise dollars for every 5000 points earned!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(60000, points) + " points away to receive $50 cruise dollars and earn more for every 5000 points earned after hitting 62,000 points.<br />";
            }
            return result;
        }

        String LevelSevenMessage(int points)
        {
            string result;

            if (points >= 51000)
            {
                //LevelSevenBadgeImage.Visible = true;
                LevelSixPanel.Visible = false;
                result = "Congratulations! You just will receive $50 cruise dollars for every 1000 points earned!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(51000, points) + " points away to receive $50 cruise dollars for every 1000 points earned.<br />";
            }
            return result;
        }

        String LevelEightMessage(int points)
        {
            string result;
            if (points >= 62000)
            {
                //LevelEightBadgeImage.Visible = true;
                LevelSevenPanel.Visible = false;
                result = "Congratulations! You just earned a Companion <b>AND</b> $500 Towards Airfare!<br />";
            }
            else
            {
                result = "You are only " + GetPointsNeeded(62000, points) + " points away from bringing a Companion <b>AND</b> $500 Towards Airfare.<br />";
            }
            return result;
        }

        string GetPointsNeeded(int pointsNeeded, int pointsEarned)
        {
            return (pointsNeeded - pointsEarned).ToString();
        }

        string GetCruiseDollars(int pointsEarned)
        {

            int result = 0;
            int tally = pointsEarned - 62000;

            if (pointsEarned < 62000)
            {
                return "50";
            }
            else
            {
                result = (tally / 5000) * 50;

                return result.ToString();
            }
        }

    }
}