using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct NetworkMob
{
    public NetworkPosition NetworkPosition;

    public ushort ClientId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Name;

    public short Face;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public short[] Equips; // Equips é maior pra esse server parece...

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public short[] Affects;

    public NetworkCharStatus Status { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] Unk1;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28)]
    public string Tab;
}