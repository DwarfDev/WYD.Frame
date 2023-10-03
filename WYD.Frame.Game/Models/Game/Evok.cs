namespace WYD.Frame.Game.Models.Game;

public class Evok
{
    public Evok(int packetOwner, int packetEvokId)
    {
        Owner = packetOwner;
        EvokId = packetEvokId;
    }

    public int Owner { get; set; }
    public int EvokId { get; set; }
}