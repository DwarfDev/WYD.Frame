using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkAffect
{
    // Atributos
    public byte Type { get; set; } // 0			= 1
    public byte Master { get; set; } // 1			= 1
    public ushort Value { get; set; } // 2 a 3	= 2
    public uint Time { get; set; } // 4 a 7	= 4
}