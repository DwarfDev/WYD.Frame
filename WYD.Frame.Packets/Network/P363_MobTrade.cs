using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x363)]
public struct P363_MobTrade : IPacket
{
    public NetworkHeader Header { get; set; }
    public short PosX;
    public short PosY;
    public ushort ClientId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string MobName;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public ushort[] Equip;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public ushort[] Affects;

    public ushort Guild;
    public char GuildLevel;
    public NetworkCharStatus Score;
    public ushort CreateType;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] Equip2;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 26)]
    public string Nick;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
    public string Tab;

    public byte Server;

    public static P36C_Move Create(NetworkPosition to, NetworkPosition from, float moveSpeed)
    {
        var packet = new P36C_Move();

        packet.From = from;
        packet.To = to;
        packet.Speed = (int)moveSpeed;
        packet.Type = 0;

        return packet;
    }
}