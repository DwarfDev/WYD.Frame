using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x333)]
public struct P333_SendChat : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Message;

    public static P333_SendChat Create(string message)
    {
        return new P333_SendChat
        {
            Message = message
        };
    }
}