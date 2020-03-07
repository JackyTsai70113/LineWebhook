using System;
using System.IO;
using System.Net;
using System.Text;
using Models.Line;
using Models.Line.Webhook;
using Models.Google.API;
using Newtonsoft.Json;

namespace Utility.Google.MapAPIs
{
    public class GeocodeHandler
    {
        public static DistanceMatrix GetDistanceMatrix(string destinationAddress, string originAddress)
        {
            string uri = "https://maps.googleapis.com/maps/api/distancematrix/json";
            uri += $"?origins={destinationAddress}";
            uri += $"&destinations={originAddress}";
            uri += $"&key=AIzaSyAt8mx-_cf_K7SlutrTWNwuO0g4dPjqMNY";
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Add("Content-Type", "application/json");

            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            var distanceMatrixStr = streamReader.ReadToEnd();

            // 去除換行字元
            distanceMatrixStr = distanceMatrixStr.Replace("\n", "");
            return JsonConvert.DeserializeObject<DistanceMatrix>(distanceMatrixStr);
        }
    }
}