using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x383)]
public struct P383_Trade : IPacket
{
    public NetworkHeader Header { get; set; }
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst= 15)]
    public NetworkItem[] Items;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst= 15)]
    public byte[] CarryPos;

    public byte Unkw2;
    
    public int Gold;
    public short MyCheck;

    public ushort Target;

    public static P383_Trade Create(ushort targetId, int gold, NetworkItem[] items, byte[] slots, bool myCheck = false)
    {
        return new P383_Trade()
        {
            MyCheck = myCheck ? (byte)1: (byte)0,
            Gold =  gold,
            Target =  targetId,
            Items = items,
            CarryPos = slots
        };
    }
}