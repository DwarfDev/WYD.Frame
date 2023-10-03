using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkAttackTarget
{
    public int ClientId;
    public uint Damage;
}