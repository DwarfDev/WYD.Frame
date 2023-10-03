using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x37E)]
public struct P37E_RemoveEvoks : IPacket
{
    public NetworkHeader Header { get; set; }
}