using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Models;
using WYD.Frame.Game.Models.Game;

namespace WYD.Frame.Game.Helpers;

public static class BagHelper
{
    public static int FreeSlotCount(this List<BagItem> bag, int extraPagCount)
    {
        var invSize = 30 + (extraPagCount * 15);
        return bag.Count(x => x.Item.Id == 0 && x.Slot < invSize);
    }

    public static int ItemAmount(this List<BagItem> bag, int itemId)
    {
        var count = 0;

        foreach (var item in bag)
        {
            if (item.Item.Id == itemId)
            {
                var itemEf = item.Item.Ef.FirstOrDefault(x => x.Type == GameEfv.EF_AMOUNT).Value;
                var sum = itemEf == 0 ? 1 : itemEf;
                count += sum;
            }
        }

        return count;
    }

    public static int FirstFreeeSlot(this List<BagItem> bag)
    {
        return bag.FirstOrDefault(x => x.Item.Id == 0)?.Slot ?? -1;
    }

    public static int FirstFreeeSlot(this List<BagItem> bag, int ignoreSlot)
    {
        return bag.FirstOrDefault(x => x.Item.Id == 0 && x.Slot != ignoreSlot)?.Slot ?? -1;
    }

    public static int FirstFreeeSlot(this List<BagItem> bag, List<int> ignoredSlots)
    {
        return bag.FirstOrDefault(x => x.Item.Id == 0 && !ignoredSlots.Contains(x.Slot))?.Slot ?? -1;
    }
}