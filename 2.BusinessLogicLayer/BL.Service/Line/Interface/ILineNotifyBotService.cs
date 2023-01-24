using isRock.LineBot;

namespace BL.Service.Line.Interface {

    public interface ILineBotService {

        /// <summary>
        /// 推播至Group
        /// </summary>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        bool PushMessage_Group(string text);

        /// <summary>
        /// 推播至Jacky
        /// </summary>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        bool Notify_Jacky(string text);

        /// <summary>
        /// 推播至Jessi
        /// </summary>
        /// <param name="text">推播字串</param>
        /// <returns>是否推播成功</returns>
        bool PushMessage_Jessi(string text);

        bool ReplyMessage(string token, List<MessageBase> messages);
    }
}