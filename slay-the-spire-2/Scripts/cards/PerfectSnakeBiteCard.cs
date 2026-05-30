// 完美蛇咬 - 2费红卡攻击，给予5层中毒，牌库每有一张蛇咬额外+4层中毒
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Linq;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class PerfectSnakeBiteCard : SnakeTheBiteCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    // 保留关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Retain };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(5m),
        new CalculationExtraVar(4m),
        new CalculatedVar("PerfectSnakeBitePoison").WithMultiplier((card, target) =>
        {
            var allCards = card.Owner.PlayerCombatState.AllCards;
            return card.IsUpgraded
                ? allCards.Count(c => SnakeTheBiteCardTags.IsSnakeCard(c))
                : allCards.Count(c => SnakeTheBiteCardTags.IsSnakeBiteCard(c));
        }),
        new PowerVar<PoisonPower>(0m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    public PerfectSnakeBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // CalculatedVar 的 IntValue 只返回 BaseValue（5），必须调用 Calculate 才能拿到计算后的总层数
        var calculatedPoison = (CalculatedVar)DynamicVars["PerfectSnakeBitePoison"];
        int poisonAmount = (int)calculatedPoison.Calculate(cardPlay.Target);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");
        await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, poisonAmount, Owner.Creature, this, false);
    }

    protected override void OnUpgrade()
    {
        // 升级后判定条件从 Snakebite 变为 Snake，数值不变
    }
}
