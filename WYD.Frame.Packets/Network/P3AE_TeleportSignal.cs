using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x3AE)]
public struct P3AE_TeleportSignal : IPacket
{
    public NetworkHeader Header { get; set; }
    public int WarpIndex;

    public static P3AE_TeleportSignal Create(int warpIndex)
    {
        return new P3AE_TeleportSignal
        {
            WarpIndex = warpIndex
        };
    }
}