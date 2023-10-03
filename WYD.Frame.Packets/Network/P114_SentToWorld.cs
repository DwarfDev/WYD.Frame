using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x114)]
public struct P114_SentToWorld : IPacket
{
    public NetworkHeader Header { get; set; } // 0 a 11				= 12

    public NetworkPosition WorldNetworkPosition { get; set; } // 12 a 15			= 4

    public NetworkCharMob Player; // 16 a 1351		= 1336

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
    public byte[] Unk1; // 1352 a 1711	= 360
}