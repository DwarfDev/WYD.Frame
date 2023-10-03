using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x10A)]
public struct P10A_CharList : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] HashKeys;

    public NetworkCharlist NetworkCharlist;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
    public NetworkItem[] Cargo;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public byte[] Unk2;

    public long Gold;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] UserNameBytes;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1424)]
    public byte[] Unk3;
}