using isRock.LineBot;

namespace BL.Service.Line
{

    public interface ILineBotService
    {
        bool ReplyMessage(string token, List<MessageBase> messages);
    }
}