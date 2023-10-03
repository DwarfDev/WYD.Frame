using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkCharlist
{
    public uint Unk;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public short[] PosX; // 4 a 11			= 8

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public short[] PosY; // 12 a 19		= 8

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public NetworkCharname[] Name; // 20 a 83		= 64

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public NetworkCharStatus[] Status; // 84 a 275		= 192

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public NetworkCharEquips[] Equips; // 576 18 itens

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public ushort[] Guild; // 788 a 795	= 8

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public int[] Gold; // 796 a 811	= 32

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public long[] Exp; // 812 a 843	= 32
}