using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x2454)]
public struct P2454_AddEvok : IPacket
{
    public NetworkHeader Header { get; set; }

    public int Owner;
    public int EvokId;
}