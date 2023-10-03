using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x379)]
public struct P379_BuyNpcItem : IPacket
{
    public NetworkHeader Header { get; set; }
    public ushort NpcIndex;
    public short NpcSlot;
    public short DestSlot;

    public ushort Unkw;
    public int Gold;

    public static P379_BuyNpcItem Create(short npcSlot, short destSlot, ushort npcIndex)
    {
        return new P379_BuyNpcItem
        {
            NpcIndex = npcIndex,
            NpcSlot = npcSlot,
            DestSlot = destSlot
        };
    }
}