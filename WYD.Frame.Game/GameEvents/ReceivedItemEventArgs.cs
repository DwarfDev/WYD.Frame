using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game.GameEvents;

public class ReceivedItemEventArgs
{
    public NetworkItem Item { get; set; }
    public StorageType StorageType { get; set; }
}