using Mapster;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.GameEvents;
using WYD.Frame.Game.Models;
using WYD.Frame.Game.Models.Game;
using WYD.Frame.Game.Workers;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;
using WYD.Frame.Services;

namespace WYD.Frame.Game;

public class Bag
{
    private readonly WClient _wClient;
    private readonly List<BagItem> _items = new();
    private readonly object _itemsLock = new();

    public EventHandler<BagItem>? ItemChanged;
    public EventHandler<List<BagItem>>? BagChanged;
    public EventHandler<TradeInfo>? TradeReceived;
    private int _currentTradingWith = 0;
    private bool _acceptedTrade = false;

    public Bag(WClient wClient)
    {
        _wClient = wClient;
        Andarilho = new NetworkItem[2];
    }

    public int ExtraBagCount => Andarilho.Count(x => x.Id != 0);
    public NetworkItem[] Andarilho { get; private set; }

    public List<BagItem> Inventory
    {
        get
        {
            lock (_itemsLock)
            {
                return _items.Where(x => x.StorageType == StorageType.Inventario).ToList();
            }
        }
    }

    public List<BagItem> Cargo
    {
        get
        {
            lock (_itemsLock)
            {
                return _items.Where(x => x.StorageType == StorageType.Cargo).ToList();
            }
        }
    }

    public List<BagItem> Equips
    {
        get
        {
            lock (_itemsLock)
            {
                return _items.Where(x => x.StorageType == StorageType.Equipamentos).ToList();
            }
        }
    }

    public List<BagItem> Items
    {
        get
        {
            lock (_itemsLock)
            {
                return _items.ToList();
            }
        }
    }


    #region Helpers

    public void DecrementItem(StorageType storageSource, int slot)
    {
        lock (_itemsLock)
        {
            var bagItem = _items.FirstOrDefault(x => x.StorageType == storageSource && x.Slot == slot);

            if (bagItem is null) return;

            var item = bagItem.Item;
            var efvIndex = GetEfvIndexByType(item, GameEfv.EF_AMOUNT);
            if (efvIndex != -1 && item.Ef[efvIndex].Value > 1)
                item.Ef[efvIndex].Value--;
            else
            {
                bagItem.Item = NetworkItem.Create();
            }
        }
    }
    public int GetEfvIndexByType(NetworkItem item, GameEfv efvType)
    {
        var index = -1;
        for (var efvIndex = 0; efvIndex < item.Ef.Length; efvIndex++)
            if (item.Ef[efvIndex].Type == efvType)
            {
                index = efvIndex;
                break;
            }

        return index;
    }
    public BagItem? GetItemById(StorageType storageType, int id, bool ignoreMaxAmount = false, int ignoreSlot = -1)
    {
        lock (_itemsLock)
        {
            return _items.FirstOrDefault(x =>
                x.StorageType == storageType && x.Item.Id == id && (ignoreSlot == -1 || x.Slot != ignoreSlot) &&
                (!ignoreMaxAmount ||
                 (x.Item.Ef.All(z=> z.Type != GameEfv.EF_AMOUNT) || x.Item.Ef.Any(z => z is
                     { Type: GameEfv.EF_AMOUNT, Value: < 100 }))));
        }
    }

    #endregion


    #region send

    public void SendUseItem(StorageType srcType, StorageType dstType, byte srcSlot, byte dstSlot, int warp,
        bool deleteItemUponUse = false)
    {
        var packet = P373_UseItem.Create(srcType, dstType, srcSlot, dstSlot, _wClient.Player.Position.Adapt<NetworkPosition>(), warp);

        _wClient.Socket.SendEncrypted(packet);
    }
    
    public void SendTrade(ushort target, int gold, List<byte> slots, bool confirm)
    {
        var items = Inventory.Where(x => slots.Contains(x.Slot)).ToList();
        var carryPos = items.Select(x => x.Slot).ToArray();

        var tradeSlots = Enumerable.Repeat((byte) 255, 15).ToArray();
        carryPos.CopyTo(tradeSlots, 0);

        var tradeItems = Enumerable.Range(0, 15).Select(x => NetworkItem.Create()).ToArray();
        var networkItems = items.Select(x => x.Item).ToArray();
        
        networkItems.CopyTo(tradeItems, 0);
        var packet = P383_Trade.Create(target, gold, tradeItems, tradeSlots, confirm);

        _wClient.Socket.SendEncrypted(packet);

        if (target == _currentTradingWith && !_acceptedTrade)
        {
            _acceptedTrade = true;
        }
    }

    public void SendDeleteItem(StorageType storageType, byte slot)
    {
        lock (_itemsLock)
        {
            var itemToDelete = _items.Single(x => x.Slot == slot && x.StorageType == storageType);
            var packet = P2E4_DeleteItem.Create(slot, itemToDelete.Item.Id);

            _wClient.Socket.SendEncrypted(packet);
            
            _wClient.Timer.Sleep(500);
            DecrementItem(storageType, slot);
            ItemChanged?.Invoke(_wClient, itemToDelete);
        }
        
        
    }

    public void SendMoveItem(StorageType srcType, StorageType dstType, byte dstSlot, byte srcSlot, int npcIndex)
    {
        var packet = P376_MovedItem.Create(dstType, dstSlot, srcType, srcSlot, npcIndex);

        _wClient.Socket.SendEncrypted(packet);

        _wClient.Timer.Sleep(500);
    }

