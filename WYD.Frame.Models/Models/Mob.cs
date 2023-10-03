using Mapster;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Models.Models;

public class Mob
{
    public ushort ClientId { get; set; }
    public Position Position { get; set; }
    public string Name { get; set; }
    public short Face { get; set; }
    public short[] Equips { get; set; }
    public short[] Affects { get; set; }
    public PlayerStatus Status { get; set; }
    public string Tab { get; set; }

    public static Mob Create(NetworkMob networkMob)
    {
        return new Mob()
        {
            Affects = networkMob.Affects,
            Status = PlayerStatus.Create(networkMob.Status),
            Position = networkMob.NetworkPosition.Adapt<Position>(),
            Equips = networkMob.Equips,
            Face = networkMob.Face,
            ClientId = networkMob.ClientId,
            Name = networkMob.Name,
            Tab = networkMob.Tab
        };
    }
    
    public bool IsInPosition(Position position)
    {
        return Position.X == position.X && Position.Y == position.Y;
    }
}