using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct P39E_SendTwoAtk : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkSendAttack Info;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public NetworkAttackTarget[] Target;
}

