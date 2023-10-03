using WYD.Frame.Services.Models.Npc;

namespace WYD.Frame.Services;

public static class NpcService
{
    private static readonly object _npcsLock = new();
    public static readonly byte[] EvoksMerchants = { 35, 15, 16 };
    private static readonly List<Npc> _npcs = new();

    public static Npc? GetByName(string name, int maxRange = 0)
    {
        lock (_npcs)
        {
            return _npcs.FirstOrDefault(x => x.Mob.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public static List<Npc> GetAllByName(string name, int maxRange = 0)
    {
        lock (_npcs)
        {
            return _npcs.Where(x => x.Mob.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }

    public static void SetShop(ushort clientId, List<ShopItem> shopItems)
    {
        lock (_npcs)
        {
            var currentNpc = _npcs.FirstOrDefault(x => x.Mob.ClientId == clientId);

            if (currentNpc is null) Console.WriteLine($"Failed to set shop for {clientId}");
        }
    }
    
   

    public static void Add(Npc gameEntity)
    {
        if (gameEntity.Mob.Status.Race is 0 or 16 ||
            EvoksMerchants.Contains(gameEntity.Mob.Status.Merchant) || gameEntity.Mob.ClientId <= 1000) return;

        lock (_npcs)
        {
            var currentNpc = _npcs.FirstOrDefault(x => x.Mob.ClientId == gameEntity.Mob.ClientId);
            if (currentNpc != null)
            {
                currentNpc.Mob = gameEntity.Mob;
                if (gameEntity.Shop.Count > 0)
                    currentNpc.Shop = gameEntity.Shop;
                return;
            }

            _npcs.Add(gameEntity);
            Console.WriteLine($"[NpcService] new npc {gameEntity.Mob.Name}");
        }
    }


    public static IEnumerable<Npc> GetUndiscoverd()
    {
        lock (_npcsLock)
        {
            return _npcs.Where(x => !x.Discovered && x.Shop.All(z => z.Item.Id == 0)).ToList();
        }
    }

    public static Npc? GetById(ushort npcIndex)
    {
        lock (_npcsLock)
        {
            return _npcs.FirstOrDefault(x => x.Mob.ClientId == npcIndex);
        }
    }
}