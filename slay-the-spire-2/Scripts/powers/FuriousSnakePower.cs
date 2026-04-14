using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Powers;

/// <summary>
/// 怒蛇 —— 本回合内生效的 Buff 状态。
/// 
/// 效果：
/// 1. 打出的类名含 "Snake" 的卡牌会额外打出一次（参考 BurstPower / EchoFormPower）。
/// 2. 受到来自敌人的 Powered Attack 且存在未被格挡的伤害时，给予自身等量的中毒。
/// </summary>
public class FuriousSnakePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.None;

    public override int DisplayAmount => 0;

    /// <summary>
    /// 修改符合条件的卡牌的打出次数，使其额外打出一次。
    /// </summary>
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner)
            return playCount;

        string typeName = card.GetType().Name;
        if (!typeName.Contains("Snake", StringComparison.OrdinalIgnoreCase))
            return playCount;

        return playCount + 1;
    }

    /// <summary>
    /// 成功修改卡牌打出次数后触发：播放闪光特效。
    /// </summary>
    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 自己的回合开始时移除该状态，确保敌人的攻击阶段仍然生效。
    /// </summary>
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }

    /// <summary>
    /// 受到敌人攻击前触发：若存在未被格挡的伤害，则给予自身等量中毒。
    /// 过滤条件同荆棘类实现，排除非攻击/非敌人来源。
    /// </summary>
    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || dealer == null || !dealer.IsEnemy)
            return;

        bool isPoweredAttack = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
        if (!isPoweredAttack && cardSource is not Omnislice)
            return;

        int unblockedDamage = Math.Max(0, (int)amount - target.Block);
        if (unblockedDamage > 0)
        {
            Flash();
            await PowerCmd.Apply<PoisonPower>(target, unblockedDamage, target, null);
        }
    }
}
