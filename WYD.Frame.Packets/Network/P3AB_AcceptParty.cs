using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x3AB)]
public struct P3AB_AcceptParty : IPacket
{
    public NetworkHeader Header { get; set; }
    public ushort LeaderId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
    public string LeaderName;

    public static P3AB_AcceptParty Create( ushort leaderId, string leaderName)
    {
        return new P3AB_AcceptParty()
        {
            LeaderId = leaderId,
            LeaderName =  leaderName
        };
    }

}