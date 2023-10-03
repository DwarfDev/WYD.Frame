using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[Packet(Id = 0x336)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct P336_UpdateScore : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkCharStatus Status { get; set; }
    public byte Critical;
    public byte SaveMana;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public ushort[] Affect;

    public ushort Guild;
    public ushort GuildLevel;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Resist;

    public int ReqHp;
    public int ReqMp;
    public ushort Magic;
    public ushort Rsv;
    public byte LearnedSkill;
}