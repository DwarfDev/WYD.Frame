using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x17C)]
public struct P17C_ShopList
{
    public NetworkHeader Header;
    
    public int ShopType;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
    public NetworkItem[] Items;

    public int Tax;
}