using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x366)]
public struct P366_Stop : IPacket
{
    public NetworkHeader Header { get; set; }

    public NetworkPosition From;

    public int Type;
    public int Speed;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] Directions;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] Unk1;

    public NetworkPosition To { get; set; }

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