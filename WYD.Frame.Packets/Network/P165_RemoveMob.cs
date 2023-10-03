using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x165)]
public struct P165_RemoveMob : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Reason;
}