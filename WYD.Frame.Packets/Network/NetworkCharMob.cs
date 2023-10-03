using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkCharMob
{
    // Atributos
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Name; // 0 a 15				= 16 // Correto para o WYD

    public byte CapeInfo; // 16						= 1

    public byte Merchant; // 17						= 1

    public short GuildIndex; // 18 a 19			= 2

    public MobClass ClassInfo { get; set; } // 20						= 1

    public byte AffectInfo; // 21						= 1
    public short QuestInfo; // 22 a 23			= 2

    public long Gold { get; set; } // 24 a 27			= 16 // novo        

    public long Exp { get; set; } // 32 a 39			= 8

    public NetworkPosition WorldNetworkPosition { get; set; } // 40 a 43			= 4

    public NetworkCharStatus BaseStatus { get; set; }
    public NetworkCharStatus PlayerStatus { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
    public NetworkItem[] Equip; // 140 a 267		= 144

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
    public NetworkItem[] EntityInventory; // 268 a 747		= 480

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public NetworkItem[] Andarilho; // 748 a 763		= 16

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] LearnedSkill;

    public ushort StatusPoint; // 788 a 789		= 2
    public ushort MasterPoint; // 790 a 791		= 2
    public ushort SkillPoint; // 792 a 793		= 2

    public byte SaveMana; // 794					= 1
    public byte Critical; // 795					= 1

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
    public byte[] Unknown8; // 796 a 799		= 4

    public ushort ClientId; // 1026 a 1027	= 2
    public short CityId; // 1028 a 1029	= 2

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] SkillBar2; // 1030 a 1045	= 16

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Unk7; // 1046 a 1047	= 2

    public uint Hold; // 1048 a 1051	= 4

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
    public byte[] TabBytes; // 1052 a 1077	= 26

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public NetworkAffect[] Affects; // 1080 a 1335	= 256
}