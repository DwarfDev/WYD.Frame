using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[Packet(Id = 0x27B)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct P27B_ReqShopList : IPacket
{
    public NetworkHeader Header { get; set; }
    public ushort NpcIndex;
    public ushort Unkw;
    

    public static P27B_ReqShopList Create(ushort npcIndex)
    {
        return new P27B_ReqShopList
        {
            NpcIndex = npcIndex
        };
    }
}