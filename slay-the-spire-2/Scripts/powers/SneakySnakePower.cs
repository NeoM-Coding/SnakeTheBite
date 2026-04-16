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

namespace MapleShadow.Scripts.Powers;

// 偷偷的蛇能力
// 本回合内，有中毒的敌人对你造成的伤害减少 Amount%
public class SneakySnakePower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    // 强制使用 description 作为 smartDescription，以便 HoverTips 中注入 Amount 变量
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 修改受到的伤害倍数（百分比减免）
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == null || !dealer.IsEnemy)
            return 1m;
        if (!dealer.HasPower<PoisonPower>())
            return 1m;

        decimal multiplier = 1m - (Amount / 100m);
        return multiplier;
    }

    // 受到伤害前闪光特效
    public override Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !dealer.IsEnemy)
            return Task.CompletedTask;
        if (!dealer.HasPower<PoisonPower>())
            return Task.CompletedTask;

        bool isPoweredAttack = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
        if (!isPoweredAttack && cardSource is not Omnislice)
            return Task.CompletedTask;

        Flash();
        return Task.CompletedTask;
    }

    // 自己的回合开始时移除该状态（确保只持续一回合）
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
            await PowerCmd.Remove(this);
    }
}
