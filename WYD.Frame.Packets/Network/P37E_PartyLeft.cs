using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x37E)]
public struct P37E_PartyLeft : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Index { get; set; }

    public static P37E_PartyLeft Create(int leaverId)
    {
        return new P37E_PartyLeft()
        {
            Index = leaverId
        };
    }
}