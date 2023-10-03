using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.GameEvents;
using WYD.Frame.Game.Models;
using WYD.Frame.Game.Models.Game;
using WYD.Frame.Models.Models;
using WYD.Frame.Services;

namespace WYD.Frame.Game.Workers;

public class DropWorker : WorkerBase
{
    public DropWorker(WClient wClientOwner, List<DropItem> dropItems) : base(wClientOwner)
    {
        _packItems = dropItems.Where(x => x.DropActionType == DropActionType.Pack).ToList();
        _keepItems = dropItems.Where(x => x.DropActionType == DropActionType.Keep).ToList();
        _useItems = dropItems.Where(x => x.DropActionType == DropActionType.Use).ToList();
    }

    private readonly List<DropItem> _packItems;
    private readonly List<DropItem> _keepItems;
    private readonly List<DropItem> _useItems;

    protected override void DoWork()
    {
    }

    public override void Notify(object? args)
    {
        if (args is null) return;

        var receivedItem = args as BagItem;

        if (receivedItem is null) return;

        ProcessReceivedItem(receivedItem);
    }

    private void ProcessReceivedItem(BagItem receivedItem)
    {
        if (receivedItem.StorageType != StorageType.Inventario) return;
        
        if (receivedItem.Item.Id == 0) return;

        if (_packItems.Any(x => x.ItemId == receivedItem.Item.Id))
        {
            WClientOwner.Works.PauseAll(Id);
            PackItem(receivedItem);
            WClientOwner.Works.ResumeAll();
        }
        else if (_useItems.Any(x => x.ItemId == receivedItem.Item.Id))
        {
            WClientOwner.Works.PauseAll(Id);
            UseItem(receivedItem);
            WClientOwner.Works.ResumeAll();
        }
        else
        {
            var incomingFromItemList = ResourceService.Items[receivedItem.Item.Id];
            var defaultConfig = _keepItems.Where(x => x.ItemId is 9999 or 9998).Select(x => new
            {
                Config = x.DropItemEfvConfigs,
                Type = x.ItemId == 9998 ? ItemType.Weapon : ItemType.Armor,
                ItemClass = x.DropItemClass
            }).ToList();

            if (defaultConfig.Any())
            {
                var fromSameType = defaultConfig.Where(x => x.Type == incomingFromItemList.Type);
                foreach (var sameType in fromSameType)
                {
                    if (sameType.ItemClass != ItemClass.None &&
                        sameType.ItemClass != incomingFromItemList.ItemClass) continue;

                    if (EfvIsValid(sameType.Config, receivedItem))
                    {
                        WClientOwner.Log(MessageRelevance.Highest,
                            $"From default type, item {receivedItem.GetName()} has been keep.");
                        return;
                    }
                }
            }
            else
            {
                foreach (var config in _keepItems.Where(x => x.ItemId == receivedItem.Item.Id))
                {
                    if (config.DropItemClass != ItemClass.None &&
                        config.DropItemClass != incomingFromItemList.ItemClass) continue;

                    if (EfvIsValid(config.DropItemEfvConfigs, receivedItem))
                    {
                        WClientOwner.Log(MessageRelevance.Highest,
                            $"From keep, item {receivedItem.GetName()} has been keep.");
                        return;
                    }
                }
            }

            var currentItemConfig = _keepItems.Where(x => x.ItemId == receivedItem.Item.Id).ToList();

            if (currentItemConfig.Any() && currentItemConfig.All(x => !x.DropItemEfvConfigs.Any()))
            {
                WClientOwner.Log(MessageRelevance.Medium, $"From keep, item {receivedItem.GetName()} has been keep because there is no efv config for it.");
                return;
            }
            
            WClientOwner.Works.PauseAll(Id);
            DeleteItem(receivedItem);
            WClientOwner.Works.ResumeAll();

            
        }
    }

    private bool EfvIsValid(List<DropItemEfvConfig> configs, BagItem receivedItem)
    {
        if (configs.Count <= 0)
            return true;

        var currentItensEfv = receivedItem.Item.Ef.GroupBy(x => x.Type).Select(x => new
        {
            Type = x.Key,
            Value = x.Sum(z => z.Value)
        });

        var valid = configs.All(x => currentItensEfv.Any(z => z.Type == x.Efv && z.Value >= x.Value));

        return valid;
    }

    private void DeleteItem(BagItem targetItem)
    {
        WClientOwner.Log(MessageRelevance.High,
            $"Deleting item [{targetItem.GetName()}] at slot {targetItem.Slot}. Efvs {string.Join(",", targetItem.Item.Ef.Select(x => $"{x.Type}:{x.Value}"))}");
        WClientOwner.Bag.SendDeleteItem(targetItem.StorageType, targetItem.Slot);
        WClientOwner.Timer.Sleep(200);
    }

    private void UseItem(BagItem targetItem)
    {
        WClientOwner.Bag.SendUseItem(targetItem.StorageType, default, targetItem.Slot, default, default);
        WClientOwner.Log(MessageRelevance.High, $"Using item {targetItem.GetName()} at slot {targetItem.Slot}");

        WClientOwner.Timer.Sleep(200);
    }


    private void PackItem(BagItem targetItem)
    {
        if (targetItem.StorageType != StorageType.Inventario) return;

        var destinationItem = WClientOwner.Bag.GetItemById(StorageType.Inventario, targetItem.Item.Id, true,
            targetItem.Slot);

        if (destinationItem is null) return;


        if (!IsDestinationItemElegible(targetItem, destinationItem))
        {
            return;
        }

        if (targetItem.Amount() > destinationItem.Amount())
        {
            WClientOwner.Bag.SendMoveItem(destinationItem.StorageType, targetItem.StorageType, targetItem.Slot,
                destinationItem.Slot, 0);
            WClientOwner.Log(MessageRelevance.High,
                $"Pack item {targetItem.GetName()} at slot {targetItem.Slot} with {destinationItem.GetName()} at slot {destinationItem.Slot}");
        }
        else
        {
            WClientOwner.Bag.SendMoveItem(targetItem.StorageType, destinationItem.StorageType, destinationItem.Slot,
                targetItem.Slot, 0);
            WClientOwner.Log(MessageRelevance.High,
                $"Pack item {destinationItem.GetName()} at slot {destinationItem.Slot} with {targetItem.GetName()} at slot {targetItem.Slot}");
        }


        WClientOwner.Timer.Sleep(200);
    }

    public bool IsDestinationItemElegible(BagItem targetItem, BagItem destinationItem)
    {
        var amountOrigin = targetItem.Item.Ef.FirstOrDefault(x => x.Type == GameEfv.EF_AMOUNT).Value;

        if (amountOrigin == 0) amountOrigin = 1;

        var amountDst = destinationItem.Item.Ef.FirstOrDefault(x => x.Type == GameEfv.EF_AMOUNT).Value;

        if (amountDst == 0) amountDst = 1;

        if (amountDst + amountOrigin > 100)
            return false;

        return true;
    }
}