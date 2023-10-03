using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Helpers;
using WYD.Frame.Models.Models;
using WYD.Frame.Services;
using WYD.Frame.Services.Common;
using WYD.Frame.Services.Models.Lan;
using WYD.Frame.Services.Models.Npc;
using WYD.Frame.Services.Models.World;

namespace WYD.Frame.Game.Workers;

public class LanWorker : WorkerBase
{
    private QuestSpot? _selectedSpot;
    private DateTime _lastSpotSet = DateTime.Now.AddSeconds(-10);
    public List<int> Spots { get; set; }

    public LanWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }

    protected override void DoWork()
    {
        if (WClientOwner.Player.Moving)
        {
            Thread.Sleep(200);
            return;
        }

        if (WClientOwner.Player.IsOnCity)
        {
            var sleep = Utils.Rand.Next(10, 30) * 1000;
            Thread.Sleep(sleep);
            WClientOwner.Log(MessageRelevance.Highest, $"Aguardando {sleep} para entrar lan");
            if (!UseEntry())
            {
                BuyEntry();
            }

            WClientOwner.Timer.Sleep(1000);
        }

        if (QuestService.GetQuestByType(QuestType.LanN).IsInside(WClientOwner.Player.Position))
        {
            var diff = DateTime.Now - _lastSpotSet;

            if (_selectedSpot is null)
            {
                MoveToSpot();
            }

            if (diff.TotalSeconds > 30 && _selectedSpot is not null)
            {
                var moveWorker = new MoveWorker(WClientOwner)
                {
                    Id = Guid.NewGuid().ToString(),
                    Destination = _selectedSpot.Position,
                    SyncRun = true
                };
                moveWorker.Start();
                _lastSpotSet = DateTime.Now;
            }
        }
    }


    private void BuyEntry()
    {
        var areaNamesWithLanEntry = new AreaNames[] { AreaNames.Erion };
        var areasWithLanEntry = LocationService.Areas.Where(x => areaNamesWithLanEntry.Contains(x.Name)).ToArray();

        var closestCommonGoods = LocationService.ClosestCommonGoods(WClientOwner.Player.Position, areasWithLanEntry);
        
        var npc = NpcService.GetByName(closestCommonGoods.CommonGoods.Name);
        if (npc is null || npc.Mob.Position.GetDistance(WClientOwner.Player.Position) > 16)
        {
            Position positionToWalk;
            
            if(npc is null)
                positionToWalk = WorldService.RandomByArea(closestCommonGoods.CommonGoods.AreaRect);
            else 
                positionToWalk = npc.Mob.Position.RandomByRange(4);

            bool StopCondition()
            {
                var npcFound = WClientOwner.MobGrid.GetMobByName(closestCommonGoods.CommonGoods.Name, 17, true);

                return npcFound is not null;
            }
            
            Console.WriteLine($"[Lan] Failed to find NPC {closestCommonGoods.CommonGoods.Name} walking to {positionToWalk.X} / {positionToWalk.Y}");
            var moveWorker = new MoveWorker(WClientOwner)
            {
                Id = Guid.NewGuid().ToString(),
                Destination = positionToWalk,
                SyncRun = true,
                StopCondition = StopCondition
            };
        
            moveWorker.Start();
            return;
        }

        WClientOwner.Timer.Sleep(1000);
        Console.WriteLine("Buying entrys");

        if (npc.Shop.Count == 0)
        {
            WClientOwner.Log(MessageRelevance.Medium, "Npc shop not discovered yet.");
            WClientOwner.World.DiscoverNpc((short)npc.Mob.ClientId);
            return;
        }

        var npcItem = npc.Shop.FirstOrDefault(x => x.Item.Id == 4111);

        if (npcItem is null)
        {
            WClientOwner.Log(MessageRelevance.Medium, "Stopping macro, npc has no entry.");
            Stop();
            return;
        }

        WClientOwner.World.SendUseNpcItem(npcItem.CarrierSlot, npc.Mob.ClientId);


        //Buy item old method
        //BuyFiveEntries(npc.Mob.ClientId, npcItem);
    }

    private void BuyFiveEntries(ushort npcId, ShopItem npcItem)
    {
        var freeSlotscount = WClientOwner.Bag.Inventory.FreeSlotCount(WClientOwner.Bag.ExtraBagCount);


        if (freeSlotscount <= 0)
        {
            Console.WriteLine("Not enough space");
            Stop();
            return;
        }

        if (freeSlotscount > 5)
            freeSlotscount = 5;

        for (var i = 0; i < freeSlotscount; i++)
        {
            var freeSlot = WClientOwner.Bag.Inventory.FirstFreeeSlot();

            if (freeSlot == -1) return;
            Console.WriteLine($"Requested buy for lan entry at slot {freeSlot} for npc slot {npcItem.CarrierSlot}");

            WClientOwner.World.SendBuyItem((byte)npcItem.CarrierSlot, (byte)freeSlot, npcId);
            WClientOwner.Timer.Sleep(1000);
        }
    }

    private bool UseEntry()
    {
        var itemToUse = WClientOwner.Bag.GetItemById(StorageType.Inventario, 4111);
        if (itemToUse is null) return false;

        WClientOwner.Bag.SendUseItem(itemToUse.StorageType, default, itemToUse.Slot, default, default, true);

        return true;
    }


    private void MoveToSpot()
    {
        var randomPos = QuestService.GetQuestByType(QuestType.LanN)
            .GetRandomBlockPosition(WClientOwner.Id.ToString(), Spots);
        GuildedService.Log(GuildedLogType.Behavior, $"{randomPos.Position.X} / {randomPos.Position.Y}",
            WClientOwner.Player.Name,
            $"...");

        _selectedSpot = randomPos;

        var moveWorker = new MoveWorker(WClientOwner)
        {
            Id = Guid.NewGuid().ToString(),
            Destination = randomPos.Position,
            SyncRun = true
        };

        moveWorker.Start();
    }
}