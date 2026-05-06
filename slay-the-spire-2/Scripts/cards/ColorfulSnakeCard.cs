// 彩蛇 - 2费稀有技能，分别将1张随机蛇标签攻击、技能、能力牌加入手牌，本回合费用随机；消耗。升级后本回合免费。
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class ColorfulSnakeCard : SnakeTheBiteCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    public ColorfulSnakeCard() : base(energyCost, type, rarity, targetType) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var snakeCards = ModelDb.AllCards
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                && c.Type != CardType.Status
                && c.Type != CardType.Curse)
            .ToList();

        var attacks = snakeCards.Where(c => c.Type == CardType.Attack).ToList();
        var skills = snakeCards.Where(c => c.Type == CardType.Skill).ToList();
        var powers = snakeCards.Where(c => c.Type == CardType.Power).ToList();

        var selectedCards = new List<CardModel>();

        if (attacks.Count > 0)
            selectedCards.Add(CardFactory.GetDistinctForCombat(Owner, attacks, 1, Owner.RunState.Rng.CombatCardGeneration).First());
        if (skills.Count > 0)
            selectedCards.Add(CardFactory.GetDistinctForCombat(Owner, skills, 1, Owner.RunState.Rng.CombatCardGeneration).First());
        if (powers.Count > 0)
            selectedCards.Add(CardFactory.GetDistinctForCombat(Owner, powers, 1, Owner.RunState.Rng.CombatCardGeneration).First());

        foreach (var card in selectedCards)
        {
            if (IsUpgraded)
            {
                card.SetToFreeThisTurn();
            }
            else
            {
                int cost = Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
                card.EnergyCost.SetThisTurn(cost);
            }
        }

        if (selectedCards.Count > 0)
        {
            await CardPileCmd.AddGeneratedCardsToCombat(selectedCards, PileType.Hand, addedByPlayer: true);
        }
    }
}
