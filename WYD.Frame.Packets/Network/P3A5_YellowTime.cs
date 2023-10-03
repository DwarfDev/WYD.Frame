using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x3A5)]
public struct P3A5_YellowTime : IPacket
{
    // Atributos
    public NetworkHeader Header { get; set; } // 0 a 11		= 12
    public int Seconds;
}