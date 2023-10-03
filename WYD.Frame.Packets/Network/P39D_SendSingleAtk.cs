using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x39D)]
public struct P39D_SendSingleAtk : IPacket
{
    public NetworkHeader Header { get; set; }
    public NetworkSendAttack Info;
    public NetworkAttackTarget Target;
    public int Unkw;

    public static P39D_SendSingleAtk Create(ushort targetId, NetworkSendAttack info)
    {
        return new P39D_SendSingleAtk
        {
            Info = info,
            Target = new NetworkAttackTarget
            {
                ClientId = targetId,
                Damage = info.SkillId == GameSkills.Mlee || info.SkillId == GameSkills.Ranged
                    ? uint.MaxValue - 1
                    : uint.MaxValue
            }
        };
    }
}

