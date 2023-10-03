namespace WYD.Frame.Models.Models;

public class GeneralConfig
{
    public string Id { get; set; }
    public BehaviorConfig Behavior { get; set; } = new();
}

public class BehaviorConfig
{
    public bool ReviveRandom { get; set; } = true;
    public int ReviveAfterSeconds { get; set; }
    public bool NotifyGuilded { get; set; } = true;
    public bool TurnoffWorkersOnDeath { get; set; } = false;
}