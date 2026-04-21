// 蛇之密酿 - 罕见药水，从3张随机蛇攻击牌中选择一张加入手牌，本回合费用随机
using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MapleShadow.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace MapleShadow.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class SnakeDenseBrewPotion : MapleShadowPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);

        var snakeAttackCards = ModelDb.AllCards
            .Where(c => MapleShadowCardTags.IsSnakeCard(c)
                && c.Type == CardType.Attack
                && c.Type != CardType.Status
                && c.Type != CardType.Curse)
            .ToList();

        if (snakeAttackCards.Count == 0)
            return;

        List<CardModel> cards = CardFactory.GetDistinctForCombat(
            base.Owner,
            snakeAttackCards,
            3,
            base.Owner.RunState.Rng.CombatCardGeneration
        ).ToList();

        if (cards.Count == 0)
            return;

        CardModel selectedCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);

        if (selectedCard != null)
        {
            int cost = base.Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
            selectedCard.EnergyCost.SetThisTurn(cost);
            await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, addedByPlayer: true);
        }
    }
}