    #endregion

    #region receive

    public void ReceiveWorld(P114_SentToWorld p114SentToWorld)
    {
        lock (_itemsLock)
        {
            _items.RemoveAll(x => x.StorageType == StorageType.Inventario);
            _items.RemoveAll(x => x.StorageType == StorageType.Equipamentos);
            _items.AddRange(p114SentToWorld.Player.EntityInventory.Select((x, index) => new BagItem
            {
                StorageType = StorageType.Inventario,
                Item = x,
                Slot = (byte)index
            }));
            _items.AddRange(p114SentToWorld.Player.Equip.Select((x, index) => new BagItem
            {
                StorageType = StorageType.Equipamentos,
                Item = x,
                Slot = (byte)index
            }));
            BagChanged?.Invoke(_wClient, _items.ToList());
        }

        Andarilho = p114SentToWorld.Player.Andarilho;
    }

    internal void ReceiveItem(P182_RcvItem packetData)
    {
        lock (_itemsLock)
        {
            var currentItem = _items.Single(x => x.Slot == packetData.Slot && x.StorageType == packetData.Type);

            currentItem.Item = packetData.NetworkItem;
            
            _wClient.Log(MessageRelevance.High, $"New item received {currentItem.GetName()} at slot {currentItem.Slot} in Storage {currentItem.StorageType}");
            
            _wClient.Works.GetAll().Where(x => x.GetType() == typeof(DropWorker)).ToList().ForEach(x =>
            {
                x.Notify(currentItem);
            });
            
            ItemChanged?.Invoke(_wClient, currentItem);
        }

    }

    internal void ReceiveCharlist(P10A_CharList receiveCharListPacket)
    {
        lock (_itemsLock)
        {
            _items.RemoveAll(x => x.StorageType == StorageType.Cargo);
            _items.AddRange(receiveCharListPacket.Cargo.Select((x, index) => new BagItem
            {
                StorageType = StorageType.Cargo,
                Item = x,
                Slot = (byte)index
            }));
        }
    }

    internal void ReceivedTrade(P383_Trade trade)
    {
        var tradeInfo = new TradeInfo();

        tradeInfo.Items = trade.Items;
        tradeInfo.Gold = trade.Gold;
        tradeInfo.PlayerName = _wClient.MobGrid.GetMobById(trade.Target)?.Name;
        tradeInfo.PlayerId = trade.Target;

        _currentTradingWith = trade.Target;
        
        if (trade.Target != _currentTradingWith && _acceptedTrade)
        {
            _acceptedTrade = false;

        }
        else if (_acceptedTrade && trade.Target == _currentTradingWith && trade.Items.All(x => x.Id == 0) &&
                 trade.Gold == default && trade.MyCheck == 0)
        {
            _acceptedTrade = false;
        }
        tradeInfo.Accepted = _currentTradingWith == trade.Target && _acceptedTrade;

        TradeReceived?.Invoke(_wClient, tradeInfo);
    }
    
    public void ReceiveUpdateInventory(P185_UpdateInventory updateInventoryData)
    {
        lock (_itemsLock)
        {
            _items.RemoveAll(x => x.StorageType == StorageType.Inventario);
            _items.AddRange(updateInventoryData.Inventory.Select((x, index) => new BagItem
            {
                StorageType = StorageType.Inventario,
                Item = x,
                Slot = (byte)index
            }));
        }
    }

    internal void ReceiveMovedItem(P376_MovedItem packetData)
    {
        Console.WriteLine(
            $"MOVED {packetData.SrcType} / {packetData.DstType} to {packetData.SlotOrigin} / {packetData.SlotDst}");
        lock (_itemsLock)
        {
            var originItem = _items.Single(x => x.StorageType == packetData.SrcType && x.Slot == packetData.SlotOrigin);
            var destinationItem =
                _items.Single(x => x.StorageType == packetData.DstType && x.Slot == packetData.SlotDst);

            originItem.StorageType = packetData.DstType;
            originItem.Slot = packetData.SlotDst;

            destinationItem.StorageType = packetData.SrcType;
            destinationItem.Slot = packetData.SlotOrigin;

            Console.WriteLine(
                $"Item {originItem.GetName()} is now at {originItem.StorageType} at slot {originItem.Slot}");
            Console.WriteLine(
                $"Item {destinationItem.GetName()} is now at {destinationItem.StorageType} at slot {destinationItem.Slot}");
            ItemChanged?.Invoke(_wClient, destinationItem);
            ItemChanged?.Invoke(_wClient, originItem);

        }
    }

    #endregion

    public void ReceiveItemBuy(P379_BuyNpcItem buyedItemData)
    {
        var npc = NpcService.GetById(buyedItemData.NpcIndex);
        if (npc is null)
        {
            Console.WriteLine("Received buy from unknown npc");
            return;
        }

        var npcItem = npc.Shop.FirstOrDefault(x => x.CarrierSlot == buyedItemData.NpcSlot);

        if (npcItem is null)
        {
            Console.WriteLine($"Received unknown item from npc {npc.Mob.Name}");
            return;
        }

        lock (_itemsLock)
        {
            var currentItemOnSlot =
                _items.First(x => x.Slot == buyedItemData.DestSlot && x.StorageType == StorageType.Inventario);

            currentItemOnSlot.Item = npcItem.Item;
            ItemChanged?.Invoke(_wClient, currentItemOnSlot);

        }
    }
    
    
    

    
}