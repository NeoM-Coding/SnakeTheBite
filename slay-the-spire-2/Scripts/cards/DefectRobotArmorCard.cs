// 缺陷机器人的装甲 - 羁绊牌，抽到这张牌时自动打出3张飞跃+，消耗
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(StatusCardPool))]
public class DefectRobotArmorCard : SnakeTheBiteCardModel
{
    public DefectRobotArmorCard() : base(-1, CardType.Status, CardRarity.Status, TargetType.None, shouldShowInCardLibrary: false)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this) return;

        for (int i = 0; i < 3; i++)
        {
            var leap = Owner.Creature.CombatState.CreateCard<Leap>(Owner);
            CardCmd.Upgrade(leap);
            await CardCmd.AutoPlay(choiceContext, leap, Owner.Creature);
        }

        await CardCmd.Exhaust(choiceContext, this);
    }
}
