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
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class VenomFangRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;

        var enemies = combatState.HittableEnemies.ToList();
        if (enemies.Count == 0)
            return;

        var target = enemies[Owner.RunState.Rng.CombatTargets.NextInt(enemies.Count)];
        var roll = Owner.RunState.Rng.CombatTargets.NextInt(3);
        switch (roll)
        {
            case 0:
                await PowerCmd.Apply<PoisonPower>(target, 1, Owner.Creature, null);
                break;
            case 1:
                await PowerCmd.Apply<WeakPower>(target, 1, Owner.Creature, null);
                break;
            case 2:
                await PowerCmd.Apply<VulnerablePower>(target, 1, Owner.Creature, null);
                break;
        }
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner.Creature)
            return;
        if (power.Type != PowerType.Debuff)
            return;
        if (amount <= 0)
            return;

        var target = power.Owner;
        if (target == null || target == Owner.Creature || !target.IsAlive)
            return;

        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), new List<Creature> { target }, amount, ValueProp.Unpowered, Owner.Creature, null);
    }
}
