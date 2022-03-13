﻿using System;
using System.Net.Http;
using System.Web;
using BL.Services.Line.Interfaces;
using Microsoft.Extensions.Logging;

namespace BL.Services.Line {

    public class LineNotifyBotService : ILineNotifyBotService {
        private readonly ILogger<LineNotifyBotService> logger;
        private readonly string _bearerToken_Group;
        private readonly string _bearerToken_Jacky;
        private readonly string _bearerToken_Jessi;

        private readonly string _notifyUri = "https://notify-api.line.me/api/notify";

        public LineNotifyBotService(ILogger<LineNotifyBotService> logger) {
            this.logger = logger;
            _bearerToken_Group = ConfigService.Line_NotifyBearerToken_Group;
            _bearerToken_Jacky = ConfigService.Line_NotifyBearerToken_Jacky;
            _bearerToken_Jessi = ConfigService.Line_NotifyBearerToken_Jessi;
        }

        /// <summary>
        /// 推播至Group
        /// </summary>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Group(string text) {
            return PushMessage(_bearerToken_Group, text);
        }

        /// <summary>
        /// 推播至Jacky
        /// </summary>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Jacky(string text) {
            return PushMessage(_bearerToken_Jacky, text);
        }

        /// <summary>
        /// 推播至Jessi
        /// </summary>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Jessi(string text) {
            return PushMessage(_bearerToken_Jessi, text);
        }

        /// <summary>
        /// 推播至指定bearerToken的聊天室
        /// </summary>
        /// <param name="bearerToken">OAuth 2.0 Bearer Token</param>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        private bool PushMessage(string bearerToken, string text) {
            try {
                string result;
                string urlEncodedText = HttpUtility.UrlEncode(text);
                string uri = _notifyUri + $"?message={urlEncodedText}";
                using (var httpClient = new HttpClient()) {
                    httpClient.BaseAddress = new Uri(uri);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
                    result = httpClient.PostAsync(uri, new StringContent("")).Result.ToString();
                }

                logger.LogInformation($"[PushMessage] text: {text}, PostAsync.Result: {result}");
                return true;
            } catch (Exception ex) {
                logger.LogError($"[PushMessage] text: {text}, ex: {ex}");
                return false;
            }
        }
    }
}