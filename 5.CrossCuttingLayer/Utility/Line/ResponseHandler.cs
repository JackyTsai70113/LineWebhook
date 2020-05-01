using Models.Line.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Utility.Line {

    public class ResponseHandler {
        public static readonly string httpPostRequestUri = "https://api.line.me/v2/bot/message/reply";

        public static readonly string channelAccessToken =
            @"tkOO80fthaESrdEWkHn5+gsypQLHd1N3DZcNsWaJku3GeO/
            HsFMyCSyU95KnA6p2bTLPFJS0y4joCknQyppqlwaDK34rrQgS
            W39EcS0j5WNEZGIlkup0nJ+xlBf+mcw89H1xKAc5Ubd0xA9/Z
            9RSIwdB04t89/1O/w1cDnyilFU=";

        public static string PostToLineServer(RequestBodyToLine requestBody) {
            string result = "";
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpPostRequestUri);
                request.Method = "POST";
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Authorization", "Bearer " + channelAccessToken);

                // Write data to requestStream
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(
                    System.Text.Json.JsonSerializer.Serialize(requestBody));
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                //requestStream.WriteTimeout = 20000;
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result = streamReader.ReadToEnd();

                // Add Logs
                string requestBodyStr = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                Console.WriteLine($"========== TO LINE SERVER: {httpPostRequestUri} ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBodyStr}");
            } catch (Exception ex) {
                result += "Exception: " + ex.Message;
                Console.WriteLine($"Exception: {ex.Message}");
            }
            Console.WriteLine($"====================");
            return result;
        }
    }
}