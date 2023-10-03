using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Helpers;
using WYD.Frame.Game.Pool;
using WYD.Frame.Models.Models;
using WYD.Frame.Services;
using WYD.Frame.Services.Common;
using WYD.Frame.Services.Models.Location;

namespace WYD.Frame.Game.Workers;

public class EventWorker : WorkerBase
{
    public Position NpcPosition { get; set; }
    public EventType EventType { get; set; }

    private object _summonLocker = new object();
    private readonly Area _armia;

    private readonly List<Position> _spots = new List<Position>()
    {
        new Position(1088, 1501),
        new Position(1084, 1455),
        new Position(1058, 1457),
        new Position(1059, 1500)
    };

    private DateTime _lastSpotSet = DateTime.Now.AddDays(-1);
    private Position? _selectedSpot = null;

    public EventWorker(WClient wClientOwner) : base(wClientOwner)
    {
        _armia = LocationService.Areas.First(x => x.Name == AreaNames.Armia);
    }

    protected override void DoWork()
    {
        if (EventType == EventType.Boss)
        {
            UseEventBoss();
        }
        else if (EventType == EventType.Teleporte)
        {
            UseEventTeleport();
        }
        else if (EventType == EventType.Toten)
        {
            UseEventToten();
        }
    }

    public override void Notify(object? args)
    {
        lock (_summonLocker)
        {
            if (args is WClient)
            {
                var argsAsClient = args as WClient;
                if (argsAsClient is null)
                {
                    return;
                }

                WClientOwner.Social.SendChat(argsAsClient.Player.Name, "summon");
                WClientOwner.Timer.Sleep(1000);
            }
            else if (args is PartyWorker.PartyReceivedArgs partyArgs && EventType != EventType.Toten)
            {
                var clientSender = GamePool.TryGetClientByClientId((ushort)partyArgs.LeaderId);

                if (clientSender is null) return;

                WClientOwner.Player.SendAcceptParty(partyArgs.LeaderId, partyArgs.LeaderName);
            }
        }
    }

    private void UseEventToten()
    {
        if (WClientOwner.Player.Party.Count < 12)
        {
            var nearbyClients = GamePool.GetAllClients().Where(z =>
                z.Status == ConnectionStatus.World &&
                z.Works.GetAll().Any(x => x is EventWorker evt && evt.EventType != EventType.Toten));

            foreach (var client in nearbyClients)
            {
                if (client.Player.ClientId != WClientOwner.Player.ClientId)
                    WClientOwner.Player.SendPartyRequest(client.Player.ClientId);
            }
        }

        Thread.Sleep(5000);
    }

    private void UseEventTeleport()
    {
        if (WClientOwner.Player.Moving)
        {
            Thread.Sleep(200);
            return;
        }

        var mascaraEvento = WClientOwner.Bag.GetItemById(StorageType.Inventario, 5210);
        if (mascaraEvento is null)
        {
            if (!BuyEntry())
            {
                Thread.Sleep(5000);
                WClientOwner.Log(MessageRelevance.High, "Falha ao comprar itens.");
                return;
            }
        }

        var npcEvento = WClientOwner.MobGrid.GetMobByName("Carnaval I", ignoreService: true);
        if (npcEvento is null || npcEvento.Position.GetDistance(WClientOwner.Player.Position) >= 10)
            if (!MoveAreaTp())
                return;
        GuardaMedalhas();
        ClickNpc();
        Thread.Sleep(1000);
    }

    private bool MoveAreaTp()
    {
        if (!WorldService.NpcEventoRect.IsOnArea(WClientOwner.Player.Position))
        {
            if (WClientOwner.Player.Party.Count > 1 && !_armia.Bounds.IsOnArea(WClientOwner.Player.Position))
            {
                if (!CheckOrBuyTp()) return false;
                var party = WClientOwner.Player.Party;

                var leaderId = party.FirstOrDefault(x => x.PartyIndex == 0);
                var runningClients = GamePool.GetAllClients()
                    .FirstOrDefault(z => z.Status == ConnectionStatus.World && z.Player.ClientId == leaderId.ClientId);

                if (runningClients is null)
                {
                    Console.WriteLine("Falha ao localizar cliente de toten.");
                    return false;
                }

                var tpClient =
                    runningClients.Works.GetAll()
                        .FirstOrDefault(z => z.GetType() == typeof(EventWorker)) as EventWorker;

                if (tpClient is null || tpClient.EventType != EventType.Toten)
                {
                    Console.WriteLine("Cliente toten sem macro toten ligado.");
                    return false;
                }

                tpClient.Notify(WClientOwner);
                WClientOwner.Social.SendChat(runningClients.Player.Name, "relo");
                WClientOwner.Timer.Sleep(500);
            }
            else
            {
                var moveWorker = new MoveWorker(WClientOwner)
                {
                    Id = Guid.NewGuid().ToString(),
                    Destination = WorldService.RandomByArea(WorldService.NpcEventoRect),
                    SyncRun = true
                };

                moveWorker.Start();
            }
        }

        return true;
    }

