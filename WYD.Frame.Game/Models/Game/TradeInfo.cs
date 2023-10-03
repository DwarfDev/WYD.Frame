using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game.Models.Game;

public class TradeInfo
{
    public int Gold { get; set; }
    public NetworkItem[] Items { get; set; }
    public string? PlayerName { get; set; }
    public ushort PlayerId { get; set; }
    public bool Accepted { get; set; }
}