using System.Diagnostics;
using AStar;
using WYD.Frame.Packets;
using Position = WYD.Frame.Models.Models.Position;

namespace WYD.Frame.Services;

public static class PathingService
{
    public static Position[] FindPath(PathFinder finder, Position origin, Position dest)
    {
        if (origin.GetDistance(dest) > 1000)
            return new[] { origin };

        Console.WriteLine(
            $"Initializing Path finding with ram at {Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024} Mb");
        var path = finder.FindPath(new AStar.Position(origin.X, origin.Y), new AStar.Position(dest.X, dest.Y));
        Console.WriteLine(
            $"Path finding done with ram at {Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024} Mb");

        if (path is null) return new Position[] { origin };

        return path.Select(x => new Position((short)(x.Row), (short)(x.Column))).ToArray();
    }

    public static Position[] NormalizePath(Position[] path)
    {
        var newPathing = new List<Position>();

        var currentPos = path[0];
        var currentIndex = 1;
        
        while (true)
        {
            if (currentIndex >= path.Length) break;

            if (currentPos.GetDistance(path[currentIndex]) >= 9)
            {
                newPathing.Add(path[currentIndex]);
                currentPos = path[currentIndex];
            }
            
            currentIndex++;
        }

        return newPathing.ToArray();
    }


    public static byte[]? GetRoute(int x, int y, int targetx, int targety, int distance)
    {
        int lastx = x;
        int lasty = y;
        int tx = targetx;
        int ty = targety;
        byte[] Route = new byte[24];
        int MH = 8;

        for (int i = 0; i < distance && i < Route.Length; i++)
        {
            if (x - 0 < 1 || y - 0 < 1 || x - 0 > 4096 - 2 || y - 0 > 4096 - 2)
            {
                Route[i] = 0;
                break;
            }

            int cul = MapService.HeightMapBytes[((y - 0) * 4096 + x - 0)];

            int n = MapService.HeightMapBytes[(y - 0 - 1) * 4096 + x - 0];
            int ne = MapService.HeightMapBytes[(y - 0 - 1) * 4096 + x - 0 + 1];
            int e = MapService.HeightMapBytes[(y - 0) * 4096 + x - 0 + 1];
            int se = MapService.HeightMapBytes[(y - 0 + 1) * 4096 + x - 0 + 1];
            int s = MapService.HeightMapBytes[(y - 0 + 1) * 4096 + x - 0];
            int sw = MapService.HeightMapBytes[(y - 0 + 1) * 4096 + x - 0 - 1];
            int w = MapService.HeightMapBytes[(y - 0) * 4096 + x - 0 - 1];
            int nw = MapService.HeightMapBytes[(y - 0 - 1) * 4096 + x - 0 - 1];

            if (tx == x && ty == y)
            {
                Route[i] = 0;
                break;
            }

            if (tx == x && ty < y && n < cul + MH && n > cul - MH)
            {
                Route[i] = 2;
                y--;

                continue;
            }

            if (tx > x && ty < y && ne < cul + MH && ne > cul - MH)
            {
                Route[i] = 3;

                x++;
                y--;

                continue;
            }

            if (tx > x && ty == y && e < cul + MH && e > cul - MH)
            {
                Route[i] = 6;

                x++;

                continue;
            }

            if (tx > x && ty > y && se < cul + MH && se > cul - MH)
            {
                Route[i] = 9;

                x++;
                y++;

                continue;
            }

            if (tx == x && ty > y && s < cul + MH && s > cul - MH)
            {
                Route[i] = 8;

                y++;

                continue;
            }

            if (tx < x && ty > y && sw < cul + MH && sw > cul - MH)
            {
                Route[i] = 7;

                x--;
                y++;

                continue;
            }

            if (tx < x && ty == y && w < cul + MH && w > cul - MH)
            {
                Route[i] = 4;

                x--;

                continue;
            }

            if (tx < x && ty < y && nw < cul + MH && nw > cul - MH)
            {
                Route[i] = 1;

                x--;
                y--;

                continue;
            }

            if (tx > x && ty < y && e < cul + MH && e > cul - MH)
            {
                Route[i] = 6;

                x++;

                continue;
            }

            if (tx > x && ty < y && n < cul + MH && n > cul - MH)
            {
                Route[i] = 2;

                y--;

                continue;
            }

            if (tx > x && ty > y && e < cul + MH && e > cul - MH)
            {
                Route[i] = 6;

                x++;

                continue;
            }

            if (tx > x && ty > y && s < cul + MH && s > cul - MH)
            {
                Route[i] = 8;

                y++;

                continue;
            }

            if (tx < x && ty > y && w < cul + MH && w > cul - MH)
            {
                Route[i] = 4;

                x--;

                continue;
            }

            if (tx < x && ty > y && s < cul + MH && s > cul - MH)
            {
                Route[i] = 8;

                y++;

                continue;
            }

            if (tx < x && ty < y && w < cul + MH && w > cul - MH)
            {
                Route[i] = 4;

                x--;

                continue;
            }

            if (tx < x && ty < y && n < cul + MH && n > cul - MH)
            {
                Route[i] = 2;

                y--;

                continue;
            }

            if (tx == x + 1 || ty == y + 1 || tx == x - 1 || ty == y - 1)
            {
                Route[i] = 0;

                break;
            }

            if (tx == x && ty > y && se < cul + MH && se > cul - MH)
            {
                Route[i] = 9;

                x++;
                y++;

                continue;
            }

            if (tx == x && ty > y && sw < cul + MH && sw > cul - MH)
            {
                Route[i] = 7;

                x--;
                y++;

                continue;
            }

            if (tx == x && ty < y && ne < cul + MH && ne > cul - MH)
            {
                Route[i] = 3;

                x++;
                y--;

                continue;
            }

            if (tx == x && ty < y && nw < cul + MH && nw > cul - MH)
            {
                Route[i] = 1;

                x--;
                y--;

                continue;
            }

            if (tx < x && ty == y && sw < cul + MH && sw > cul - MH)
            {
                Route[i] = 7;

                x--;
                y++;

                continue;
            }

            if (tx < x && ty == y && nw < cul + MH && nw > cul - MH)
            {
                Route[i] = 1;

                x--;
                y--;

                continue;
            }

            if (tx > x && ty == y && se < cul + MH && se > cul - MH)
            {
                Route[i] = 9;

                x++;
                y++;

                continue;
            }

            if (tx > x && ty == y && ne < cul + MH && ne > cul - MH)
            {
                Route[i] = 3;

                x++;
                y--;

                continue;
            }

            Route[i] = 0;
            break;
        }

        if (lastx == x && lasty == y)
            return null;

        if (lastx == x && lasty == y)
            return null;

        return Route;
    }
}