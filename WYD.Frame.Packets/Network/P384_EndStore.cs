using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x384)]
public struct P384_EndStore : IPacket
{
    // Atributos
    public NetworkHeader Header { get; set; } // 0 a 11		= 12
}