using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkItemEf
{
    // Atributos
    public GameEfv Type; // 0
    public byte Value; // 1
}