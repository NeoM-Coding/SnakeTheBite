// 点燃星海 - 命运攻击牌，0费3星，对所有敌人造成21点伤害，可将星星牌的辉星转为额外伤害
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class IgniteStarSeaCard : SnakeTheBiteCardModel
{
    public IgniteStarSeaCard() : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
    {
    }

    public override int CanonicalStarCost => 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(21m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal totalDamage = DynamicVars.Damage.BaseValue;

        var starCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.CanonicalStarCost >= 0)
            .ToList();

        if (starCards.Count > 0)
        {
            var selected = await CardSelectCmd.FromHand(
                choiceContext, Owner,
                new CardSelectorPrefs(new LocString("card_selection", "TO_SELECT"), 1),
                c => c.CanonicalStarCost >= 0,
                this
            );

            var targetCard = selected.FirstOrDefault();
            if (targetCard != null)
            {
                int savedStars = targetCard.CurrentStarCost;
                totalDamage += savedStars * 3;
                targetCard.SetStarCostThisCombat(0);
            }
        }

        await DamageCmd.Attack(totalDamage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
            .Execute(choiceContext);
    }
}
