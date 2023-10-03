using Mapster;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Game.Workers;
using WYD.Frame.Models.Models;
using WYD.Frame.Packets.Network;
using WYD.Frame.Services;
using WYD.Frame.Services.Models.Npc;

namespace WYD.Frame.Game;

public class World
{
    private readonly WClient _wClient;

    public World(WClient wClient)
    {
        _wClient = wClient;
    }
    public ushort LastShopRequestIndex { get; set; }

    #region Send

    /// <summary>
    ///     Send NPC click
    /// </summary>
    /// <param name="npcIndex">The npc client id</param>
    public void SendNpcClick(ushort npcIndex)
    {
        var packet = P28B_Click.Create(npcIndex);
        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Use a portal. You need to be on it.
    /// </summary>
    public void SendPortal()
    {
        var packet = new P290_UsePortal();
        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Buy an NPC item
    /// </summary>
    /// <param name="npcSlot">
    ///     The NPC Slot. From ShopList the calculation is x = indexFromShop % 5, y = indexFromShop / 5, then
    ///     r = x + 5 * y then r % 9 + 27 * (r/9)
    ///
    ///  For base Skills, subtract 5000 from the shoplist to have the skillList id.
    /// </param>
    /// <param name="dstSlot">Destination slot</param>
    /// <param name="npcIndex">Npc Client Id</param>
    public void SendBuyItem(short npcSlot, short dstSlot, ushort npcIndex)
    {
        var packet = P379_BuyNpcItem.Create(npcSlot, dstSlot, npcIndex);

        _wClient.Socket.SendEncrypted(packet);
    }
    
    /// <summary>
    ///     Buy an NPC item
    /// </summary>
    /// <param name="npcSlot">
    ///     The NPC Slot. From ShopList the calculation is x = indexFromShop % 5, y = indexFromShop / 5, then
    ///     r = x + 5 * y then r % 9 + 27 * (r/9)
    /// </param>
    /// <param name="npcIndex">Npc Client Id</param>
    public void SendUseNpcItem(int npcSlot, ushort npcIndex)
    {
        var packet = P381_ClickNpcItem.Create(npcIndex, npcSlot);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Send a signal to the server, informing you gonna do something delayed. Eg: using a portal scroll.
    /// </summary>
    /// <param name="warpIndex">I don't know exactly what this do. Usually warpIndex is 1.</param>
    public void SendDelayedActionSignal(int warpIndex)
    {
        var packet = P3AE_TeleportSignal.Create(warpIndex);

        _wClient.Socket.SendEncrypted(packet);
    }

    public void SendRequestShopList(ushort mobClientId)
    {
        var packet = P27B_ReqShopList.Create(mobClientId);

        _wClient.Socket.SendEncrypted(packet);
    }

    #endregion

    #region Receive

    public void ReceiveMobDeath(P338_MobDeath packetData)
    {
        if (packetData.Killed == _wClient.Player.ClientId)
        {
            _wClient.Player.Status.CurHp = 0;
            var killerMob = _wClient.MobGrid.GetMobById(packetData.Killer)?.Name ?? "Desconhecido";
            
            if(_wClient.Configuration.GeneralConfig.Behavior.NotifyGuilded)
                GuildedService.Log(GuildedLogType.Death, $"Conta morta: {_wClient.Player.Name}.", $"Morto por {killerMob}",  DateTime.Now.ToShortDateString());

            if (_wClient.Configuration.GeneralConfig.Behavior.ReviveRandom)
            {
                Task.Delay(Utils.Rand.Next(10, 60) * 1000).ContinueWith(task =>
                {
                    _wClient.Player.SendRevive();
                    if(_wClient.Configuration.GeneralConfig.Behavior.NotifyGuilded)
                        GuildedService.Log(GuildedLogType.Death, "Revivendo conta morta.", $"Morto por {killerMob}",  DateTime.Now.ToShortDateString());
                });
            }
            else if(_wClient.Configuration.GeneralConfig.Behavior.ReviveAfterSeconds > 0)
            {
                Task.Delay(_wClient.Configuration.GeneralConfig.Behavior.ReviveAfterSeconds * 1000).ContinueWith(task =>
                {
                    _wClient.Player.SendRevive();
                    if(_wClient.Configuration.GeneralConfig.Behavior.NotifyGuilded)
                        GuildedService.Log(GuildedLogType.Death, "Revivendo conta morta.", $"Morto por {killerMob}",  DateTime.Now.ToShortDateString());
                });
            }

            if (_wClient.Configuration.GeneralConfig.Behavior.TurnoffWorkersOnDeath)
            {
                GuildedService.Log(GuildedLogType.Behavior, $"Desligando todos os workers da conta {_wClient.Player.Name}", $"Shutdown",  DateTime.Now.ToShortDateString());
                _wClient.Works.GetAll().ToList().ForEach(x =>
                {
                    if (x.GetType() != typeof(SignalWorker))
                    {
                        x.Stop();
                    }
                });
            }
        }
        
        _wClient.Works.GetAll().ToList().ForEach(work =>
        {
            if (work.GetType() != typeof(SkillWorker)) return;
            
            work.Notify(packetData.Killed);
        });

        var deadMob = _wClient.MobGrid.GetMobById(packetData.Killed);
        if (deadMob is null) return;

        deadMob.Status.CurHp = 0;
    }

    internal void ReceiveMobMove(P36C_Move packetData)
    {
        if (packetData.Header.ClientId == _wClient.Player.ClientId)
        {
            _wClient.Player.ReceiveMove(packetData.To);
            return;
        }

        var mob = _wClient.MobGrid.GetMobById(packetData.Header.ClientId);
        if (mob is null)
        {
            Console.WriteLine($"Unknown mob {packetData.Header.ClientId} moved {packetData.To.Y} / {packetData.To.X}");
            return;
        }
        
        mob.Position.X = packetData.To.X;
        mob.Position.Y = packetData.To.Y;
        //
        // if (mob.Name.Contains(EventService.CurrentLookingFor))
        // {
        //     Console.WriteLine($"Updated pos to {mob.Position.X} / {mob.Position.Y}");
        //     EventService.Saw(mob);
        // }
        
    }

    internal void ReceiveMobStop(P366_Stop packetData)
    {
        if (packetData.Header.ClientId == _wClient.Player.ClientId)
        {
            _wClient.Player.ReceiveMove(packetData.To);
            return;
        }

        var mob = _wClient.MobGrid.GetMobById(packetData.Header.ClientId);
        if (mob is null)
        {
            return;
        }

        mob.Position = packetData.To.Adapt<Position>();
    }

    internal void ReceiveMobTrade()
    {
    }
    

    internal void ReceiveNewMob(P364_NewMob packetData)
    {
        if (packetData.Mob.ClientId == _wClient.Player.ClientId)
        {
            _wClient.Player.ReceiveMove(packetData.Mob.NetworkPosition);
            return;
        }

        var newMob = Mob.Create(packetData.Mob);
        NpcService.Add(new Npc()
        {
            Mob = newMob
        });

        _wClient.MobGrid.Add(newMob);

        // if (newMob.Name.Contains(EventService.CurrentLookingFor))
        // {
        //     Console.WriteLine($"Saw ... {newMob} at {newMob.Position.X} / {newMob.Position.Y}");
        //     EventService.Saw(newMob);
        // }
    }

    internal void ReceiveRemoveMob(P165_RemoveMob packetData)
    {
        _wClient.MobGrid.Remove(packetData.Header.ClientId);
    }
    internal void ReceiveShopList(P17C_ShopList packetData)
    {
        var lastClickedNpc = _wClient.World.LastShopRequestIndex;
        
        var mob = _wClient.MobGrid.GetMobById((ushort)lastClickedNpc);
        if (mob is null)
        {
           Console.WriteLine("[NpcService] Clicked shop doesnt exists");
            _wClient.World.LastShopRequestIndex = 0;
            return;
        }
        
        var npc = new Npc()
        {
            Mob = mob
        };
        
        Console.WriteLine($"[NpcService] received shop for {mob.Name} Items: {string.Join("," , packetData.Items.Select(x => x.Id))}");
        
        for (int i = 0; i < packetData.Items.Length; i++)
        {
            var x = i % 5;
            var y = i / 5;
            var sourPos = x + 5 * y;
            var carrierPos = sourPos % 9 + 27 * (int)(sourPos / 9);
            npc.Shop.Add(new ShopItem()
            {
                Item = packetData.Items[i],
                CarrierSlot = carrierPos
            });
        }
        
        NpcService.Add(npc);
    }


    #endregion


    #region Helpers
    public bool DiscoverNpc(short npcIndex)
    {
        var npc = NpcService.GetById((ushort)npcIndex);

        if (npc is null) return false;

        if (npc.Discovered) return true;

        if (_wClient.Player.Position.GetDistance(npc.Mob.Position) > 17)
        {
            Console.WriteLine($"[Discovery] Too far away {npc.Mob.Name}");
            return false;
        }

        _wClient.World.LastShopRequestIndex = npc.Mob.ClientId;
        Console.WriteLine($"[Discovery] Getting for {npc.Mob.Name} / {npc.Mob.ClientId}");

        _wClient.World.SendRequestShopList(npc.Mob.ClientId);

        for (var i = 0; i < 5; i++)
        {
            if (_wClient.World.LastShopRequestIndex == 0) break;
            _wClient.Timer.Sleep(200);
        }

        npc.Discovered = true;

        _wClient.World.LastShopRequestIndex = 0;

        return true;
    }
    
    #endregion

  
}