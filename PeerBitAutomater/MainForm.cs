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

            String lowestOrderID = "";
            Double lowstOrderPrice = 0.0;

            int count = 1;

            pbAPI.GetLowestInstantHalfChance(ref lowestOrderID, ref lowstOrderPrice);

            //Loop thru martingale
            do{  
                pbAPI.PlaceOrder(lowestOrderID, 1);

                Thread.Sleep(1000);
                if (pbAPI.CheckIfWonOrder(lowestOrderID) == true)
                {
                    pbAPI.GetLowestInstantHalfChance(ref lowestOrderID, ref lowstOrderPrice);
                    count++;
                }
                else
                {
                    Double foundOrderPrice = 0.0;
                    lowstOrderPrice = lowstOrderPrice * 2;
                    pbAPI.GetClosestOrderInstantDouble(lowstOrderPrice, ref lowestOrderID, ref foundOrderPrice);
                }
            }
            while (count < nudTimetoRun.Value);
            pbAPI.RefreshPeerBetGetUserInfo();
            this.lbl24HourProfit.Text = pbAPI.TodayProfitFromPB.ToString();
            this.lblBalance.Text = pbAPI.Balance.ToString();
        
        }
		
    }
}
