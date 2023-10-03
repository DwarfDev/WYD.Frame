using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game.Workers;

public class PartyWorker: WorkerBase
{
    public List<string> GroupAlways { get; set; }
    public List<string> AcceptAlways { get; set; }
    public PartyWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }
    public override void Notify(object? args)
    {
        if (args is null) return;

        var receivedPartyInfo = args as PartyReceivedArgs;
        if (receivedPartyInfo is null) return;

        if (AcceptAlways.Any(x =>
                x.Equals(receivedPartyInfo.LeaderName, StringComparison.InvariantCultureIgnoreCase)))
        {
            WClientOwner.Player.SendAcceptParty(receivedPartyInfo.LeaderId, receivedPartyInfo.LeaderName);
        }
    }

    public override void Stop()
    {
        WClientOwner.Player.SendLeaveParty();
        WClientOwner.Player.Party = new List<NetworkParty>();
        base.Stop();
    }

    protected override void DoWork()
    {
        if (WClientOwner.Player.Party.Count > 0)
        {
            var leader = WClientOwner.Player.Party.FirstOrDefault(x => x.PartyIndex == 0);

            if (leader.ClientId != default && leader.ClientId != WClientOwner.Player.ClientId)
            {
                //Não é lider.
                Thread.Sleep(15000);
                return;
            }
        }
        foreach (var groupAlways in GroupAlways)
        {
            if (WClientOwner.Player.Party.Any(z =>
                    z.Name.Equals(groupAlways, StringComparison.InvariantCultureIgnoreCase))) continue;
            
            var currentTarget = WClientOwner.MobGrid.GetMobByName(groupAlways);
            if(currentTarget is null) continue;
            
            if(WClientOwner.Player.Party.Any(z => z.ClientId == currentTarget.ClientId)) continue;

            WClientOwner.Player.SendPartyRequest(currentTarget.ClientId);
        }
        
        Thread.Sleep(10000);
    }

    internal class PartyReceivedArgs
    {
        public string LeaderName { get; set; }
        public ushort LeaderId { get; set; }
    }
}