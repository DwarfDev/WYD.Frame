using Mapster;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Helpers;
using WYD.Frame.Models.Models;
using WYD.Frame.Packets;
using WYD.Frame.Services;
using WYD.Frame.Services.Models.Location;

namespace WYD.Frame.Game.Workers;

public class MoveWorker : WorkerBase
{
    public MoveWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }

    public Position Destination { get; set; }
    public bool Hpa { get; set; } = true;
    public Func<bool>? StopCondition { get; set; }

    protected override void DoWork()
    {
        try
        {
            MoveHpa();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to move: reason {ex.Message} ");
            Stop();
            WClientOwner.Works.ResumeAll();
        }

        WClientOwner.Player.Moving = false;
    }

    private void MoveHpa()
    {
        var currentWorkers = WClientOwner.Works.GetAll().Where(x => x.Id != Id);

        if (currentWorkers.Any(x => x.GetType() == typeof(MoveWorker) && x.State != WorkState.Ended))
        {
            Console.WriteLine("can't walk. already has some move worker...");
            Stop();
            return;
        }

        if (WClientOwner.Status != ConnectionStatus.World) return;


        WClientOwner.Works.PauseAll(Id);
        WClientOwner.Player.Moving = true;

        if (WalkWithScrolls(Destination))
        {
            var currentArea = LocationService.GetAreaHpa(WClientOwner.Player.Position);

            if (currentArea is null)
            {
                WClientOwner.Log(MessageRelevance.Highest,
                    $"Player em area desconhecida: {WClientOwner.Player.Position.X} / {WClientOwner.Player.Position.Y}");
                goto end;
            }

            if (!MoveInArea(Destination, currentArea)) goto end;
            WClientOwner.Timer.Sleep(1000);
            
            if(WClientOwner.Player.Position.GetDistance(Destination) < 20)
                goto end;
        }

        var destinationArea = LocationService.GetAreaHpa(Destination);
        var playerArea = LocationService.GetAreaHpa(WClientOwner.Player.Position);


        if (destinationArea is null)
        {
            Console.WriteLine("Dest Area is unknown");
            return;
        }

        if (playerArea is null)
        {
            Console.WriteLine("Player Current Area is unknown");
            return;
        }

        Console.WriteLine(
            $"{DateTime.Now} Started walking to {Destination.X} / {Destination.Y} from area {playerArea.Name} to {destinationArea.Name}");

        // var paths = LocationService.DrawAreaPaths(playerArea, destinationArea);

        var pathFuncion = LocationService.ShortestPathFunction(playerArea);
        var paths = pathFuncion(destinationArea).ToList();

        if (!paths.Contains(playerArea))
        {
            paths.Insert(0, playerArea);
        }

        MoveFromPaths(paths);

        end:
        WClientOwner.Timer.Sleep(1000);
        WClientOwner.Works.ResumeAll();
        WClientOwner.Player.Moving = false;

        Stop();
    }

    private void MoveFromPaths(List<Area> paths)
    {
        int moveTries = 0;
        for (int i = 0; i < paths.Count; i++)
        {
            if (i + 1 < paths.Count)
            {
                var exitToNextArea = paths[i].Exits.First(x => x.Destination == paths[i + 1]);

                var finderForNode = MapService.CreateForNode(paths[i]);

                var origin = paths[i].ReduceToArea(WClientOwner.Player.Position);

                var destination = paths[i].ReduceToArea(exitToNextArea.Exit);

                var positionsToWalk =
                    PathingService.FindPath(finderForNode, origin, destination);

                var normalizedPosition = PathingService.NormalizePath(positionsToWalk);
                foreach (var position in normalizedPosition)
                {
                    position.X += paths[i].Bounds.Min.X;
                    position.Y += paths[i].Bounds.Min.Y;
                    RandomizePoint(position, paths[i]);
                    if (!WalkTo(position)) return;
                }

                if (!WalkTo(exitToNextArea.Exit)) return;

                if (exitToNextArea.ExitType == ExitType.Portal)
                {
                    var beforePos = WClientOwner.Player.Position.Clone();
                    Console.WriteLine("Using portal");
                    WClientOwner.World.SendPortal();
                    WClientOwner.Timer.Sleep(2000);

                    if (WClientOwner.Player.Position.GetDistance(beforePos) < 20)
                    {
                        if (moveTries > 3)
                        {
                            Console.WriteLine("Failed to move.");
                            return;
                        }
                        Console.WriteLine("Didnt work");
                        WClientOwner.Player.Position.X -= 5;
                        WClientOwner.Player.Position.Y -= 5;
                        moveTries++;
                        i--;
                    }
                    continue;
                }

                var nextAreaPos = exitToNextArea.Exit.Clone();

                while (true)
                {
                    nextAreaPos.Y++;
                    if (paths[i + 1].Bounds.IsOnArea(nextAreaPos))
                        break;
                    nextAreaPos.Y--;
                    nextAreaPos.X++;
                    if (paths[i + 1].Bounds.IsOnArea(nextAreaPos))
                        break;
                    nextAreaPos.X--;

                    nextAreaPos.Y--;
                    if (paths[i + 1].Bounds.IsOnArea(nextAreaPos))
                        break;
                    nextAreaPos.Y++;
                    nextAreaPos.X--;
                    if (paths[i + 1].Bounds.IsOnArea(nextAreaPos))
                        break;
                    nextAreaPos.X++;

                    break;
                }

                WClientOwner.Player.Position = nextAreaPos;
            }
            else
            {
                if (!MoveInArea(Destination, paths[i])) return;
            }
        }
    }

    private void RandomizePoint(Position originalPosition, Area currentPlayerArea)
    {
        var randX = Utils.Rand.Next(1, 4);
        var randY = Utils.Rand.Next(1, 4);

        var position = originalPosition.Clone();

        var coinFlip = Utils.Rand.Next(0, 4);

        if (coinFlip == 0)
        {
            position.X += (short)randX;
            position.Y += (short)randY;
        }
        else if (coinFlip == 1)
        {
            position.X -= (short)randX;
            position.Y -= (short)randY;
        }
        else if (coinFlip == 2)
        {
            position.X += (short)randX;
            position.Y -= (short)randY;
        }
        else if (coinFlip == 3)
        {
            position.X -= (short)randX;
            position.Y += (short)randY;
        }

        if (MapService.Grid[position.X, position.Y] == 0)
        {
            Console.WriteLine($"Can't walk to {position.X} / {position.Y}");
            return;
        }
        
        if (!currentPlayerArea.Bounds.IsOnArea(position))
        {
            Console.WriteLine($"Can't walk to {position.X} / {position.Y} isnt area");
            return;
        }

        originalPosition.X = position.X;
        originalPosition.Y = position.Y;
    }

    private bool MoveInArea(Position destination, Area playerArea)
    {
        var finderForNode = MapService.CreateForNode(playerArea);

        var origin = playerArea.ReduceToArea(WClientOwner.Player.Position);

        var dst = playerArea.ReduceToArea(Destination);

        var positionsToWalk =
            PathingService.FindPath(finderForNode, origin, dst);

        if (positionsToWalk.Length == 0)
        {
            WalkTo((dst));
            return false;
        }

        var normalizedPosition = PathingService.NormalizePath(positionsToWalk);
        foreach (var position in normalizedPosition)
        {
            position.X += playerArea.Bounds.Min.X;
            position.Y += playerArea.Bounds.Min.Y;
            if (!WalkTo(position)) return false;
        }

        return WalkTo(destination);
    }


    private bool IsPortalPathingMoreExpensive(Position destination, Portal[] positions)
    {
        var totalTravel = 0;
        for (var i = 0; i < positions.Length; i++)
        {
            var lead = LocationService.AreaList.First(x => x.Name == positions[i].LeadsTo);
            if (i == positions.Length - 1)
                totalTravel += lead.CenterPosition.GetDistance(destination);
            else
                totalTravel += lead.CenterPosition.GetDistance(positions[i + 1].Location);
        }

        if (totalTravel > WClientOwner.Player.Position.GetDistance(destination))
            return true;

        return false;
    }

    private List<Position> PortalPaths(Position destination)
    {
        var closestToDest = LocationService.ClosestArea(destination);
        var closestToPlayer = LocationService.ClosestCity(WClientOwner.Player.Position);

        var positions = LocationService.DrawPathPortals(closestToPlayer, closestToDest);


        return IsPortalPathingMoreExpensive(destination, positions)
            ? new List<Position>()
            : positions.Select(x => x.Location).ToList();
    }

    private bool WalkWithScrolls(Position destination)
    {
        var playerDistanceFromDestination = destination.GetDistance(WClientOwner.Player.Position);
        var bestScrollForDestination = LocationService.FindClosestScroll(Destination);
        var scrollDistanceToDestination = destination.GetDistance(bestScrollForDestination.Destination);
        var itemSlot = WClientOwner.Bag.Inventory.FirstOrDefault(x =>
            x.StorageType is StorageType.Inventario &&
            x.Item.Id == bestScrollForDestination.ItemId);

        if (itemSlot is null || scrollDistanceToDestination > playerDistanceFromDestination) return false;

        WClientOwner.World.SendDelayedActionSignal(1);
        Thread.Sleep(5500);

        WClientOwner.Bag.SendUseItem(itemSlot.StorageType, default, itemSlot.Slot, default,
            bestScrollForDestination.Index + 1);

        Thread.Sleep(2000);
        return true;
    }

    private bool WalkTo(Position currentWalkDestination)
    {
        var moveSpeed = WClientOwner.Bag.Equips.First(x => x.Slot == 14).Item.Id == 0 ? 3f : 6f;

        var to = WClientOwner.MobGrid.GeneratePositionWithoutMob(currentWalkDestination);
        var from = WClientOwner.Player.Position;
        if (from.X == to.X && from.Y == to.Y) return true;
        var sleep = (int)(WClientOwner.Player.Position.GetDistance(to) / moveSpeed * 1000f);

        if (to.GetDistance(from) >= 20)
        {
            WClientOwner.Log(MessageRelevance.Highest,
                $"Failed to move from {from.X} / {from.Y} to {to.X} / {to.Y} because distance is too high.");
            return false;
        }

        var directions = PathingService.GetRoute(from.X, from.Y, to.X, to.Y, from.GetDistance(to));
        

        if (directions is not null)
        {
            Console.WriteLine($"Moving from {WClientOwner.Player.Position.X} / {WClientOwner.Player.Position.Y} to {to.X} / {to.Y}");
            var dirConvrt = System.Text.Encoding.UTF8.GetBytes(string.Join("", directions));
            WClientOwner.Player.SendMove(to.X, to.Y, (byte)moveSpeed, dirConvrt);
        }
        else
        {
            Console.WriteLine($"Moving from {WClientOwner.Player.Position.X} / {WClientOwner.Player.Position.Y} to {to.X} / {to.Y}");
            WClientOwner.Player.SendMove(to.X, to.Y, (byte)moveSpeed);
        }
        WClientOwner.Player.Position = to.Clone().Adapt<Position>();

        WClientOwner.Moved?.Invoke(WClientOwner, to);
        
        var result = StopCondition?.Invoke();

        if (result.HasValue && result.Value)
        {
            WClientOwner.Log(MessageRelevance.Highest,
                $"Stop condition has been met.");
            return false;
        }
        
        Thread.Sleep(sleep);
        return true;
    }

    private void DiscoverNpcs()
    {
        var closestNpcs = NpcService.GetUndiscoverd()
            .Where(x => x.Mob.Position.GetDistance(WClientOwner.Player.Position) < 10).ToList();

        if (closestNpcs.Count > 0)
            foreach (var npc in closestNpcs)
            {
                if (WClientOwner.Player.Position.GetDistance(npc.Mob.Position) > 10)
                {
                    Console.WriteLine($"[Discovery] Too far away {npc.Mob.Name}");
                    continue;
                }

                WClientOwner.World.LastShopRequestIndex = npc.Mob.ClientId;
                Console.WriteLine($"[Discovery] Getting for {npc.Mob.Name} / {npc.Mob.ClientId}");

                WClientOwner.World.SendRequestShopList(npc.Mob.ClientId);

                for (var i = 0; i < 5; i++)
                {
                    if (WClientOwner.World.LastShopRequestIndex == 0) break;
                    WClientOwner.Timer.Sleep(200);
                }

                npc.Discovered = true;

                WClientOwner.World.LastShopRequestIndex = 0;
            }
    }
}