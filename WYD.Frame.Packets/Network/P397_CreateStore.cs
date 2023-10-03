using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x397)]
public struct P397_CreateStore : IPacket
{
    // Atributos
    public NetworkHeader Header { get; set; } // 0 a 11		= 12

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string Name;

    public int Unk1;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public NetworkItem[] Items;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] Unk2;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] Slots;

    public short Unk3;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] Prices;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] Unk4;

    public ushort ClientId;

    public static P397_CreateStore Create(NetworkHeader header, string storeName, NetworkItem networkItem, byte slot,
        int price, ushort clientId)
    {
        var items = new NetworkItem[10];
        items[0] = networkItem;

        var prices = new int[10];
        prices[0] = price;

        var slots = new byte[10];
        slots[0] = slot;

        return new P397_CreateStore
        {
            Header = header,
            Name = storeName,
            Items = items,
            Prices = prices,
            Slots = slots,
            ClientId = clientId
        };
    }
}