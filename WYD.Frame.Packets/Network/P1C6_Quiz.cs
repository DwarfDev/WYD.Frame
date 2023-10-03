using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x1C6)]
public struct P1C6_Quiz : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    public byte[] Title;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public NetworkQuizOption[] Options;

    public int Unkw { get; set; }
}

