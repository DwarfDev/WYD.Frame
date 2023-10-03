using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x185)]
public struct P185_UpdateInventory
{
    public NetworkHeader Header;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
    public NetworkItem[] Inventory;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public NetworkItem[] Special; // bolsas etc  prob

    //Talvez gold e mais algumas coisas
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] Unk;
}