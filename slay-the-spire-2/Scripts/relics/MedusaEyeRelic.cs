// 美杜莎之眼 - 事件遗物
// 每当你对敌人给予一次中毒时，同时给予一层虚弱
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class MedusaEyeRelic : SnakeTheBiteRelicModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<TruePoisonPower>(), HoverTipFactory.FromPower<WeakPower>()];

    // 遗物稀有度：事件
    public override RelicRarity Rarity => RelicRarity.Event;

    // 当任意 Power 层数变化后触发：若自己施加的中毒增加，则目标额外获得一层虚弱
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner.Creature)
            return;
        if (amount <= 0)
            return;
        if (power is not PoisonPower and not TruePoisonPower)
            return;
        if (!power.Owner.IsEnemy)
            return;

        Flash();
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), power.Owner, 1m, Owner.Creature, cardSource, false);
    }
}
