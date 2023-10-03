using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x2E4)]
public struct P2E4_DeleteItem : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Slot;
    public int Id;

    public static P2E4_DeleteItem Create(int slot, int id)
    {
        return new P2E4_DeleteItem
        {
            Slot = slot,
            Id = id
        };
    }
}