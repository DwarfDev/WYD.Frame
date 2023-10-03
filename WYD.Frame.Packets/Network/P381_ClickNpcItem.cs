using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x381)]
public struct P381_ClickNpcItem : IPacket
{
    //18 00 1A 84 81 03 BE 01 0D 11 9C 24
    //97 0E 00 00 1C 00 00 00 56 6B 19 00


    public NetworkHeader Header { get; set; }
    public int NpcIndex;
    public int NpcSlot;
    public int Unkw3;

    public static P381_ClickNpcItem Create(int npcIndex, int npcSlot)
    {
        return new P381_ClickNpcItem()
        {
            NpcIndex = npcIndex,
            NpcSlot = npcSlot
        };
    }
}