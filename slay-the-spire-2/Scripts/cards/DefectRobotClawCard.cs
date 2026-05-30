// 缺陷机器人的爪子 - 羁绊牌，抽到这张牌时对所有敌人打出3张狂乱撕扯+，消耗
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
using MegaCrit.Sts2.Core.Models.Cards;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(StatusCardPool))]
public class DefectRobotClawCard : SnakeTheBiteCardModel
{
    public DefectRobotClawCard() : base(-1, CardType.Status, CardRarity.Status, TargetType.None, shouldShowInCardLibrary: false)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this) return;

        var enemies = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Where(c => c.IsAlive).ToList();
        if (enemies == null || enemies.Count == 0) return;

        for (int i = 0; i < 3; i++)
        {
            var target = enemies[Owner.RunState.Rng.CombatTargets.NextInt(enemies.Count)];
            var claw = Owner.Creature.CombatState.CreateCard<Claw>(Owner);
            CardCmd.Upgrade(claw);
            await CardCmd.AutoPlay(choiceContext, claw, target);
        }

        await CardCmd.Exhaust(choiceContext, this);
    }
}
