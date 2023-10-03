using WYD.Frame.Game.Models;
using WYD.Frame.Models.Models;

namespace WYD.Frame.Game.Contracts;

public interface IClient
{
    ClientConfiguration Configuration { get; }
    Bag Bag { get; }
    Social Social { get; }
    Timer Timer { get; }
    World World { get; }
    Player Player { get; }
    Charlist Charlist { get; }
    CommunicationControl CommunicationControl { get; }
}