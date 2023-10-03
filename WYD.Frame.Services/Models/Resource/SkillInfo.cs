using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Services.Models.Resource;

public class SkillInfo
{
    public GameSkills Id { get; set; }
    public string Name { get; set; }
    public MobClass MobClass { get; set; }
    public int SkillPoint{get;set;}
    public int TargetType{get;set;}
    public int ManaSpent{get;set;}
    public double Delay{get;set;}
    public int Range{get;set;}
    public int InstanceType{get;set;}
    public int InstanceValue{get;set;}
    public int TickType{get;set;}
    public int TickValue{get;set;}
    public int AffectType{get;set;}
    public int AffectValue{get;set;}
    public int AffectTime{get;set;}
    public byte[] Act1{get;set;}
    public byte[] Act2{get;set;}
    public int InstanceAttribute{get;set;}
    public int TickAttribute{get;set;}
    public bool Aggressive{get;set;}
    public int MaxTarget{get;set;}
    public int bParty{get;set;}
    public int AffectResist{get;set;}
    public bool Passive{get;set;}
    public int ForceDamage{get;set;}
}