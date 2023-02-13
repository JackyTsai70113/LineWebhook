using System.Net;
using System.Text.Json;
using BL.Service.Line.Interface;
using isRock.LineBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BL.Service.Line
{

    public class LineBotService : ILineBotService
    {
        private readonly ILogger<LineBotService> Logger;
        private readonly IConfiguration Config;
        private readonly Bot Bot;

        public LineBotService(ILogger<LineBotService> logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            Bot = new Bot(config["Line:ChannelAccessToken"]);
        }

        /// <summary>
        /// 推播至Group
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Group(string msg)
        {
            return SendNotify(Config["Line:NotifyBearerToken_Group"], msg);
        }

        /// <summary>
        /// 推播至Jacky
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool Notify_Jacky(string msg)
        {
            var result = Bot.SendNotify(Config["Line:NotifyBearerToken_Jacky"]
            , msg
            , new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/240px-Cat03.jpg")
            , new Uri("https://images.hdqwalls.com/download/cat-4k-po-2048x2048.jpg")
            , 6362
            , 11087927
            , false);

            if (result.status != 200)
            {
                Logger.LogError("{message}", result.message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 推播至Jessi
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Jessi(string msg)
        {
            return SendNotify(Config["Line:NotifyBearerToken_Jessi"], msg);
        }

        public bool ReplyMessage(string token, List<MessageBase> messages)
        {
            try
            {
                string res = Bot.ReplyMessage(token, messages);
                Logger.LogInformation("ReplyMessage result: {res}", res);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is WebException)
                {
                    int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Length;
                    int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                    string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                    LineHttpPostException response = JsonSerializer.Deserialize<LineHttpPostException>(responseStr);
                    Logger.LogError(
                        "LineWebhookService.ResponseToLineServer 錯誤, replyToken: {token},\n" +
                        "messages: {messages},\n" +
                        "response: {response},\n" +
                        "ex: {ex}", token, JsonSerializer.Serialize(messages), JsonSerializer.Serialize(response), ex);
                    Notify_Jacky($"message: {response.Message},\n" +
                        $"details: {JsonSerializer.Serialize(response.Details)}");
                }
                return false;
            }
        }

        private bool SendNotify(string token, string msg)
        {
            var result = Bot.SendNotify(token, msg);
            if (result.status != 200)
            {
                Logger.LogError("{message}", result.message);
                return false;
            }
            return true;
        }
    }
}