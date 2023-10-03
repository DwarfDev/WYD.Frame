using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkCharStatus
{
    public int Level;
    public int Defesa;
    public int Ataque;

    public byte Race;
    public byte Merchant;

    public short Unk;

    public int CurHp { get; set; }
    public int CurMp { get; set; }
    public int MaxHp;
    public int MaxMp;

    public short Str;
    public short Int;
    public short Dex;
    public short Con;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public short[] Master; // 40 a 47	= 8
}