using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x3AF)]
public struct P3AF_UpdateGold : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Gold;
}