using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x110)]
public struct P110_UpdateCharlist : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkCharlist NetworkCharlist;

}