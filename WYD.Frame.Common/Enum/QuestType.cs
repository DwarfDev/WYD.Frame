namespace WYD.Frame.Common.Enum;

public enum QuestType
{
    LanN = 0,
    [NpcName("Coveiro")]
    Coveiro = 1,
    [NpcName("Jardineiro")]
    Jardim = 2,
    [NpcName("Batedor")]
    Kaizen = 3,
    [NpcName("Guarda")]
    Hidra = 4,
    [NpcName("Representante")]
    Elfos = 5
}


public class NpcName : Attribute
{
    public string Name;
    public NpcName(string name)
    {
        Name = name;
    }
}