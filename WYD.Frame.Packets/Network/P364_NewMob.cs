using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x364)]
public struct P364_NewMob : IPacket
{
    public NetworkHeader Header { get; set; }


    public NetworkMob Mob { get; set; }
}