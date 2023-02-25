namespace BL.Service.Line
{
    public interface IChatGPTService
    {
        Result CallChatGPT(string msg);
    }
}