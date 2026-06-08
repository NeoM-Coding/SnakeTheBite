// 枯木 - 所有攻击/技能牌获得消耗且费用-1，消耗时从本职业卡池随机加牌到手牌
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class WitheredWoodRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Type == CardType.Attack || card.Type == CardType.Skill)
        {
            modifiedCost = System.Math.Max(0, originalCost - 1);
            return true;
        }
        modifiedCost = originalCost;
        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if ((card.Type == CardType.Attack || card.Type == CardType.Skill) && !card.Keywords.Contains(CardKeyword.Exhaust))
        {
            Flash();
            await CardCmd.Exhaust(context, card, causedByEthereal: false);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        var poolCards = Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);
        var randomCard = CardFactory.GetDistinctForCombat(Owner, poolCards, 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (randomCard != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(randomCard, PileType.Hand, Owner, CardPilePosition.Random);
        }
    }
}
