using WYD.Frame.Models.Models;

namespace WYD.Frame.Services.Models;

public class AreaRect
{
    public Position Max { get; set; }
    public Position Min { get; set; }

    public AreaRect()
    {
        
    }
    public AreaRect(Position min, Position max)
    {
        Min = min;
        Max = max;
    }
    
    public bool IsOnArea(Position pos)
    {
        return pos.X >= Min.X && pos.Y >= Min.Y &&
               pos.X <= Max.X && pos.Y <= Max.Y;
    }
}