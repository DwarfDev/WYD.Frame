using WYD.Frame.Models.Models;

namespace WYD.Frame.Services.Models.Location;

public class HuntingScroll
{
    public int ItemId { get; set; }
    public int Index { get; set; }
    public double Range { get; set; }
    public Position Destination { get; set; }
}