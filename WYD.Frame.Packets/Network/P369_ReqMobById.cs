using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x369)]
public struct P369_ReqMobById : IPacket
{
    public NetworkHeader Header { get; set; }
    public ushort ClientId;

    public static P369_ReqMobById Create(ushort clientId)
    {
        var packet = new P369_ReqMobById();


        packet.ClientId = clientId;

        return packet;
    }
}