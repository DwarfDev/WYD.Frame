using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x376)]
public struct P376_MovedItem : IPacket
{
    public NetworkHeader Header { get; set; }
    public StorageType DstType; // 01 inventario 02 cargo
    public byte SlotDst; // slot de destino
    public StorageType SrcType; // 01 inventario 02 cargo
    public byte SlotOrigin; // slot de origem
    public int NpcIndex;

    //14 00 2B FC 76 03 14 00 AC D4 57 28 01 08 01 09 00 00 00 00

    public static P376_MovedItem Create(StorageType dstType, byte dstSlot, StorageType originType,
        byte originSlot, int npcIndex)
    {
        return new P376_MovedItem
        {
            SlotOrigin = originSlot,
            SlotDst = dstSlot,
            DstType = dstType,
            SrcType = originType,
            NpcIndex = npcIndex
        };
    }
}