﻿using Core.Domain.DTO.ResponseDTO.Line;
using Models.Line.Webhook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Utility.Line {

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
                string requestBodyStr2 = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
                byte[] data = encoding.GetBytes(requestBodyStr2);
                //byte[] data = encoding.GetBytes(testStr);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                //requestStream.WriteTimeout = 20000;
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                // Add 紀錄發至LineServer的requestBody
                string requestBodyStr = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                Console.WriteLine($"========== TO LINE SERVER: {httpPostRequestUri} ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBodyStr2}");
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