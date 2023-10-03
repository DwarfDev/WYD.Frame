using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x28B)]
public struct P28B_Click : IPacket
{
    public NetworkHeader Header { get; set; }
    public ushort NpcIndex;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public byte[] Unk;

    public static P28B_Click Create(ushort npcIndex)
    {
        return new P28B_Click
        {
            NpcIndex = npcIndex
        };
    }
}