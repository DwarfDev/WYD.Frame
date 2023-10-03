using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x337)]
public struct P337_UpdateScoreEtc
{
    public NetworkHeader Header;
    public int FakeExp;
    public long Exp;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] LearnedSkill;
    public ushort ScoreBonus;
    public ushort SpecialBonus;
    public ushort SkillBonus;
    public int Coin;
}