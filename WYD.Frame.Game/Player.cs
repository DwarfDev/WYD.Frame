using Mapster;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Models.Game;
using WYD.Frame.Game.Workers;
using WYD.Frame.Models.Models;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game;

public class Player
{
    private readonly WClient _wClient;


    public Player(WClient wClient)
    {
        _wClient = wClient;
    }

    public PlayerStatus Status = new();
    public long Gold { get; private set; }
    public Position Position { get; set; } = new();
    public long Experience { get; private set; }
    private readonly object _evoksLocker = new();
    private readonly object _partyLocker = new();
    public List<Evok> Evoks { get; private set; } = new();
    public AffectInfo[] Affects { get; set; } = new AffectInfo[1];
    public List<NetworkParty> Party { get; set; } = new();
    public string Name { get; private set; }
    public ushort ClientId { get; set; }
    public bool Moving { get; set; }
    public MobClass ClassInfo { get; set; }
    public List<int> LearnedSkill { get; set; }
    public bool Dead => Status.CurHp <= 0;

    public bool IsOnCity =>
        IsOnArmia || //Armia
        IsOnUxmal ||
        (Position.X < 2580 && Position.X > 2433 && Position.Y > 1640 && Position.Y < 1751) ||
        (Position.X < 3683 && Position.X > 3633 && Position.Y > 3092 && Position.Y < 3160) ||
        (Position.X < 2476 && Position.X > 2448 && Position.Y > 1960 && Position.Y < 2024);

    public bool IsOnUxmal => (Position.X is < 3317 and > 3209 && Position.Y is > 1673 and < 1712);
    public bool IsOnArmia => (Position.X is < 2146 and > 2076 && Position.Y is > 2070 and < 2128);
    public bool IsOnEventArea => (Position.X is < 1381 and > 1302 && Position.Y is > 1483 and < 1516);


    #region Send

