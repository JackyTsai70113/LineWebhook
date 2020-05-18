using Core.Domain.DTO.ResponseDTO.Line;
using Models.Line.Webhook;
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

        public static string PostToLineServer(ReplyMessageRequestBody requestBody) {
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

                // Add 紀錄發至LineServer的requestBody
                string requestBodyStr = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                Console.WriteLine($"========== TO LINE SERVER: {httpPostRequestUri} ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBodyStr}");
                Console.WriteLine($"====================");

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result = streamReader.ReadToEnd();
            } catch (WebException webEx) {
                result += "Server未取得回應 " + webEx.ToString();
                Console.WriteLine($"Server未取得回應 WebException: {webEx}");
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex}");
            }
            return result;
        }
    }
}