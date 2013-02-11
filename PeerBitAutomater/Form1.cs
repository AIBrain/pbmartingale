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

namespace PeerBitAutomater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            PeerBetAPI pbAPI = new PeerBetAPI("sal002", "afsgroup");

            //pbAPI.LoginPeerBet();
            String lowestOrderID = "";
            Double lowstOrderPrice = 0.0;

            pbAPI.GetLowestInstantHalfChance(ref lowestOrderID, ref lowstOrderPrice);
            this.textBox1.Text = pbAPI.APIKey;//single string
            this.textBox2.Text = lowestOrderID;
            this.textBox3.Text = lowstOrderPrice.ToString() ;
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
