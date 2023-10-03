using System.Linq.Expressions;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Models.Models;
using WYD.Frame.Packets;
using WYD.Frame.Services;
using WYD.Frame.Services.Models.Resource;

namespace WYD.Frame.Game.Workers;

public class SkillWorker : WorkerBase
{
    private List<SkillDelay>? _attacks = null;
    private List<SkillInfo>? _buffs = null;
    private Mob? _currentTarget;
    private short[] _ignoreMerchants = new short[] { 36, 68, 4, 3 };
    private int currentSkillIndex = 0;
    private int currentBuffIndex = 0;

    public SkillWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }

    public List<GameSkills> Skills { get; set; } = new();

    public override void Notify(object? args)
    {
        if (_buffs is null) return;

        var killedId = (ushort?)args;
        if (killedId is null) return;

        if (WClientOwner.Player.Dead) return;
        
        if (WClientOwner.Player.Party.All(z => z.ClientId != killedId))
            return;

        var reviveBuff = _buffs.FindIndex(x => x.Id == GameSkills.Renascimento);
        if (reviveBuff == -1) return;

        var mobFromGrid = WClientOwner.MobGrid.GetMobById(killedId.Value);

        if (mobFromGrid is null)
        {
            WClientOwner.Log(MessageRelevance.Medium, $"Your party member died, but I wasn't able to find it on mob grid.");
            return;
        }

        var previousPosition = WClientOwner.Player.Position.Clone();
        bool moved = false;
        if (mobFromGrid.Position.GetDistance(WClientOwner.Player.Position) > _buffs[reviveBuff].Range)
        {
            WClientOwner.Log(MessageRelevance.Medium, $"Moving to dead body at {mobFromGrid.Position.X} / {mobFromGrid.Position.Y}.");

            var moveWorker = new MoveWorker(WClientOwner)
            {
                SyncRun = true,
                Destination = mobFromGrid.Position.Clone(),
                Id = Guid.NewGuid().ToString()
            };
            moveWorker.Start();
            moved = true;
        }
        
        WClientOwner.Log(MessageRelevance.Medium, $"Trying to revive {mobFromGrid.Name}");

        WClientOwner.Player.SendSingleAttack(GameSkills.Renascimento, killedId.Value);
        WClientOwner.Timer.Sleep(1000);
        if (moved)
        {
            WClientOwner.Log(MessageRelevance.Medium, $"Moving back.");

            var moveWorker = new MoveWorker(WClientOwner)
            {
                SyncRun = true,
                Destination = previousPosition,
                Id = Guid.NewGuid().ToString()
            };
            moveWorker.Start();
        }

        base.Notify(args);
    }

    protected override void DoWork()
    {
        if (_buffs == null)
            _buffs = ResourceService.Skills.Where(x => Skills.Contains(x.Id) && !x.Passive && !x.Aggressive).ToList();

        if (_attacks == null)
        {
            _attacks = ResourceService.Skills.Where(x => Skills.Contains(x.Id) && x.Aggressive).Select(x =>
                new SkillDelay
                {
                    Skill = x,
                    Delay = (int)x.Delay
                }).ToList();
            _attacks.ForEach(x =>
            {
                var sanc = WClientOwner.Bag.Equips[4].Item.Ef.FirstOrDefault(x => x.Type == GameEfv.EF_SANC);

                if (sanc.Value < 9) return;

                x.Delay--;
                Console.WriteLine($"Reducing delay for {x.Skill}. Current: {x.Delay}, Old: {x.Skill.Delay}");
            });
        }

        Buff();
        Attack();
    }

    private void Attack()
    {
        if (WClientOwner.Player.IsOnCity)
        {
            Thread.Sleep(1000);
            return;
        }

        if (WClientOwner.Player.Moving)
        {
            Thread.Sleep(1000);
            return;
        }

        if (_attacks is null || _attacks.Count == 0)
        {
            return;
        }

        if (currentSkillIndex >= _attacks.Count) currentSkillIndex = 0;
        var skillDelay = _attacks[currentSkillIndex];
        if (!SelectTarget(skillDelay.Skill)) return;

        if (_currentTarget is null) return;
        if (skillDelay.Skill.MaxTarget > 1)
        {
            var mobs = WClientOwner.MobGrid.Entities.Where(IsValidForAttack)
                .Where(x => x.Position.GetDistance(_currentTarget.Position) <=
                            skillDelay.Skill.Range)
                .OrderBy(x => x.Position.GetDistance(_currentTarget.Position))
                .Select(x => x.ClientId)
                .Take(13)
                .ToArray();

            var targets = new ushort[13];
            mobs.CopyTo(targets, 0);
            if (State == WorkState.Paused) return;
            WClientOwner.Player.SendMultiAttack(skillDelay.Skill.Id, _currentTarget.ClientId, targets);
            WClientOwner.Timer.Sleep(1000);
            currentSkillIndex++;
            return;
        }


        if (State == WorkState.Paused) return;
        WClientOwner.Player.SendSingleAttack(skillDelay.Skill.Id, _currentTarget.ClientId);
        WClientOwner.Timer.Sleep(1000);
        currentSkillIndex++;
    }

    private bool IsValidForAttack(Mob mob)
    {
        return mob is { ClientId: > 1000, Status: { Race: 0, CurHp: > 0 } } &&
               !mob.Name.EndsWith("^");
    }

    private bool SelectTarget(SkillInfo skill)
    {
        if (_currentTarget != null && WClientOwner.MobGrid.ContainsMob(_currentTarget.ClientId) &&
            _currentTarget.Position.GetDistance(WClientOwner.Player.Position) <= skill.Range) return true;

        var elegibleMobs = WClientOwner.MobGrid.Entities
            .Where(IsValidForAttack)
            .Where(x => x.Position.GetDistance(WClientOwner.Player.Position) <= skill.Range)
            .OrderBy(x => x.Position.GetDistance(WClientOwner.Player.Position)).ToArray();

        if (!elegibleMobs.Any())
            return false;

        _currentTarget = elegibleMobs.First();

        return true;
    }

