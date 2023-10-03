using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x334)]
public struct P334_SendChat : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Command;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string Message;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] Unknown;

    public static P334_SendChat Create(string message, string command)
    {
        return new P334_SendChat
        {
            Command = command,
            Message = message
        };
    }
}