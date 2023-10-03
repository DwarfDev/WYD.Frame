using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkQuizOption
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] Text;
}