//m_sHeadIndex = equips[0].sIndex;
    // int TMHuman::IsMerchant()
    // {
    //     if ((m_stScore.Reserved & 0xF) >= 1 && (m_stScore.Reserved & 0xF) <= 14)
    //         return 1;
    //
    //     if (m_sHeadIndex == 54 || m_sHeadIndex == 55 || m_sHeadIndex == 56 || m_sHeadIndex == 57 || m_sHeadIndex == 51 || m_sHeadIndex == 68 || m_sHeadIndex == 67)
    //         return 1;
    //
    //     return 0;
    // }
    private void Buff()
    {
        var currentDate = DateTime.Now.AddSeconds(30);

        if (_buffs is null) return;

        foreach (var buff in _buffs)
        {
            if (buff.AffectType != 0)
            {
                var currentAffects = WClientOwner.Player.Affects.FirstOrDefault(x => x.Affect.Type == buff.AffectType);

                if (currentAffects != null && currentDate < currentAffects.EndAt)
                    continue;
            }

            if (buff is { AffectType: 0, bParty: 0 }) //prob evok
                if (WClientOwner.Player.Evoks.Count(x => x.Owner == WClientOwner.Player.ClientId) > 0)
                    continue;


            if (buff.MaxTarget > 1)
            {
                var partyIds = WClientOwner.Player.Party.Select(x => x.ClientId);
                var clientPosition = WClientOwner.Player.Position.Clone();
                var mobs = WClientOwner.MobGrid.Entities.Where(x => partyIds.Contains(x.ClientId))
                    .Where(x => x.Position.GetDistance(clientPosition) <= buff.Range)
                    .OrderBy(x => x.Position.GetDistance(clientPosition))
                    .Select(x => x.ClientId)
                    .Take(12)
                    .ToArray();

                var targets = new ushort[13];
                targets[0] = WClientOwner.Player.ClientId;
                mobs.CopyTo(targets, 1);
                WClientOwner.Log(MessageRelevance.Medium, $"Trying to area buff with {buff.Name} / {buff.Id}");
                WClientOwner.Player.SendMultiAttack(buff.Id, WClientOwner.Player.ClientId, targets);
                WClientOwner.Timer.Sleep(1000);
                return;
            }

            if (buff.Id == GameSkills.Renascimento)
            {
                Thread.Sleep(1000);
                continue;
            }
            
            WClientOwner.Log(MessageRelevance.Medium, $"Trying to buff with {buff.Name} / {buff.Id}");
            WClientOwner.Player.SendSingleAttack(buff.Id, WClientOwner.Player.ClientId);
            WClientOwner.Timer.Sleep(1000);
        }
    }

    private class SkillDelay
    {
        public SkillInfo Skill { get; set; }
        public int Delay { get; set; }
    }
}