    private bool CheckOrBuyTp()
    {
        var pergaTp = WClientOwner.Bag.GetItemById(StorageType.Inventario, 776);

        if (pergaTp is not null)
        {
            return true;
        }

        var moveWorker = new MoveWorker(WClientOwner)
        {
            Id = Guid.NewGuid().ToString(),
            Destination = WorldService.RandomByArea(WorldService.AkiAreaRect),
            SyncRun = true
        };

        moveWorker.Start();
        var npc = NpcService.GetByName("Aki");

        if (npc is null || npc.Shop.Count == 0)
        {
            return false;
        }

        var portalItem = npc.Shop.First(x => x.Item.Id == 776);

        for (int i = 0; i < 5; i++)
        {
            var freeSlot = WClientOwner.Bag.Inventory.FirstFreeeSlot();
            if (freeSlot == -1) return false;

            WClientOwner.World.SendBuyItem((short)portalItem.CarrierSlot, (short)freeSlot, npc.Mob.ClientId);
            WClientOwner.Timer.Sleep(1000);
        }

        return true;
    }

    private void GuardaMedalhas()
    {
        var npcCargo = WClientOwner.MobGrid.GetMobByName("Guarda Carga");

        if (npcCargo is null)
        {
            WClientOwner.Log(MessageRelevance.Highest, "Guarda carga não encontrado.");
            return;
        }

        var currentPacks = WClientOwner.Bag.Inventory.ItemAmount(4054) / 100;

        if (currentPacks < 5) return;

        for (int i = 0; i < 30; i++)
        {
            var currentMedal = WClientOwner.Bag.GetItemById(StorageType.Inventario, 4054);
            var freeCargoSlot = WClientOwner.Bag.Cargo.FirstFreeeSlot();

            if (currentMedal is null) break;

            if (currentMedal.Item.GetAmount() < 100) continue;

            WClientOwner.Bag.SendMoveItem(StorageType.Inventario, StorageType.Cargo, (byte)freeCargoSlot,
                currentMedal.Slot, npcCargo.ClientId);
            WClientOwner.Timer.Sleep(1000);
        }
    }

    private bool ClickNpc()
    {
        // if (ClientOwner.Player.Party.Count > 1)
        // {

        // }


        var npc = WClientOwner.MobGrid.GetMobByName("Carnaval I", ignoreService: true);
        if (npc is not null)
        {
            Console.WriteLine("[Evento] Clicando NPC I");
            WClientOwner.World.SendNpcClick(npc.ClientId);
            WClientOwner.Timer.Sleep(1200);
        }

        npc = WClientOwner.MobGrid.GetMobByName("Carnaval II", ignoreService: true);
        if (npc is not null)
        {
            Console.WriteLine("[Evento] Clicando NPC II");
            WClientOwner.World.SendNpcClick(npc.ClientId);
            WClientOwner.Timer.Sleep(1200);
        }

        npc = WClientOwner.MobGrid.GetMobByName("Carnaval III", ignoreService: true);
        if (npc is not null)
        {
            Console.WriteLine("[Evento] Clicando NPC III");
            WClientOwner.World.SendNpcClick(npc.ClientId);
            WClientOwner.Timer.Sleep(1200);
        }

        npc = WClientOwner.MobGrid.GetMobByName("Carnaval IV", ignoreService: true);
        if (npc is not null)
        {
            Console.WriteLine("[Evento] Clicando NPC IV");
            WClientOwner.World.SendNpcClick(npc.ClientId);
            WClientOwner.Timer.Sleep(1200);
        }

        return true;
    }

