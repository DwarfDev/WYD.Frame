using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Models.Models;

public class PlayerStatus
{
    public int Level { get; set; }
    public int Defesa { get; set; }
    public int Ataque { get; set; }
    public int CurHp { get; set; }
    public int CurMp { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public short Str { get; set; }
    public short Int { get; set; }
    public short Dex { get; set; }
    public short Con { get; set; }

    public short[] Master { get; set; }
    
    public ushort StatusPoint{ get; set; } // 788 a 789		= 2
    public ushort MasterPoint{ get; set; } // 790 a 791		= 2
    public ushort SkillPoint{ get; set; }
    
    public byte Race { get; set; }
    public byte Merchant { get; set; }

    public bool Dead => CurHp <= 0;

    public static PlayerStatus Create(NetworkCharMob mob)
    {
        return new PlayerStatus()
        {
            Level = mob.PlayerStatus.Level,
            SkillPoint = mob.SkillPoint,
            StatusPoint = mob.StatusPoint,
            MasterPoint = mob.MasterPoint,
            Ataque = mob.PlayerStatus.Ataque,
            Defesa = mob.PlayerStatus.Defesa,
            CurHp = mob.PlayerStatus.CurHp,
            CurMp = mob.PlayerStatus.CurMp,
            MaxHp = mob.PlayerStatus.MaxHp,
            MaxMp = mob.PlayerStatus.MaxMp,
            Str = mob.PlayerStatus.Str,
            Int = mob.PlayerStatus.Int,
            Dex = mob.PlayerStatus.Dex,
            Con = mob.PlayerStatus.Con,
            Master = mob.PlayerStatus.Master,
            Race = mob.PlayerStatus.Race,
            Merchant= mob.PlayerStatus.Merchant,
        };
    }
    
    public static PlayerStatus Create(NetworkCharStatus mob)
    {
        return new PlayerStatus()
        {
            Level = mob.Level,
            Ataque = mob.Ataque,
            Defesa = mob.Defesa,
            CurHp = mob.CurHp,
            CurMp = mob.CurMp,
            MaxHp = mob.MaxHp,
            MaxMp = mob.MaxMp,
            Str = mob.Str,
            Int = mob.Int,
            Dex = mob.Dex,
            Con = mob.Con,
            Master = mob.Master,
            Race = mob.Race,
            Merchant = mob.Merchant
        };
    }

    public void UpdateStatus(NetworkCharStatus status)
    {
        Level = status.Level;
        Ataque = status.Ataque;
        Defesa = status.Defesa;
        CurHp = status.CurHp;
        CurMp = status.CurMp;
        MaxHp = status.MaxHp;
        MaxMp = status.MaxMp;
        Str = status.Str;
        Int = status.Int;
        Dex = status.Dex;
        Con = status.Con;
        Master = status.Master;
        Race = status.Race;
        Merchant = status.Merchant;
    }
}