using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x373)]
public struct P373_UseItem : IPacket
{
    public NetworkHeader Header { get; set; }
    public StorageType SrcType;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Unkw;

    public int SrcSlot;
    public StorageType DstType;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Unkw2;

    public int DstSlot;
    public short PosX;
    public short PosY;
    public int Warp;

    public static P373_UseItem Create(StorageType srcType, StorageType dstType, int srcSlot,
        int dstSlot, NetworkPosition playerPos, int warp)
    {
        return new P373_UseItem
        {
            SrcSlot = srcSlot,
            PosX = playerPos.X,
            PosY = playerPos.Y,
            DstSlot = dstSlot,
            DstType = dstType,
            SrcType = srcType,
            Warp = warp
        };
    }
}