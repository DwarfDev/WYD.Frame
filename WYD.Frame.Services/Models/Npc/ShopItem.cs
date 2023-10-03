using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Services.Models.Npc;

public class ShopItem
{
    public NetworkItem Item { get; set; } = new();
    public int CarrierSlot { get; set; }
}