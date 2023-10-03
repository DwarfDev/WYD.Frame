using WYD.Frame.Common.Enum;
using WYD.Frame.Models.Models;
using WYD.Frame.Services.Common;

namespace WYD.Frame.Services.Models.Location;

public class Area
{
    public AreaType Type { get; set; }
    public AreaNames Name { get; set; }

    public CommonGoods CommonGoods { get; set; }
    public Position CenterPosition
    {
        get
        {
            var centerX = Bounds.Max.X - Bounds.Min.X;
            centerX += Bounds.Min.X + centerX / 2 ;
            
            var centerY = Bounds.Max.Y - Bounds.Min.Y;
            centerY += Bounds.Min.Y + centerY / 2 ;
            return new Position((short)centerX, (short)centerY);
        }
    }

    public Position ReduceToArea(Position pos)
    {
        var newPos = new Position((short)(pos.X - Bounds.Min.X),
            (short)(pos.Y - Bounds.Min.Y));
        
        if (newPos.X < 0)
            newPos.X *= -1;

        if (newPos.Y < 0)
            newPos.Y *= -1;

        return newPos;
    }

    public AreaRect Bounds { get; set; }
    public List<Portal> Portals { get; set; } = new();
    public List<AreaExit> Exits { get; set; } = new();

    public void AddPortalBulk(List<Portal> portals)
    {
        Portals.AddRange(portals);
    }
}

public class AreaExit
{
    public Area Destination { get; set; }
    public Position Exit { get; set; }
    public ExitType ExitType { get; set; } = ExitType.Walk;

    public AreaExit()
    {
        
    }
    public AreaExit(Area destination, Position exit)
    {
        Destination = destination;
        Exit = exit;
    }
    public AreaExit(Area destination, Position exit, ExitType exitType)
    {
        Destination = destination;
        Exit = exit;
        ExitType = exitType;
    }
}