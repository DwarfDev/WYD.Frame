using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x182)]
public struct P182_RcvItem : IPacket
{
    public NetworkHeader Header { get; set; }
    public StorageType Type;
    public byte Unk;
    public short Slot;
    public NetworkItem NetworkItem;
}