    /// <summary>
    ///     Send party request to target id
    /// </summary>
    /// <param name="targetId">The target id</param>
    public void SendPartyRequest(ushort targetId)
    {
        var partyPacket = NetworkParty.Create(ClientId, Name, default, (short)Status.Level, 100, 100, ClassInfo);
        var packet = P37F_PartyRequest.Create(partyPacket, targetId);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Send party kick to target id
    /// </summary>
    /// <param name="targetId">The target id</param>
    public void SendPartyKick(ushort targetId)
    {
        var partyPacket = P37E_PartyLeft.Create(targetId);
        _wClient.Socket.SendEncrypted(partyPacket);
    }

    /// <summary>
    ///     Send leave party
    /// </summary>
    public void SendLeaveParty()
    {
        var packet = P37E_PartyLeft.Create(ClientId);

        _wClient.Socket.SendEncrypted(packet);

        _wClient.Player.Party.Clear();
    }

    /// <summary>
    ///     Send accept party
    /// </summary>
    /// <param name="leaderId">The party leader id</param>
    /// <param name="leaderName">The party leader name</param>
    public void SendAcceptParty(ushort leaderId, string leaderName)
    {
        var packet = P3AB_AcceptParty.Create(leaderId, leaderName);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Move player
    /// </summary>
    /// <param name="dstX">Destination X</param>
    /// <param name="dstY">Destination Y</param>
    /// <param name="moveSpeed">Move speed</param>
    public void SendMove(short dstX, short dstY, byte moveSpeed)
    {
        var packet = P36C_Move.Create(new NetworkPosition
        {
            X = dstX,
            Y = dstY
        }, Position.Adapt<NetworkPosition>(), moveSpeed);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Move player
    /// </summary>
    /// <param name="dstX">Destination X</param>
    /// <param name="dstY">Destination Y</param>
    /// <param name="moveSpeed">Move speed</param>
    /// <param name="directions">Direções</param>
    public void SendMove(short dstX, short dstY, byte moveSpeed, byte[] directions)
    {
        var packet = P36C_Move.Create(new NetworkPosition
        {
            X = dstX,
            Y = dstY
        }, Position.Adapt<NetworkPosition>(), moveSpeed);

        packet.Directions = directions;

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Add Int,For,Dex or Con points. 1 per time.
    /// </summary>
    public void SendAddPoints(PointType pointType)
    {
        var packet = P277_BuyScore.Create(BonusType.Points, (short)pointType, default);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Buy the informed skill
    /// </summary>
    public void SendBuySkill(GameSkills skill)
    {
        var packet = P277_BuyScore.Create(BonusType.Skills, (short)(skill + 5000), default);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Add linhagem points 1 per time.
    /// </summary>
    public void SendAddLinhagem(LinhagemType pointType)
    {
        var packet = P277_BuyScore.Create(BonusType.Linhagem, (short)pointType, default);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Revive the player
    /// </summary>
    public void SendRevive()
    {
        var packet = new P289_Revive();

        _wClient.Socket.SendEncrypted(packet);
    }


    /// <summary>
    ///     Attack multiple targets
    /// </summary>
    /// <param name="skillId">Skill id</param>
    /// <param name="targets">Targets</param>
    /// <param name="mainTarget">Main target</param>
    /// <exception cref="NotImplementedException"></exception>
    public void SendMultiAttack(GameSkills skillId, ushort mainTarget, ushort[] targets)
    {
        NetworkPosition targetPosition;
        if (mainTarget == _wClient.Player.ClientId)
        {
            targetPosition = _wClient.Player.Position.Adapt<NetworkPosition>();
        }
        else
        {
            var mainTargetPos = _wClient.MobGrid.GetMobById(mainTarget);
            if (mainTargetPos is null) return;
            targetPosition = mainTargetPos.Position.Adapt<NetworkPosition>();
        }

        var sendAttack = NetworkSendAttack.Create(ClientId, _wClient.Player.Position.Adapt<NetworkPosition>(),
            targetPosition, skillId);

        var packet = P367_SendMultiAtk.Create(targets, sendAttack);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Attack or buff single target
    /// </summary>
    /// <param name="skillId">Skill id</param>
    /// <param name="mainTarget">The target</param>
    public void SendSingleAttack(GameSkills skillId, ushort mainTarget)
    {
        NetworkPosition targetPosition;
        if (mainTarget == _wClient.Player.ClientId)
        {
            targetPosition = _wClient.Player.Position.Adapt<NetworkPosition>();
        }
        else
        {
            var mainTargetPos = _wClient.MobGrid.GetMobById(mainTarget);
            if (mainTargetPos is null) return;
            targetPosition = mainTargetPos.Position.Adapt<NetworkPosition>();
        }

        var sendAttack = NetworkSendAttack.Create(ClientId, _wClient.Player.Position.Adapt<NetworkPosition>(),
            targetPosition, skillId);

        var packet = P39D_SendSingleAtk.Create(mainTarget, sendAttack);


        _wClient.Socket.SendEncrypted(packet);
    }

    #endregion

    #region Receive

    internal void ReceiveUpdateGold(P3AF_UpdateGold packetData)
    {
        Gold = packetData.Gold;
    }

    internal void ReceiveUpdateScore(P336_UpdateScore packetData)
    {
        if (packetData.Header.ClientId != ClientId) return;

        Status.UpdateStatus(packetData.Status);
        _wClient.ScoreUpdated?.Invoke(_wClient, Status);
    }

    internal void ReceiveUpdateHealth(P181_UpdateHpMp packetData)
    {
        if (packetData.Header.ClientId == ClientId)
        {
            Status.CurHp = packetData.ToHP;
            Status.CurMp = packetData.ToMP;
        }
        else
        {
            var mobGrid = _wClient.MobGrid.GetMobById(packetData.Header.ClientId);
            if (mobGrid is null) return;

            mobGrid.Status.CurHp = packetData.ToHP;
            mobGrid.Status.CurMp = packetData.ToMP;

            if (mobGrid.Status.CurHp <= 0)
            {
                Console.WriteLine($"{mobGrid.Name} died.");
            }
        }
    }


    internal void ReceiveWorld(P114_SentToWorld packetData)
    {
        Status = PlayerStatus.Create(packetData.Player);
        Gold = packetData.Player.Gold;
        Experience = packetData.Player.Exp;
        Position = packetData.WorldNetworkPosition.Adapt<Position>();
        Name = packetData.Player.Name;
        ClientId = packetData.Player.ClientId;
        ClassInfo = packetData.Player.ClassInfo;
        LearnedSkill = packetData.Player.LearnedSkill.ToList();
    }


    internal void ReceiveAffectInfo(P3B9_AffectInfo p3B9AffectInfo)
    {
        Affects = p3B9AffectInfo.Affects.Select(x => new AffectInfo
        {
            Affect = x,
            EndAt = DateTime.Now.AddSeconds(x.Time * 8)
        }).ToArray();
    }


    internal void ReceiveAddEvoks(P2454_AddEvok packet)
    {
        lock (_evoksLocker)
        {
            Evoks.Add(new Evok(packet.Owner, packet.EvokId));
        }
    }

    internal void ReceivePartyRequest(P37F_PartyRequest packet)
    {
        var hasProcessed = false;
        _wClient.Works.GetAll().ToList().ForEach(work =>
        {
            if (work.GetType() == typeof(PartyWorker))
            {
                work.Notify(new PartyWorker.PartyReceivedArgs
                {
                    LeaderId = packet.Party.ClientId,
                    LeaderName = packet.Party.Name
                });
                hasProcessed = true;
            }
            else if (work.GetType() == typeof(EventWorker))
            {
                work.Notify(new PartyWorker.PartyReceivedArgs
                {
                    LeaderId = packet.Party.ClientId,
                    LeaderName = packet.Party.Name
                });
                hasProcessed = true;
            }
        });

        if (!hasProcessed)
        {
            _wClient.PartyReceived?.Invoke(_wClient, packet.Party);
        }
    }

    internal void ReceiveAddParty(P37D_PartyJoined packet)
    {
        lock (_partyLocker)
        {
            Console.WriteLine($"New party member: {packet.Party.Name}");
            Party.RemoveAll(x => x.ClientId == packet.Party.ClientId);
            Party.Add(packet.Party);
        }

        _wClient.PartyJoined?.Invoke(_wClient, packet.Party);
    }

    internal void ReceiveRemoveParty(P37E_PartyLeft packetData)
    {
        lock (_evoksLocker)
        {
            Evoks.RemoveAll(x => x.EvokId == packetData.Index);
        }

        lock (_partyLocker)
        {
            if (packetData.Index == _wClient.Player.ClientId)
            {
                Party.Clear();
            }
            else
                Party.RemoveAll(x => x.ClientId == packetData.Index);
        }

        _wClient.PartyLeft?.Invoke(_wClient, packetData.Index);
    }

    internal void ReceiveMove(NetworkPosition to)
    {
        Position = to.Adapt<Position>();
        _wClient.Moved?.Invoke(_wClient, to.Adapt<Position>());
        Console.WriteLine($"Server moved you to {to.X} / {to.Y}");
        Console.WriteLine($"Your new position {Position.X} / {Position.Y}");
    }

    internal void ReceiveUpdateEtc(P337_UpdateScoreEtc updateEtcData)
    {
        Status.SkillPoint = updateEtcData.SkillBonus;
        Status.StatusPoint = updateEtcData.ScoreBonus;
        Status.MasterPoint = updateEtcData.SpecialBonus;
        LearnedSkill = updateEtcData.LearnedSkill.ToList();

        _wClient.ScoreUpdated?.Invoke(_wClient, Status);
    }

    #endregion

    public void ReceiveDisconnect()
    {
        ClientId = default;
        Position = new();
        Evoks.Clear();
        Party.Clear();
    }
}