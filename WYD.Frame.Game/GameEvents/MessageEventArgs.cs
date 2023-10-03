using WYD.Frame.Common.Enum;

namespace WYD.Frame.Game.GameEvents;

public class MessageEventArgs
{
    public MessageRelevance Relevance { get; set; }
    public string Message { get; set; } = "";
    public string Command { get; set; } = "";
}