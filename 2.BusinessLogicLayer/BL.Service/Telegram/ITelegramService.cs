using Telegram.Bot.Types;

namespace BL.Service.Telegram
{
    public interface ITelegramService
    {
        User GetMe();

        /// <summary>
        /// 測試
        /// </summary>
        List<Message> SendDice();

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        void NotifyByMessage(string message);
    }
}