    private bool BuyEntry()
    {
        var moveWorker = new MoveWorker(WClientOwner)
        {
            Id = Guid.NewGuid().ToString(),
            Destination = WorldService.RandomByArea(WorldService.AkiAreaRect),
            SyncRun = true
        };

        moveWorker.Start();

        var npc = NpcService.GetByName("Aki");
        if (npc is null)
        {
            Console.WriteLine("[Lan] Failed to find NPC.");
            return false;
        }

        WClientOwner.Timer.Sleep(1000);
        Console.WriteLine("Buying entrys");

        var npcItem = npc.Shop.FirstOrDefault(x => x.Item.Id == 5210);

        if (npcItem is null)
        {
            Console.WriteLine("Npc has no item");

            return false;
        }

        for (int i = 0; i < 100; i++)
        {
            var freeSlot = WClientOwner.Bag.Inventory.FirstFreeeSlot();
            if (freeSlot == -1) return false;

            WClientOwner.World.SendBuyItem((short)npcItem.CarrierSlot, (short)freeSlot, npc.Mob.ClientId);
            WClientOwner.Timer.Sleep(1000);
        }


        return true;
    }

    private void UseEventBoss()
    {
        if (WClientOwner.Player.Moving)
        {
            Thread.Sleep(200);
            return;
        }

        var eventNames = new List<AreaNames> { AreaNames.Evento, AreaNames.Evento_Inicio, AreaNames.Evento_Ponte };

        var eventArea = LocationService.Areas.Where(x => eventNames.Contains(x.Name));
        if (eventArea.Any(z => z.Bounds.IsOnArea(WClientOwner.Player.Position)))
        {
            if (_selectedSpot is null || (_lastSpotSet.AddSeconds(15) < DateTime.Now && !IsBossNear()))
            {
                WClientOwner.Log(MessageRelevance.High, "Re-setting spot.");
                MoveToSpot();
                _lastSpotSet = DateTime.Now;
            }
        }
        else
        {
            _selectedSpot = null;
            MoveToNpcTeleport();
        }
    }

    private void MoveToNpcTeleport()
    {
        var moveWorker = new MoveWorker(WClientOwner)
        {
            SyncRun = true,
            Destination = NpcPosition,
            Hpa = true,
            Id = Guid.NewGuid().ToString()
        };

        moveWorker.Start();

        WClientOwner.Timer.Sleep(2000);
        var npcEvento = WClientOwner.MobGrid.GetMobByName("DarkDragon", ignoreService: false);

        if (npcEvento is null)
        {
            WClientOwner.Log(MessageRelevance.Highest, "Falha ao localizar npc evento DarkDragon");
            Thread.Sleep(15000);
            return;
        }

        WClientOwner.World.SendNpcClick(npcEvento.ClientId);
        WClientOwner.Timer.Sleep(1000);
    }

    private bool IsBossNear()
    {
        var mobNear = WClientOwner.MobGrid.GetMobByName("CarbunkleChef", 20, ignoreService: true);
        return mobNear is not null && mobNear.Position.GetDistance(WClientOwner.Player.Position) < 10;
    }
    private void MoveToSpot()
    {
        Position selectedSpot;

        if (_selectedSpot is null)
        {
            var selectSpot = Utils.Rand.Next(0, _spots.Count);
            selectedSpot = _spots[selectSpot];
        }
        else
        {
            selectedSpot = _selectedSpot;
        }

        WClientOwner.Log(MessageRelevance.High,
            $"Spot evento escolhido {selectedSpot.X} / {selectedSpot.Y}");


        var moveWorker = new MoveWorker(WClientOwner)
        {
            SyncRun = true,
            Destination = selectedSpot,
            Hpa = true,
            Id = Guid.NewGuid().ToString()
        };

        moveWorker.Start();

        var mobNear = WClientOwner.MobGrid.GetMobByName("CarbunkleChef", 20, ignoreService: true);

        if (!IsBossNear())
        {
            var moveToBossWorker = new MoveWorker(WClientOwner)
            {
                SyncRun = true,
                Destination = mobNear.Position,
                Hpa = true,
                Id = Guid.NewGuid().ToString()
            };

            moveToBossWorker.Start();
        }

        _selectedSpot = selectedSpot;
    }
}