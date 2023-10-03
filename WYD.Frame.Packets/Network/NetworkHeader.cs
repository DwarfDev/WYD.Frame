using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct NetworkHeader
{
    public ushort Size;
    public byte Key;
    public byte Checksum;
    public ushort PacketId;
    public ushort ClientId;
    public uint Timestamp;

    public static NetworkHeader Create<T>(ushort packetId, uint timestamp, byte key, ushort clientId = 0)
    {
        return new NetworkHeader
        {
            Size = (ushort)Marshal.SizeOf<T>(),
            Checksum = 0,
            PacketId = packetId,
            Timestamp = timestamp,
            Key = key,
            ClientId = clientId
        };
    }
}