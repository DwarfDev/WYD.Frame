using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x3A0)]
public struct P3A0_Signal : IPacket
{
    public NetworkHeader Header { get; set; }
}