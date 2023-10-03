using WYD.Frame.Models.Models;
using WYD.Frame.Services.Models.Npc;

namespace WYD.Frame.Services;

public static class EventService
{
    private static object _occupationsLocker = new object();
    private static List<SearchOccupation> _occupations = new();
    public static readonly string CurrentLookingFor = "EVENTO HIT";

    private static List<Position> _extremes = new List<Position>()
    {
        new Position(2081, 2084),
        new Position(2083, 2119),
        new Position(2137, 2120),
        new Position(2139, 2083)
    };

    private static object _lastRegisterLocker = new object();
    public static Mob? LastRegister { get; private set; } = null;
    public static DateTime LastSeen { get; private set; } = DateTime.Now.AddDays(-2);


    class SearchOccupation
    {
        public int Spot { get; set; }
        public List<string> OccupiedBy { get; set; }
    }

    public class Direction
    {
        public int Spot { get; set; }
        public Position Position { get; set; }
    }

    public static void FreeDirection(string clientId)
    {
        _occupations.ForEach(x => { x.OccupiedBy.RemoveAll(z => z.Equals(clientId)); });
    }

    public static Direction GetDirectionAndOccupy(string clientId, List<int> pleaseIgnore)
    {
        lock (_occupationsLocker)
        {
            if (_occupations.Count == 0)
            {
                Populate();
            }

            foreach (var occupation in _occupations)
            {
                if (occupation.OccupiedBy.Contains(clientId))
                {
                    occupation.OccupiedBy.RemoveAll(x => x.Equals(clientId));
                }
            }

            var orderedByOccupation = _occupations.OrderBy(x => x.OccupiedBy.Count).AsQueryable();

            if (_occupations.Count(x => !pleaseIgnore.Contains(x.Spot)) > 0)
            {
                orderedByOccupation = orderedByOccupation.Where(x => !pleaseIgnore.Contains(x.Spot));
            }

            var lessOccupied = orderedByOccupation.First();

            lessOccupied.OccupiedBy.Add(clientId);
            return new Direction
            {
                Position = _extremes[lessOccupied.Spot],
                Spot = lessOccupied.Spot
            };
        }
    }

    public static void Saw(Mob npc)
    {
        lock (_lastRegisterLocker)
        {
            LastRegister = npc;
            LastSeen = DateTime.Now;
        }
    }

    private static void Populate()
    {
        for (int i = 0; i < 4; i++)
        {
            _occupations.Add(new SearchOccupation()
            {
                Spot = i,
                OccupiedBy = new List<string>()
            });
        }
    }
}