using WYD.Frame.Models.Models;

namespace WYD.Frame.Services.Models.Lan;

public class LanSpot
{
    public int SpotX { get; set; }
    public int SpotY { get; set; }
    public int SpotId { get; set; }

    public List<string> OccupiedBy { get; set; } = new List<string>();

    public Position Position { get; set; } = new();
}