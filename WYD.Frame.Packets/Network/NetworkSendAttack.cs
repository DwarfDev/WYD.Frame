using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkSendAttack
{
    public long FakeExp;//zera
    public int ReqMp;//zera
    public long CurrentExp;//zera
    public short Rsv; //zera

    public NetworkPosition AttackerPos; // 32 - 35
    public NetworkPosition TargetPos; // 36 - 39
    public ushort AttackerId; // 42 - 43
    public short Progress;
    public byte Motion;
    public byte Local;
    public byte DoubleCritical;
    public byte SkillParm;
    public short Unkw; //Zerado
    public uint CurrentMana;
    public GameSkills SkillId;

    public static NetworkSendAttack Create(ushort attackerId, NetworkPosition attackerPos, NetworkPosition targetPos,
        GameSkills skill)
    {
        return new NetworkSendAttack
        {
            Motion = (byte)(skill == GameSkills.Ranged || skill == GameSkills.Mlee ? 6 : 255),
            SkillId = skill,
            Rsv = (byte)(skill == GameSkills.Ranged || skill == GameSkills.Mlee ? 0 : 0),
            AttackerPos = attackerPos,
            TargetPos = targetPos,
            AttackerId = attackerId,
            DoubleCritical = 0,
            CurrentMana = uint.MaxValue
        };
    }
}