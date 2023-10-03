using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkPosition
{
    public short X { get; set; } // 0 a 1	= 2
    public short Y { get; set; } // 2 a 3	= 2

    public int GetDistance(NetworkPosition target)
    {
        return (int)Math.Sqrt(
            (target.X - X) * (target.X - X) +
            (target.Y - Y) * (target.Y - Y));
    }

    public double GetAngle(NetworkPosition destiny)
    {
        var x = destiny.X - X;
        var y = destiny.Y - Y;
        var radian = Math.Atan2(y, x);

        return radian;
    }

    public NetworkPosition GetPositionByAngleAndDistance(double distance, double angle)
    {
        var x1 = Convert.ToInt32(distance * Math.Cos(angle));
        var y1 = Convert.ToInt32(distance * Math.Sin(angle));

        return new NetworkPosition
        {
            X = Convert.ToInt16(x1 + X),
            Y = Convert.ToInt16(y1 + Y)
        };
    }

    public NetworkPosition Clone()
    {
        return new NetworkPosition
        {
            X = X,
            Y = Y
        };
    }
}