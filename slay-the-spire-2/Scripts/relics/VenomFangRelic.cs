// 毒牙 - 施加负面效果时造成等量伤害，回合开始随机施加1层负面
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Powers.Mocks;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class VenomFangRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool _isProcessing;

    private static List<PowerModel>? _debuffPool;

    private static List<PowerModel> GetDebuffPool()
    {
        if (_debuffPool == null)
        {
            _debuffPool = new List<PowerModel>();

            // 原版白名单（用户指定）
            var vanillaWhitelist = new List<PowerModel>
            {
                ModelDb.Power<ConquerorPower>(),
                ModelDb.Power<ConstrictPower>(),
                ModelDb.Power<CrushUnderPower>(),
                ModelDb.Power<DarkShacklesPower>(),
                ModelDb.Power<DebilitatePower>(),
                ModelDb.Power<DisintegrationPower>(),
                ModelDb.Power<DoomPower>(),
                ModelDb.Power<DyingStarPower>(),
                ModelDb.Power<EnfeeblingTouchPower>(),
                ModelDb.Power<FlankingPower>(),
                ModelDb.Power<HangPower>(),
                ModelDb.Power<KnockdownPower>(),
                ModelDb.Power<ManglePower>(),
                ModelDb.Power<MonarchsGazeStrengthDownPower>(),
                ModelDb.Power<NeurosurgePower>(),
                ModelDb.Power<NoBlockPower>(),
                ModelDb.Power<OblivionPower>(),
                ModelDb.Power<PiercingWailPower>(),
                ModelDb.Power<PlowPower>(),
                ModelDb.Power<PoisonPower>(),
                ModelDb.Power<ShacklingPotionPower>(),
                ModelDb.Power<ShriekPower>(),
                ModelDb.Power<ShrinkPower>(),
                ModelDb.Power<SlowPower>(),
                ModelDb.Power<StranglePower>(),
                ModelDb.Power<TheGambitPower>(),
                ModelDb.Power<VulnerablePower>(),
                ModelDb.Power<WeakPower>(),

            };
            _debuffPool.AddRange(vanillaWhitelist);

            // Mod 白名单（全部 Debuff）
            var modDebuffs = new List<PowerModel>
            {
                ModelDb.Power<HangSnakePower>(),
                ModelDb.Power<RhythmPower>(),
                ModelDb.Power<SnakeBlastPower>(),
                ModelDb.Power<SnakeVenomTherapyPendingPower>(),
                ModelDb.Power<TraumaPower>(),
                ModelDb.Power<TruePoisonPower>(),
            };

            foreach (var debuff in modDebuffs)
            {
                if (!_debuffPool.Contains(debuff))
                    _debuffPool.Add(debuff);
            }
        }
        return _debuffPool;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> creatures, ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;

        var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0)
            return;

        Flash();

        var target = enemies[Owner.RunState.Rng.CombatTargets.NextInt(enemies.Count)];
        var pool = GetDebuffPool();
        if (pool.Count == 0)
            return;

        var debuff = pool[Owner.RunState.Rng.CombatTargets.NextInt(pool.Count)];
        await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), debuff.ToMutable(), target, 1m, Owner.Creature, null, false);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_isProcessing)
            return;
        if (applier != Owner.Creature)
            return;
        if (power.Type != PowerType.Debuff)
            return;
        if (amount <= 0)
            return;

        var target = power.Owner;
        if (target == null || target == Owner.Creature || !target.IsAlive)
            return;

        _isProcessing = true;
        try
        {
            Flash();
            await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), new List<Creature> { target }, amount, ValueProp.Unpowered, Owner.Creature, null);
        }
        finally
        {
            _isProcessing = false;
        }
    }
}
