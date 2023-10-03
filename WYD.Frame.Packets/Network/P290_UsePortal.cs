using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x290)]
public struct P290_UsePortal : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Unk;

    public static P290_UsePortal Create(NetworkHeader header)
    {
        return new P290_UsePortal
        {
            Header = header
        };
    }
}