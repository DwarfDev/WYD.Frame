using AStar;
using AStar.Options;
using WYD.Frame.Services.Models.Location;

namespace WYD.Frame.Conversors;

public class MapTest
{
    private static short[,] _heightMap = new short[1,1];

    private static byte[] _heightMapBytes = new byte[1];
    public static byte[] HeightMapBytes
    {
        get
        {
            if(_heightMapBytes.Length == 1)
                _heightMapBytes =  File.ReadAllBytes(Environment.CurrentDirectory + "/Resource/HeightMap.raw");

            return _heightMapBytes;
        }
    }

    public static short[,] HeightMapGrid
    {
        get
        {
            if (_heightMap.GetLength(0) <= 1)
            {
                _heightMap = GetHeightmap();
            }

            return _heightMap;
        }
    }

    private static readonly short[,] Attribute =
        GetAttribute(Environment.CurrentDirectory + "/Resource/AttributeMap.dat");

    private static short[,]? _grid;
    private static readonly object PathFinderLocker = new();
    private static readonly object GridLocker = new();

    public static short[,] Grid
    {
        get
        {
            lock (GridLocker)
            {
                return _grid ??= ConvertToGrid();
            }
        }
    }

    public static PathFinder CreateForNode(Area area)
    {
        var xBlocks = area.Bounds.Max.X - area.Bounds.Min.X + 1;
        var yBlocks = area.Bounds.Max.Y - area.Bounds.Min.Y + 1;
        
        var pathfinderOptions = new PathFinderOptions
        {
            PunishChangeDirection = true,
            UseDiagonals = true,
            SearchLimit = xBlocks * yBlocks
        };
        var grid = new short[xBlocks,yBlocks];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                var currentX = area.Bounds.Min.X + x;
                var currentY = area.Bounds.Min.Y + y;
                grid[x, y] = Grid[currentY, currentX];
            }
        }
                    
        var worldGrid = new WorldGrid(grid);
        
        return new PathFinder(worldGrid, pathfinderOptions);
    }

    
    private static short[,] ConvertToGrid()
    {
        var map = new short[4096, 4096];

        for (var x = 0; x < 4096; x++)
        {
            for (var y = 0; y < 4096; y++)
            {
                var zoneInfo = CanWalk(Attribute[x/4, y/4]);
                if (zoneInfo && HeightMapGrid[x, y] != 127 && HeightMapGrid[x, y] != 247) map[x, y] = 1;
            }
        }

        return map;
    }

    private static bool CanWalk(int value)
    {
        if (value == default) return true;

        while (true)
        {
            var logResult = Math.Log2(value);

            var exponent = Math.Floor(logResult);
            var pow = (int)Math.Pow(2, exponent);

            var zoneType = pow;
            if (zoneType == 2)
                return false;

            if (logResult % 1 == 0) break;

            value -= pow;
        }

        return true;
    }

    private static short[,] GetAttribute(string filePath)
    {
        var ba = File.ReadAllBytes(filePath);

        var mat = new short[1024, 1024];

        var x = 0;
        var y = 0;
        for (var i = 0; i < ba.Length; i++)
        {
            mat[x, y] = ba[x];

            y++;

            if (y == 1024)
            {
                y = 0;
                x++;
            }
        }

        return mat;
    }

    private static short[,] GetHeightmap()
    {
        var mat = new short[4096, 4096];

        var x = 0;
        var y = 0;
        for (var i = 0; i < HeightMapBytes.Length; i++)
        {
            mat[x, y] = HeightMapBytes[i];
            y++;

            if (y == 4096)
            {
                y = 0;
                x++;
            }
        }

        return mat;
    }
}