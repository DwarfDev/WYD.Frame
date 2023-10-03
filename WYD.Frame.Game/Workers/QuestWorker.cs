using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Models.Models;
using WYD.Frame.Services;
using WYD.Frame.Services.Models.Lan;

namespace WYD.Frame.Game.Workers;

public class QuestWorker : WorkerBase
{
    private QuestSpot? _selectedSpot;
    private DateTime _lastSpotSet = DateTime.Now.AddSeconds(-10);
    public Dictionary<QuestType, List<int>> Spots { get; set; }
    

    public QuestWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }

    protected override void DoWork()
    {
        if (WClientOwner.Player.Moving)
        {
            Thread.Sleep(1000);
            return;
        }

        var questType = GetQuestByLevel(WClientOwner.Player.Status.Level);

        if (questType is null)
        {
            WClientOwner.Log(MessageRelevance.Highest, "Sem quests para ir. Parando macro...");
            Stop();
            return;
        }
        
        if (WClientOwner.Player.IsOnCity)
        {
            var dangerPosition = GetDangerPosition(WClientOwner.Player.Status.Level);
            if (dangerPosition is not null)
            {
                Console.WriteLine("Danger level, moving to npc entrance.");
                var moveWorker = new MoveWorker(WClientOwner)
                {
                    Destination = dangerPosition,
                    Id = Guid.NewGuid().ToString()
                };
                moveWorker.Start();
                return;
            }
            else if (!UseEntry(questType.Value))
            {
                Thread.Sleep(15000);
            }

            WClientOwner.Timer.Sleep(1000);
            return;
        }

        if (!QuestService.GetQuestByType(questType.Value).IsInside(WClientOwner.Player.Position))
        {
            //must be in the entrance
            var npcAttribute =  questType.GetAttribute<NpcName>();

            if (npcAttribute is null)
            {
                WClientOwner.Log(MessageRelevance.Highest, "Falha ao identificar nome do NPC. Contate o senpepo.");
                Stop();
                return;
            }
            
            var npcName = npcAttribute.Name;
            var npcFromMob = WClientOwner.MobGrid.GetMobByName(npcName, ignoreService: false);

            if (npcFromMob is not null)
            {
                if (npcFromMob.Position.GetDistance(WClientOwner.Player.Position) < 20)
                {
                    WClientOwner.World.SendNpcClick(npcFromMob.ClientId);
                    WClientOwner.Timer.Sleep(1000);
                }
                return;
            }
        }

        if (QuestService.GetQuestByType(questType.Value).IsInside(WClientOwner.Player.Position))
        {
            var diff = DateTime.Now - _lastSpotSet;

            if (_selectedSpot is null)
            {
                MoveToSpot(questType.Value);

                WClientOwner.Works.ResumeAll();
            }

            if (diff.TotalSeconds > 30 && _selectedSpot is not null)
            {
                var moveWorker = new MoveWorker(WClientOwner)
                {
                    Id = Guid.NewGuid().ToString(),
                    Destination = _selectedSpot.Position
                };
                moveWorker.Start();
                _lastSpotSet = DateTime.Now;
            }
        }
    }

    private Position? GetDangerPosition(int level)
    {
        // if (level >= 39 && level <= 114)
        // {
        //     return QuestType.Coveiro;
        // }
        //
        // if (level >= 115 && level <= 190)
        // {
        //     return QuestType.Jardim;
        // }
        //
        // if (level >= 190 && level <= 264)
        // {
        //     return QuestType.Kaizen;
        // }
        //
        // if (level >= 265 && level <= 319)
        // {
        //     return QuestType.Hidra;
        // }
        //
        // if (level >= 320 && level <= 350)
        // {
        //     return QuestType.Elfos;
        // }
        return null;
    }

    private void MoveToSpot(QuestType type)
    {

        var desiredSpots = Spots[type];
        var randomPos = QuestService.GetQuestByType(type).GetRandomBlockPosition(WClientOwner.Id.ToString(), desiredSpots);
        GuildedService.Log(GuildedLogType.Behavior, $"{randomPos.Position.X} / {randomPos.Position.Y}",
            WClientOwner.Player.Name,
            $"Quest");

        _selectedSpot = randomPos;

        var moveWorker = new MoveWorker(WClientOwner)
        {
            Id = Guid.NewGuid().ToString(),
            Destination = randomPos.Position
        };

        moveWorker.Start();
    }

    private QuestType? GetQuestByLevel(int level)
    {
        if (level >= 39 && level <= 114)
        {
            return QuestType.Coveiro;
        }

        if (level >= 115 && level <= 189)
        {
            return QuestType.Jardim;
        }

        if (level >= 190 && level <= 264)
        {
            return QuestType.Kaizen;
        }

        if (level >= 265 && level <= 319)
        {
            return QuestType.Hidra;
        }

        if (level >= 320 && level <= 350)
        {
            return QuestType.Elfos;
        }

        return null;
    }
    private bool UseEntry(QuestType type)
    {
        var itemId = (int)type + 4037;
        var itemToUse = WClientOwner.Bag.Inventory.FirstOrDefault(x => x.Item.Id == itemId);

        if (itemToUse is null)
        {
            WClientOwner.Log(MessageRelevance.Highest, $"Sem entrada para quest {type}.");
            return false;
        }
        WClientOwner.Works.PauseAll(Id);
        WClientOwner.Bag.SendUseItem(StorageType.Inventario, default, (byte)itemToUse.Slot, default, default, false);
        WClientOwner.Timer.Sleep(3000);
        return true;
    }
}