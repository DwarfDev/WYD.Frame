using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x289)]
public struct P289_Revive : IPacket
{
    public NetworkHeader Header { get; set; }
}