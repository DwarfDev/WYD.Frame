using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x181)]
public struct P181_UpdateHpMp : IPacket
{
    public NetworkHeader Header { get; set; }
    public int FromHP;
    public int FromMP;
    public int ToHP;
    public int ToMP;
}