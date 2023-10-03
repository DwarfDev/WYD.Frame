using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game;

public class Charlist
{
    public List<string> Charnames { get; set; } = new();
    public EventHandler<List<string>>? CharlistChanged;

    private readonly WClient _wClient;
    public Charlist(WClient wClient)
    {
        _wClient = wClient;
    }
    internal void ReceiveCharlist(P10A_CharList packetData)
    {
        Charnames = packetData.NetworkCharlist.Name.Select(x => x.Name).ToList();
    }
    
    internal void ReceiveUpdateCharlist(P110_UpdateCharlist packetData)
    {
        Charnames = packetData.NetworkCharlist.Name.Select(x => x.Name).ToList();
        if (_wClient.Status == ConnectionStatus.NoCharacters)
        {
            _wClient.Status = ConnectionStatus.CorrectNumeric;
        }
        
        CharlistChanged?.Invoke(_wClient, Charnames);
    }
}