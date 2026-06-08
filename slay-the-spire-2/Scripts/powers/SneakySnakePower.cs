using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

// 偷偷的蛇能力
// 有中毒或真实中毒的敌人对你造成的伤害减少固定百分比，持续 Amount 回合
public class SneakySnakePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    // 存储伤害乘数（0.75 = 减少25%，0.50 = 减少50%）
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("DamageReduction", 0.75m)];

    // 修改受到的伤害倍数（固定百分比减免）
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
            return 1m;
        if (dealer == null || !dealer.IsEnemy)
            return 1m;
        if (!dealer.HasPower<PoisonPower>() && !dealer.HasPower<TruePoisonPower>())
            return 1m;
        if (!props.IsPoweredAttack())
            return 1m;

        return DynamicVars["DamageReduction"].BaseValue;
    }

    // 受到伤害前闪光特效
    public override Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !dealer.IsEnemy)
            return Task.CompletedTask;
        if (!dealer.HasPower<PoisonPower>() && !dealer.HasPower<TruePoisonPower>())
            return Task.CompletedTask;

        bool isPoweredAttack = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
        if (!isPoweredAttack && cardSource is not Omnislice)
            return Task.CompletedTask;

        Flash();
        return Task.CompletedTask;
    }

    // 敌方回合结束时减少 1 层持续时间
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> creatures)
    {
        if (side == CombatSide.Enemy)
            await PowerCmd.TickDownDuration(this);
    }
}
