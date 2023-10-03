using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x367)]
public struct P367_SendMultiAtk : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkSendAttack Info;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
    public NetworkAttackTarget[] Target;

    public int Unkw;

    public static P367_SendMultiAtk Create(ushort[] targetIds, NetworkSendAttack info)
    {
        return new P367_SendMultiAtk
        {
            Info = info,
            Target = targetIds.Select(x => new NetworkAttackTarget
            {
                ClientId = x,
                Damage = x != default ? uint.MaxValue : default
            }).ToArray()
        };
    }
}