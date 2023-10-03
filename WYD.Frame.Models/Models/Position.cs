using WYD.Frame.Common;

namespace WYD.Frame.Models.Models;

public class Position
{
    public short X { get; set; }
    public short Y { get; set; }

    public Position()
    {
        
    }
    public Position(short x, short y)
    {
        X = x;
        Y = y;
    }
    
    public int GetDistance(Position target)
    {
        return (int)Math.Sqrt(
            (target.X - X) * (target.X - X) +
            (target.Y - Y) * (target.Y - Y));
    }

    public double GetAngle(Position destiny)
    {
        var x = destiny.X - X;
        var y = destiny.Y - Y;
        var radian = Math.Atan2(y, x);

        return radian;
    }

    public Position RandomByRange(int range)
    {
        var positions = PointsByRadiusAndAngle(this, range).OrderBy(x => x.GetDistance(this)).ToList();

        var rnd = Utils.Rand.Next(positions.Count, positions.Count);

        return positions[rnd];
    }
    
    private List<Position> PointsByRadiusAndAngle(Position orginalPos, int radius)
    {
        var positionsList = new List<Position>();
        for (var x = -radius; x <= radius; x++)
        for (var y = -radius; y <= radius; y++)
            positionsList.Add(new Position
            {
                X = (short)(orginalPos.X + radius * Math.Cos(x)),
                Y = (short)(orginalPos.Y + radius * Math.Sin(y))
            });

        return positionsList;
        // return new Position { X = (short)(r * Math.Cos(angle) + orginalPos.X), Y = (short)(r * Math.Sin(angle) + orginalPos.Y) };;
    }

    public Position GetPositionByAngleAndDistance(double distance, double angle)
    {
        var x1 = Convert.ToInt32(distance * Math.Cos(angle));
        var y1 = Convert.ToInt32(distance * Math.Sin(angle));

        return new Position
        {
            X = Convert.ToInt16(x1 + X),
            Y = Convert.ToInt16(y1 + Y)
        };
    }

    public Position Clone()
    {
        return new Position
        {
            X = X,
            Y = Y
        };
    }
}