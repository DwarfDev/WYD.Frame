using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkCharname
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Name; // 0 a 15	= 16
}