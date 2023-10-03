using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x37D)]
public struct P37D_PartyJoined : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkParty Party;
}