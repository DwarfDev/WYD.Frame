using WYD.Frame.Models.Models;

namespace WYD.Frame.Services.Models.Location;

public class CommonGoods
{
    public Position Position { get; set; }
    public AreaRect AreaRect { get; set; }
    public string Name { get; set; }
    public bool SellsLan { get; set; }

    public CommonGoods(string name, Position position, AreaRect rect, bool sellsLan)
    {
        Position = position;
        Name = name;
        SellsLan = sellsLan;
        AreaRect = rect;
    }
}