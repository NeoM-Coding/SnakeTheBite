// 怒蛇能力 - 蛇之姿态。蛇牌额外打出，受未被格挡伤害时获得中毒。
using BaseLib.Abstracts;
using SnakeTheBite.Scripts.Utils;
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

namespace SnakeTheBite.Scripts.Powers;

public class FuriousSnakePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    //
    // 修改符合条件的卡牌的打出次数，使其额外打出 Amount 次。
    //
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != base.Owner)
            return playCount;

        if (!SnakeTheBiteCardTags.IsSnakeCard(card))
            return playCount;

        return playCount + (int)Amount;
    }

    //
    // 成功修改卡牌打出次数后触发：播放闪光特效。
    //
    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }

    //
    // 受到敌人攻击前触发：若存在未被格挡的伤害，则给予自身等量×Amount 的中毒。
    // 过滤条件同荆棘类实现，排除非攻击/非敌人来源。
    //
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
            await PowerCmd.Apply<PoisonPower>(target, unblockedDamage * Amount, target, null);
        }
    }
}
