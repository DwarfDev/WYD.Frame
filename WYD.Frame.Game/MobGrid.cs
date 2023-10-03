using WYD.Frame.Models.Models;
using WYD.Frame.Packets;
using WYD.Frame.Services;

namespace WYD.Frame.Game;

public class MobGrid
{
    private readonly WClient _wClient;
    private readonly IDictionary<ushort, Mob> _entities = new Dictionary<ushort, Mob>();
    private readonly object _entityLock = new();

    public EventHandler<Mob>? MobChanged;
    public EventHandler<ushort>? MobRemoved;

    public MobGrid(WClient wClient)
    {
        _wClient = wClient;
    }

    public List<Mob> Entities
    {
        get
        {
            lock (_entityLock)
            {
                return _entities.Values.ToList();
            }
        }
    }

    #region Helpers

    public Position GeneratePositionWithoutMob(Position pos)
    {
        lock (_entityLock)
        {
            if (!_entities.Any(x => x.Value.IsInPosition(pos)))
            {
                if (MapService.Grid[pos.X, pos.Y] == 1)
                    return pos;
            }
        }

        Console.WriteLine($"{DateTime.Now} Player in pos {pos.X}/{pos.Y}");
        var random = RandomPositionGivenPoint(pos);
        Console.WriteLine($"{DateTime.Now} New pos {random.X}/{random.Y}");
        return random;
    }

    private Position RandomPositionGivenPoint(Position pos)
    {
        var position = new Position
        {
            X = pos.X, Y = pos.Y
        };

        for (var i = 0; i < 4; i++)
        {
            var positions = PointsByRadiusAndAngle(pos, i).OrderBy(x => x.GetDistance(pos)).ToList();
            for (var j = 0; j < positions.Count; j++)
                if (!Entities.Any(x => x.IsInPosition(positions[j]))
                   )
                {
                    if (MapService.Grid[positions[j].X, positions[j].Y] == 1) return positions[j];
                }
        }

        return position;
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

    public Mob? GetMobByName(string name, int maxRange = 0, bool ignoreService = false)
    {
        lock (_entityLock)
        {
            Mob npc = _entities.Values
                .FirstOrDefault(x =>
                    x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && (maxRange == 0 ||
                        x.Position.GetDistance(_wClient.Player.Position) < maxRange));

            if (ignoreService) return npc;

            if (npc != null) return npc;

            
            var npcs = NpcService.GetAllByName(name, maxRange);
            
            var npcT = npcs.FirstOrDefault(x =>
                maxRange == 0 || x.Mob.Position.GetDistance(_wClient.Player.Position) < maxRange);

            if (npcT is null) return default;

            return npcT.Mob;
        }
    }

    public ushort GetMobIndexByName(string name)
    {
        lock (_entityLock)
        {
            var entity = _entities.Values
                .FirstOrDefault(x => x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));

            return entity.ClientId;
        }
    }

    public Mob? GetMobById(ushort mobId)
    {
        lock (_entityLock)
        {
            if (_entities.ContainsKey(mobId))
                return _entities[mobId];
            return default;
        }
    }

    public bool ContainsMob(ushort mobId)
    {
        lock (_entityLock)
        {
            return _entities.ContainsKey(mobId);
        }
    }


    public void Remove(ushort mobId)
    {
        lock (_entityLock)
        {
            if (_entities.ContainsKey(mobId))
                _entities.Remove(mobId);
            MobRemoved?.Invoke(_wClient, mobId);
        }
    }

    public void Add(Mob mob)
    {
        lock (_entityLock)
        {
            if (_entities.ContainsKey(mob.ClientId))
            {
                _entities[mob.ClientId] = mob;
            }
            else
                _entities.Add(mob.ClientId, mob);

            MobChanged?.Invoke(_wClient, mob);
        }
    }

    #endregion
}