using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Game.Models.Game;

public class GameMessage
{
    public MessageOrigin MessageOrigin { get; set; }
    public string Command { get; set; }
    public string Message { get; set; }
    public MessageRelevance Relevance { get; set; }
    
    public DateTime ReceivedAt { get; set; } = DateTime.Now;
}