using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Line {

    public class LineBotService {

        public LineBotService() {
        }

        public bool PushMessage() {
            //傳送對象
            var toUserID = "________換成你自己的UserID________";
            //Channel Access Token
            var token = "________換成你LineBot的ChannelAccessToken________";
            //create bot instance
            Bot bot = new Bot(token);
            //send message
            bot.PushMessage(toUserID, "Hello test");
            return true;
        }
    }
}