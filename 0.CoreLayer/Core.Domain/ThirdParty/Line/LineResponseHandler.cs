﻿using Core.Domain.DTO.ResponseDTO.Line;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Core.Domain.ThirdParty.Line {

    public class LineResponseHandler {
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
                UTF8Encoding encoding = new UTF8Encoding();
                //string testStr = "{\"type\":\"template\",\"altText\":\"thisisacarouseltemplate\",\"template\":{\"type\":\"carousel\",\"columns\":[{\"thumbnailImageUrl\":\"https://example.com/bot/images/item1.jpg\",\"imageBackgroundColor\":\"#FFFFFF\",\"title\":\"thisismenu\",\"text\":\"description\",\"defaultAction\":{\"type\":\"uri\",\"label\":\"Viewdetail\",\"uri\":\"http://example.com/page/123\"},\"actions\":[{\"type\":\"postback\",\"label\":\"Buy\",\"data\":\"action=buy&itemid=111\"},{\"type\":\"postback\",\"label\":\"Addtocart\",\"data\":\"action=add&itemid=111\"},{\"type\":\"uri\",\"label\":\"Viewdetail\",\"uri\":\"http://example.com/page/111\"}]},{\"thumbnailImageUrl\":\"https://example.com/bot/images/item2.jpg\",\"imageBackgroundColor\":\"#000000\",\"title\":\"thisismenu\",\"text\":\"description\",\"defaultAction\":{\"type\":\"uri\",\"label\":\"Viewdetail\",\"uri\":\"http://example.com/page/222\"},\"actions\":[{\"type\":\"postback\",\"label\":\"Buy\",\"data\":\"action=buy&itemid=222\"},{\"type\":\"postback\",\"label\":\"Addtocart\",\"data\":\"action=add&itemid=222\"},{\"type\":\"uri\",\"label\":\"Viewdetail\",\"uri\":\"http://example.com/page/222\"}]}],\"imageAspectRatio\":\"rectangle\",\"imageSize\":\"cover\"}}";
                string requestBodyStr = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                byte[] data = encoding.GetBytes(requestBodyStr);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                // Add 紀錄發至LineServer的requestBody
                Console.WriteLine($"========== TO LINE SERVER: {httpPostRequestUri} ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBodyStr}");
                Console.WriteLine($"====================");

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                result = streamReader.ReadToEnd();
            } catch (WebException webEx) {
                result += "伺服器無回應, " + webEx.ToString();
                Console.WriteLine($"伺服器無回應, WebException: {webEx}");
            } catch (Exception ex) {
                result += "Exception: " + ex.ToString();
                Console.WriteLine($"Exception: {ex}");
            }
            return result;
        }
    }
}