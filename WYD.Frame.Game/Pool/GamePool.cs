namespace WYD.Frame.Game.Pool;

public static class GamePool
{
    private static readonly IDictionary<Guid, WClient> _clients = new Dictionary<Guid, WClient>();

    private static readonly object _clientsLock = new();

    public static void Add(WClient wClient)
    {
        lock (_clientsLock)
            _clients.Add(wClient.Id, wClient);
    }

    public static void RemoveById(Guid id)
    {
        lock (_clientsLock)
            _clients.Remove(id);
    }

    public static WClient? TryGetClient(Guid id)
    {
        lock (_clientsLock)
        {
            if (_clients.TryGetValue(id, out var client))
                return client;

            return null;
        }
    }
    
    public static WClient? TryGetClientByClientId(ushort id)
    {
        lock (_clientsLock)
        {
            return _clients.Values.FirstOrDefault(x => x.Player.ClientId == id);
        }
    }

    public static WClient GetClient(Guid id)
    {
        lock (_clientsLock)
        {
            if (_clients.TryGetValue(id, out var client))
                return client;
            else
            {
                throw new Exception("O cliente informado não está ativo.");
            }
        }
    }


    public static IEnumerable<WClient> GetAllClients()
    {
        lock (_clientsLock)
            return _clients.Select(x => x.Value).ToList();
    }
}