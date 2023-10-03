using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Packets.Network;
using WYD.Frame.Services;

namespace WYD.Frame.Game.Models.Game;

public class BagItem
{
    public NetworkItem Item { get; set; }
    public StorageType StorageType { get; set; }
    public byte Slot { get; set; }

    public string GetName()
    {
        return ResourceService.Items[Item.Id].Name ?? "";
    }

    public int Amount()
    {
        if (Item.Ef.Any(x => x.Type == GameEfv.EF_AMOUNT))
            return Item.Ef.First(x => x.Type == GameEfv.EF_AMOUNT).Value;
        return 1;
    }

    public void Decrement()
    {
        for (var i = 0; i < Item.Ef.Length; i++)
            if (Item.Ef[i].Type == GameEfv.EF_AMOUNT)
            {
                if (Item.Ef[i].Value <= 1)
                {
                    break;
                }

                Item.Ef[i].Value--;
                return;
            }

        Item = new NetworkItem
        {
            Ef = Enumerable.Range(0, 3).Select(x => new NetworkItemEf
            {
                Type = default,
                Value = default
            }).ToArray(),
            Id = default
        };
    }
}