// 数据库 - 命运技能牌，从抽牌堆和弃牌堆各抽2张牌，费用之和给予集中
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class DatabaseCard : SnakeTheBiteCardModel
{
    public DatabaseCard() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energySum = 0;

        // 从抽牌堆抽2张
        var drawn = await CardPileCmd.Draw(choiceContext, 2, Owner);
        foreach (var card in drawn)
        {
            energySum += card.EnergyCost.GetResolved();
        }

        // 从弃牌堆取2张
        var discardPile = PileType.Discard.GetPile(Owner);
        var fromDiscard = discardPile.Cards.Take(2).ToList();
        if (fromDiscard.Count > 0)
        {
            await CardPileCmd.Add(fromDiscard, PileType.Hand);
            foreach (var card in fromDiscard)
            {
                energySum += card.EnergyCost.GetResolved();
            }
        }

        // 给予集中
        if (energySum > 0)
        {
            await PowerCmd.Apply<FocusPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, energySum, Owner.Creature, this, false);
        }
    }
}
