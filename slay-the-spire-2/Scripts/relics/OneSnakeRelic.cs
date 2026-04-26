// 一条蛇 - 稀有遗物，蛇咬牌数值+1；每回合第一次打出蛇咬牌获得1点能量
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SnakeTheBite.Scripts.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class OneSnakeRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：稀有（金）
    public override RelicRarity Rarity => RelicRarity.Rare;

    // 不在商店出现
    public override bool IsAllowedInShops => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 本回合是否已经打出过蛇咬牌并获得能量
    [SavedProperty]
    private bool _hasGainedEnergyThisTurn;

    // 蛇咬牌伤害+1
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null)
            return 0m;
        if (!SnakeTheBiteCardTags.IsSnakeBiteCard(cardSource))
            return 0m;
        if (dealer != Owner.Creature && cardSource.Owner != Owner)
            return 0m;

        return 1m;
    }

    // 蛇咬牌中毒+1
    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (cardSource == null)
            return amount;
        if (!SnakeTheBiteCardTags.IsSnakeBiteCard(cardSource))
            return amount;
        if (giver != Owner.Creature && cardSource.Owner != Owner)
            return amount;
        if (power is not PoisonPower and not TruePoisonPower)
            return amount;

        return amount + 1m;
    }

    // 回合开始时重置标志
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;

        _hasGainedEnergyThisTurn = false;
        return Task.CompletedTask;
    }

    // 打出蛇咬牌时，如果是本回合第一次，获得1点能量
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        if (!SnakeTheBiteCardTags.IsSnakeBiteCard(cardPlay.Card))
            return;
        if (_hasGainedEnergyThisTurn)
            return;

        _hasGainedEnergyThisTurn = true;
        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
