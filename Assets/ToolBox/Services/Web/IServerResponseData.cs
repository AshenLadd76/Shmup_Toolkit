namespace ToolBox.Services.Web
{
    public interface IServerResponseData
    {
        string ResponseCode { get; set; }
        ServerResponse Response { get; set; }
        bool IsResponseDone { get; set; }
    }
}