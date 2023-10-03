using WYD.Frame.Common.Enum;

namespace WYD.Frame.Game.Models.Game;

public class DropItem
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public DropActionType DropActionType { get; set; }
    public List<DropItemEfvConfig> DropItemEfvConfigs { get; set; } = new();
    public ItemClass DropItemClass { get; set; }
}