// 蛇！眼药水 - 0费红卡技能，自身中毒、抽满手牌、随机费用
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class SnakeEyeDropsCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度（金色 = 稀有）
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（自己）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    /// <summary>该卡牌自带"消耗"关键词。</summary>
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    /// <summary>卡牌动态变量：3层中毒。</summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(3m)
    ];

    /// <summary>悬停提示：显示中毒和消耗关键词的提示信息。</summary>
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public SnakeEyeDropsCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 打出时的效果逻辑：
    /// 1. 自身获得3层中毒；
    /// 2. 抽牌至手牌满（上限10张）；
    /// 3. 所有手牌费用随机变为 0-3。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 自身获得3层中毒
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<PoisonPower>(Owner.Creature, DynamicVars.Poison.BaseValue, Owner.Creature, this);

        // 2. 抽牌至手牌满
        int handSize = PileType.Hand.GetPile(Owner).Cards.Count;
        int drawsNeeded = 10 - handSize;
        if (drawsNeeded > 0)
        {
            await CardPileCmd.Draw(choiceContext, drawsNeeded, Owner);
        }

        // 3. 所有手牌费用随机
        foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card.EnergyCost.Canonical < 0)
                continue;

            int newCost = Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
            card.EnergyCost.SetThisCombat(newCost);
            NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        }
    }

    /// <summary>升级后的效果：获得保留关键词。</summary>
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
