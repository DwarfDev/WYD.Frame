using WYD.Frame.Models.Models;
using WYD.Frame.Packets;

namespace WYD.Frame.Services.Models.Npc;

public class Npc
{
    public Mob Mob { get; set; }
    public List<ShopItem> Shop { get; set; } = new();
    public bool Discovered { get; set; }
}