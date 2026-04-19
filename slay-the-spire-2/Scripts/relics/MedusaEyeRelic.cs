// 美杜莎之眼 - 事件遗物
// 每当你对敌人给予一次中毒时，同时给予一层虚弱
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MapleShadow.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class MedusaEyeRelic : MapleShadowRelicModel
{
    // 遗物稀有度：事件
    public override RelicRarity Rarity => RelicRarity.Event;

    // 当任意 Power 层数变化后触发：若自己施加的中毒增加，则目标额外获得一层虚弱
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner.Creature)
            return;
        if (amount <= 0)
            return;
        if (power is not PoisonPower)
            return;
        if (!power.Owner.IsEnemy)
            return;

        Flash();
        await PowerCmd.Apply<WeakPower>(power.Owner, 1m, Owner.Creature, cardSource);
    }
}
