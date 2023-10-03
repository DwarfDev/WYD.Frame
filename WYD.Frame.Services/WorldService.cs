using WYD.Frame.Common;
using WYD.Frame.Models.Models;
using WYD.Frame.Services.Models;
using WYD.Frame.Services.Models.World;

namespace WYD.Frame.Services;

public static class WorldService
{
    public static AreaRect RutiAreaRect => new()
    {
        Max = new Position()
        {
            X = 2487, Y = 1709
        },
        Min = new Position()
        {
            X = 2481, Y = 1698
        }
    };
    public static AreaRect ArmiaCenterRect => new()
    {
        Max = new Position()
        {
            X = 2102, Y = 2104
        },
        Min = new Position()
        {
            X = 2092, Y = 2096
        }
    };
    public static AreaRect NpcEventoTeleportRect => new()
    {
        Max = new Position()
        {
            X = 2088, Y = 2124
        },
        Min = new Position()
        {
            X = 2078, Y = 2112
        }
    };

    public static AreaRect NpcEventoTradeRect => new()
    {
        Max = new Position()
        {
            X = 2125, Y = 2127
        },
        Min = new Position()
        {
            X = 2119, Y = 2122
        }
    };
    public static AreaRect ParcheAreaRect => new()
    {
        Max = new Position()
        {
            X = 2468, Y = 2011
        },
        Min = new Position()
        {
            X = 2462, Y = 2000
        }
    };

    public static AreaRect AkiAreaRect => new()
    {
        Max = new Position()
        {
            X = 2140, Y = 2094
        },
        Min = new Position()
        {
            X = 2139, Y = 2087
        }
    };

    public static AreaRect NpcEventoRect => new()
    {
        Max = new Position()
        {
            X = 2097, Y = 2109,
        },
        Min = new Position()
        {
            X = 2092, Y = 2108
        }
    };

    public static Position ArmiaCenter => new()
    {
        X = 2100, Y = 2100
    };

    public static Position CMontariaPosition => new()
    {
        X = 2466, Y = 2015
    };

    public static Position RandomByArea(AreaRect rect)
    {
        var x = Utils.Rand.Next(rect.Min.X, rect.Max.X + 1);
        var y = Utils.Rand.Next(rect.Min.Y, rect.Max.Y + 1);

        return new Position() { X = (short)x, Y = (short)y };
    }

    /// <summary>
    ///     Pots, lans, returns
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static GoodInfo? ClosestCommonGoods(Position pos)
    {
        var commonGoodsStores = new List<GoodInfo>
        {
            new()
            {
                NpcName = "Aki",
                Rect = AkiAreaRect
            },
            new()
            {
                NpcName = "Parche",
                Rect = ParcheAreaRect
            },
            new()
            {
                NpcName = "Ruti",
                Rect = RutiAreaRect
            }
        };

        return commonGoodsStores.MinBy(x => x.Rect.Min.GetDistance(pos));
    }

    public static bool IsOnArea(Position pos, AreaRect rect)
    {
        return pos.X >= rect.Min.X && pos.Y >= rect.Min.Y &&
               pos.X <= rect.Max.X && pos.Y <= rect.Max.Y;
    }
}