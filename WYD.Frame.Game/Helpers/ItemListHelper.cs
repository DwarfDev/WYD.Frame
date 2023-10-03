using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Services.Models.Resource;

namespace WYD.Frame.Game.Helpers;

public static class ItemListHelper
{
    public static TStaticEffect GetItemEfv(this TItemList item, GameEfv efvSearch)
    {
        return item.stEffect.First(x => x.sEffect == (short)efvSearch);
    }
}