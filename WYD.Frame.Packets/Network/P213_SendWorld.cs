using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 36)]
[Packet(Id = 0x213)]
public struct P213_SendWorld : IPacket
{
    public NetworkHeader Header { get; set; }
    public byte CharIndex;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
    public byte[] Unkw;
    
    public static P213_SendWorld Create(int charIndex)
    {
        var packet = new P213_SendWorld
        {
            CharIndex = (byte)charIndex
        };
        return packet;
    }
}