using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x338)]
public struct P338_MobDeath : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Hold;
    public ushort Killed;
    public ushort Killer;
    public int unknown;
    public ulong Exp;
}