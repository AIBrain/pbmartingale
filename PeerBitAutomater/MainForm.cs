using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace PeerBitAutomater
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            PeerBetAPI pbAPI = new PeerBetAPI(tbUsername.Text, tbPassword.Text);

            this.lbl24HourProfit.Text = pbAPI.TodayProfitFromPB.ToString();
            this.lblBalance.Text = pbAPI.Balance.ToString();
            Double startingBalance = pbAPI.Balance;

            String lowestOrderID = "";
            Double lowstOrderPrice = 0.0;
            Boolean inRun = false;

            int count = 1;

            pbAPI.GetLowestInstantHalfChance(ref lowestOrderID, ref lowstOrderPrice);

            //Loop thru martingale
            do{
                if (lowestOrderID != "0")
                {
                    if ((inRun == false) && lowstOrderPrice <= (0.1 * pbAPI.Balance))
                    {
                        pbAPI.PlaceOrder(lowestOrderID, 1);
                        CheckAndGetNext(lowestOrderID, lowstOrderPrice, ref pbAPI,
                                     ref lowestOrderID, ref lowstOrderPrice,
                                     ref inRun, ref count);
                    }
                    if (inRun == true)
                    {
                        pbAPI.PlaceOrder(lowestOrderID, 1);
                        CheckAndGetNext(lowestOrderID, lowstOrderPrice, ref pbAPI,
                                     ref lowestOrderID, ref lowstOrderPrice,
                                     ref inRun, ref count);
                    }
                }
                else
                {
                    if (inRun == true)

                        pbAPI.GetGTClosestOrder(lowstOrderPrice, PeerBetAPI.BetType.Instant, 2, 1,
                                    ref lowestOrderID, ref lowstOrderPrice);
                    else
                        pbAPI.GetLowestInstantHalfChance(ref lowestOrderID, ref lowstOrderPrice);
                }
                Double percentageWin = (pbAPI.Balance - startingBalance) / startingBalance;
                if (percentageWin > ((double)this.nmupPercentageProfit.Value * 0.01))
                    count = (int)nudTimetoRun.Value;

            }
            while (count < (int)nudTimetoRun.Value);
            pbAPI.RefreshPeerBetGetUserInfo();
            this.lbl24HourProfit.Text = pbAPI.TodayProfitFromPB.ToString();
            this.lblBalance.Text = pbAPI.Balance.ToString();
        
        }

        private void CheckAndGetNext(string lastOrderID, double lastOrderPrice,
                                     ref PeerBetAPI pbAPI,
                                     ref string nextOrderId, ref double nextOrderPrice,
                                     ref Boolean inRun, ref int countWon)
        {
            if (pbAPI.CheckIfWonOrder(lastOrderID) == true)
            {
                inRun = false;
                pbAPI.GetLowestInstantHalfChance(ref nextOrderId, ref nextOrderPrice);
                countWon++;
            }
            else
            {
                nextOrderPrice = 0.0;
                nextOrderId = "0";
                inRun = true;
                nextOrderPrice = lastOrderPrice * 2;
                FindNextOrderInBalance(pbAPI.Balance, ref pbAPI,
                                                ref nextOrderId, ref  nextOrderPrice);
                //pbAPI.GetGTClosestOrder(nextOrderPrice, PeerBetAPI.BetType.Instant, 2, 1,
                //    ref nextOrderId, ref nextOrderPrice);

            }
            Thread.Sleep(2000);
            pbAPI.RefreshPeerBetGetUserInfo();
        }

        private Boolean FindNextOrderInBalance(double balance, ref PeerBetAPI pbAPI,
                                                ref string nextOrderId, ref double nextOrderPrice)
        {
            Boolean returnVal = true;

            do
            {
                pbAPI.GetGTClosestOrder(nextOrderPrice, PeerBetAPI.BetType.Instant, 2, 1,
                    ref nextOrderId, ref nextOrderPrice);
            }
            while (nextOrderPrice >= pbAPI.Balance);

            return(returnVal);
        }
    }
}
