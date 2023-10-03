using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x37F)]
public struct P37F_PartyRequest : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkParty Party;
    public int TargetId;

    public static P37F_PartyRequest Create(NetworkParty party, int targetId)
    {
        return new P37F_PartyRequest()
        {
            Party = party,
            TargetId = targetId
        };
    }
}