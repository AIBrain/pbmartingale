using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PeerBitAutomater
{
    class PBWebRequest
    {
        public String GetPBResponse(String requestString)
        {
            WebRequest request;
            Stream dataStream;

            String finalUrl = string.Format("{0}{1}", "https://peerbet.org/api.php", "?" + requestString);
            request = WebRequest.Create(finalUrl);

            WebResponse response = request.GetResponse();
            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return (responseFromServer);
        }
    }
}
