using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[Packet(Id = 0x3B9)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct P3B9_AffectInfo : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public NetworkAffect[] Affects;

}