using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace BL.Service.Telegram
{
    public interface ITelegramService
    {
        User GetMe();

        Task<IEnumerable<Message>> SendDiceAsync();

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        void NotifyByMessage(string message);

        void UpdateWebhook(MessageEventArgs messageEventArgs);
    }
}