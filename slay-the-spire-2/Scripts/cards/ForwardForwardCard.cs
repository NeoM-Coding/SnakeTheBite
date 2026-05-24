// 前进！前进！ - 命运技能牌，获得2能量3辉星抽3张牌，选择攻击牌变为冲刺并打出三次
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
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class ForwardForwardCard : SnakeTheBiteCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new StarsVar(3),
        new CardsVar(3)
    ];

    public ForwardForwardCard() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(2, Owner);
        await PlayerCmd.GainStars(3m, Owner);
        await CardPileCmd.Draw(choiceContext, 3, Owner);

        var attacks = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Attack)
            .ToList();

        if (attacks.Count == 0)
            return;

        var selected = await CardSelectCmd.FromHand(
            choiceContext, Owner,
            new CardSelectorPrefs(new LocString("card_selection", "TO_TRANSFORM"), 1),
            c => c.Type == CardType.Attack,
            this
        );

        var target = selected.FirstOrDefault();
        if (target == null)
            return;

        var sprint = target.CardScope!.CreateCard<SprintCard>(Owner);
        await CardCmd.Transform(target, sprint);

        // 将冲刺打出三次
        for (int i = 0; i < 3; i++)
        {
            var enemy = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).FirstOrDefault(c => c.IsAlive);
            await CardCmd.AutoPlay(choiceContext, sprint, enemy);
        }
    }
}
