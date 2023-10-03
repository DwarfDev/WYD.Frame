using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct PD1D_ServerSMS : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
    public string Nickname;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Message;
}