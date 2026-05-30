// 蛇之秘酿 - 罕见药水，从3张随机蛇能力牌中选择一张加入手牌，本回合费用随机
using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace SnakeTheBite.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class SnakeSecretBrewPotion : SnakeTheBitePotionModel
{
    // 稀有度 - 罕见（蓝）
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    // 使用方式 - 仅在战斗中使用
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型 - 自己（不能给队友）
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 使用时的效果逻辑：从三张随机蛇能力牌中选择一张，本回合费用随机后加入手牌。
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);

        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, target.Player!);

        // 从所有卡牌中筛选出蛇能力牌
        var snakeCards = ModelDb.AllCards
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c) && c.Type == CardType.Power)
            .ToList();

        if (snakeCards.Count == 0)
            return;

        // 随机选取3张不同的蛇牌用于战斗
        List<CardModel> cards = CardFactory.GetDistinctForCombat(
            base.Owner,
            snakeCards,
            3,
            base.Owner.RunState.Rng.CombatCardGeneration
        ).ToList();

        if (cards.Count == 0)
            return;

        // 弹出选择屏幕让玩家选一张
        CardModel selectedCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);

        if (selectedCard != null)
        {
            // 本回合费用随机（0-3）
            int cost = base.Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
            selectedCard.EnergyCost.SetThisTurn(cost);

            await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, Owner);
        }
    }


}
