using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game.Models.Game;

public class AffectInfo
{
    public NetworkAffect Affect { get; set; }
    public DateTime EndAt { get; set; }
}