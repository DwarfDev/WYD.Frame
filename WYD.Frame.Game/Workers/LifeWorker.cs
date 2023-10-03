using System.Collections.Immutable;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Helpers;
using WYD.Frame.Services;
using WYD.Frame.Services.Models.Resource;

namespace WYD.Frame.Game.Workers;

public class LifeWorker : WorkerBase
{
    public int RationPercentage { get; set; }
    public int HpPercentage { get; set; }
    public int MpPercentage { get; set; }
    public bool Predict { get; set; } = false;

    private List<int> _hpPotsIds;
    private List<ItemInfo> _hpPots;
    private List<ItemInfo> _mpPots;
    private List<int> _mpPotsIds;
    private bool potsSet = false;

    public LifeWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }

    private void SetPots()
    {
        if (potsSet) return;
        _hpPots = ResourceService.Items.Where(IsHpPotItem).ToList();
        _hpPotsIds = _hpPots.Select(x => x.Id).ToList();
        _mpPots = ResourceService.Items.Where(IsMpPotItem).ToList();
        _mpPotsIds = _hpPots.Select(x => x.Id).ToList();
    }

    protected override void DoWork()
    {
        SetPots();
        ProcessRation();
        ProcessHpMp();
        Thread.Sleep(100);
    }

    private void ProcessHpMp()
    {
        var currentPlayerHpPercentage = WClientOwner.Player.Status.CurHp * 100 / WClientOwner.Player.Status.MaxHp;

        if (currentPlayerHpPercentage <= HpPercentage)
        {
            if (_hpPotsIds != null)
            {
                var potItem = WClientOwner.Bag.Inventory.FirstOrDefault(x => _hpPotsIds.Contains(x.Item.Id));
                if (potItem is not null)
                {
                    Console.WriteLine($"Usando pot hp {potItem.Item.Id} / {potItem.GetName()}");

                    if (Predict)
                    {
                        var itemListItem = _hpPots.First(x => x.Id == potItem.Item.Id);
                        var increment = itemListItem.ItemListItem.GetItemEfv(GameEfv.EF_HP).sValue;
                        if (increment + WClientOwner.Player.Status.CurHp >= WClientOwner.Player.Status.MaxHp)
                            WClientOwner.Player.Status.CurHp = WClientOwner.Player.Status.MaxHp;
                        else
                            WClientOwner.Player.Status.CurHp += increment;
                    }

                    WClientOwner.Bag.SendUseItem(potItem.StorageType, default, potItem.Slot, default, default, false);
                    WClientOwner.Timer.Sleep(1000);
                }
            }
        }

        if (currentPlayerHpPercentage <= MpPercentage)
        {
            if (_mpPotsIds != null)
            {
                var potItem = WClientOwner.Bag.Inventory.FirstOrDefault(x => _mpPotsIds.Contains(x.Item.Id));
                if (potItem is not null)
                {
                    Console.WriteLine($"Usando pot mp {potItem.Item.Id} / {potItem.GetName()}");

                    if (Predict)
                    {
                        var itemListItem = _hpPots.First(x => x.Id == potItem.Item.Id);
                        var increment = itemListItem.ItemListItem.GetItemEfv(GameEfv.EF_HP).sValue;
                        if (increment + WClientOwner.Player.Status.CurHp >= WClientOwner.Player.Status.MaxHp)
                            WClientOwner.Player.Status.CurMp = WClientOwner.Player.Status.MaxMp;
                        else
                            WClientOwner.Player.Status.CurMp += increment;
                    }

                    WClientOwner.Bag.SendUseItem(potItem.StorageType, default, potItem.Slot, default, default, false);
                    WClientOwner.Timer.Sleep(1000);
                }
            }
        }

        WClientOwner.Timer.Sleep(1000);
    }

    private bool IsHpPotItem(ItemInfo arg)
    {
        var itemListItem = arg.ItemListItem;

        var stEffects = itemListItem.stEffect;

        var isHpItem = stEffects?.Any(z => (GameEfv)z.sEffect == GameEfv.EF_HP);
        var isVolatile = stEffects?.Any(z => (GameEfv)z.sEffect == GameEfv.EF_VOLATILE && z.sValue == 1);

        if (!isHpItem.HasValue || !isVolatile.HasValue) return false;
        return isHpItem.Value && isVolatile.Value;
    }

    private bool IsMpPotItem(ItemInfo arg)
    {
        var itemListItem = arg.ItemListItem;

        var stEffects = itemListItem.stEffect;

        var isHpItem = stEffects?.Any(z => (GameEfv)z.sEffect == GameEfv.EF_MP);
        var isVolatile = stEffects?.Any(z => (GameEfv)z.sEffect == GameEfv.EF_VOLATILE && z.sValue == 1);

        if (!isHpItem.HasValue || !isVolatile.HasValue) return false;
        return isHpItem.Value && isVolatile.Value;
    }

    private void ProcessRation()
    {
        var mount = WClientOwner.Bag.Equips.First(x => x.Slot == 14);
        var mountRationPercentage = (int)mount.Item.Ef[2].Type;
        var mountHpPercentage = (int)(mount.Item.Ef[0].Value * 100 / 39);

        if (mountHpPercentage < RationPercentage || mountRationPercentage < RationPercentage)
        {
            var rationId = Utils.GetCorrectRationForCurrentMount(mount.Item.Id);

            var rationInventory = WClientOwner.Bag.GetItemById(StorageType.Inventario, rationId);

            if (rationInventory is null)
            {
                WClientOwner.Log(MessageRelevance.High, "Sem ração para a montaria.");
                return;
            }

            WClientOwner.Bag.SendUseItem(StorageType.Inventario, StorageType.Equipamentos, rationInventory.Slot, 14,
                default);
            WClientOwner.Bag.DecrementItem(StorageType.Inventario, rationInventory.Slot);
            WClientOwner.Timer.Sleep(1000);
        }
    }
}