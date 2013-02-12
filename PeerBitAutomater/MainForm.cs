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
            //pbAPI.LoginPeerBet();
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
            while (count < 10);
                        //pbAPI.GetClosestOrderInstantDouble(Double.Parse(this.textBox1.Text), ref lowestOrderID, ref lowstOrderPrice);
            //this.textBox1.Text = pbAPI.APIKey;//single string
            pbAPI.RefreshPeerBetGetUserInfo();
            this.lbl24HourProfit.Text = pbAPI.TodayProfitFromPB.ToString();
            this.lblBalance.Text = pbAPI.Balance.ToString();
         /*       String strGetARafflesResponse = pbRequest.GetPBResponse("method=getactiveraffles");
 
                var results = JsonConvert.DeserializeObject<dynamic>(strGetARafflesResponse, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });

            foreach (var results2 in results)
            {
                String raffleID = results2.raffle_id;
                //Console.WriteLine(results2.raffle_id);
            }
            */
                /*Dictionary<string, string> c2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(strGetARafflesResponse, new JsonSerializerSettings
          {
              TypeNameHandling = TypeNameHandling.All,
              TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
          });*/
            /*finalUrl = string.Format("{0}{1}", "https://peerbet.org/api.php", "?" + "method=getactiveraffles");
            
             * request = WebRequest.Create(finalUrl);

            response = request.GetResponse();
            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            reader = new StreamReader(dataStream);
            //92b0929a9caea4f8352cd7693bfb13b1
            // Read the content fully up to the end.
            responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            ArrayList al = (ArrayList)JsonConvert.DeserializeObject<ArrayList>(responseFromServer);
            ht = (Hashtable)JsonConvert.DeserializeObject<Hashtable>(responseFromServer); */
        }
			//catch(Exception ex)
			//{
			//	MessageBox.Show(this, "Error Deserializing: " + ex.Message, "Deserialization Error");
			//}

//            request.GetResponse()
        //}
    }
}
