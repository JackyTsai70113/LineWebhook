using System.Net;
using System.Text.Json;
using BL.Service.Line.Interface;
using isRock.LineBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BL.Service.Line {

    public class LineBotService : ILineBotService {
        private readonly ILogger<LineBotService> logger;
        private readonly IConfiguration config;
        private readonly Bot bot;

        public LineBotService(ILogger<LineBotService> logger, IConfiguration config) {
            this.logger = logger;
            this.config = config;
            this.bot = new Bot(config["Line:ChannelAccessToken"]);
        }

        /// <summary>
        /// 推播至Group
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Group(string msg) {
            return SendNotify(config["Line:NotifyBearerToken_Group"], msg);
        }

        /// <summary>
        /// 推播至Jacky
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool Notify_Jacky(string msg) {
            bot.SendNotify(config["Line:NotifyBearerToken_Jacky"]
            , "Hi"
            , new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/240px-Cat03.jpg")
            , new Uri("https://images.hdqwalls.com/download/cat-4k-po-2048x2048.jpg")
            , 6362
            , 11087927
            , false);
            return SendNotify(config["Line:NotifyBearerToken_Jacky"], msg);
        }

        /// <summary>
        /// 推播至Jessi
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Jessi(string msg) {
            return SendNotify(config["Line:NotifyBearerToken_Jessi"], msg);
        }

        public bool ReplyMessage(string token, List<MessageBase> messages) {
            try {
                string res = bot.ReplyMessage(token, messages);
                logger.LogInformation("ReplyMessage result: " + res);
                return true;
            } catch (Exception ex) {
                if (ex.InnerException is WebException) {
                    int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
                    int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                    string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                    LineHttpPostException response = JsonSerializer.Deserialize<LineHttpPostException>(responseStr);
                    logger.LogError(
                        $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {token},\n" +
                        $"messages: {JsonSerializer.Serialize(messages)},\n" +
                        $"response: {JsonSerializer.Serialize(response)},\n" +
                        $"ex: {ex}");
                    Notify_Jacky($"message: {response.message},\n" +
                        $"details: {JsonSerializer.Serialize(response.details)}");
                }
                return false;
            }
        }

        private bool SendNotify(string token, string msg) {
            var result = bot.SendNotify(token, msg);
            if (result.status != 200) {
                logger.LogError(result.message);
                return false;
            }
            return true;
        }
    }
}