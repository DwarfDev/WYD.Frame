using System.Drawing;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Models.Models;

namespace WYD.Frame.Services.Models.Lan;

public class Quest
{
    public Position UpperLimit { get; set; } = new();
    public Position LowerLimit { get; set; } = new();
    public AreaSize BlockSize { get; set; }

    public AreaSize QuestTotalArea =>
        new()
        {
            Height = UpperLimit.Y - LowerLimit.Y,
            Width = UpperLimit.X - LowerLimit.X
        };

    public QuestType Type { get; set; }

    private readonly object _lanLock = new();

    private readonly List<QuestSpot> _spots = new();
    

    public bool IsInside(Position pos)
    {
        return pos.X < UpperLimit.X && pos.X > LowerLimit.X && pos.Y < UpperLimit.Y && pos.Y > LowerLimit.Y;
    }

    public List<QuestSpot> GetAllSpots()
    {
        var xBlocks = QuestTotalArea.Width / BlockSize.Width;
        var yBlocks = QuestTotalArea.Height / BlockSize.Height;

        PopulateSpots(xBlocks, yBlocks);

        lock (_lanLock)
            return _spots.ToList();
    }

    public QuestSpot GetRandomBlockPosition(string instanceId, List<int> desiredSpots)
    {
        var xBlocks = QuestTotalArea.Width / BlockSize.Width;
        var yBlocks = QuestTotalArea.Height / BlockSize.Height;

        PopulateSpots(xBlocks, yBlocks);
        lock (_lanLock)
        {
            _spots.ForEach(x => x.OccupiedBy.RemoveAll(z => z.Contains(instanceId)));

            var spots = desiredSpots.Count > 0
                ? _spots.Where(x => desiredSpots.Any(z => z == x.SpotId)).ToList()
                : _spots;

            var spotsOrderedByOccupation = spots.GroupBy(x => x.OccupiedBy.Count).OrderBy(x => x.Key);

            var elegibleSpots = spotsOrderedByOccupation.First().ToList();

            var randSpot = Utils.Rand.Next(0, elegibleSpots.Count - 1);

            var selectedSpot = elegibleSpots[randSpot];

            selectedSpot.OccupiedBy.Add(instanceId);
            return selectedSpot;
        }
    }
    private void PopulateSpots(int xBlocks, int yBlocks)
    {
        lock (_lanLock)
        {
            if (_spots.Count == 0)
            {
                for (int i = 0; i < xBlocks; i++)
                {
                    for (int j = 0; j < yBlocks; j++)
                    {
                        _spots.Add(new QuestSpot()
                        {
                            SpotX = i,
                            SpotY = j,
                            SpotId = yBlocks * i + j,
                            Position = new Position()
                            {
                                X = (short)(LowerLimit.X + ((i + 1) * BlockSize.Width - BlockSize.Width / 2)),
                                Y = (short)(LowerLimit.Y + ((j + 1) * BlockSize.Height - BlockSize.Height / 2))
                            }
                        });
                    }
                }
            }
        }
    }

    public List<QuestSpot> GetOccupation()
    {
        lock (_lanLock)
            return _spots.Where(x => x.OccupiedBy.Any()).ToList();
    }
}