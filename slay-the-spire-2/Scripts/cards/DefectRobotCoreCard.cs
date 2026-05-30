// 缺陷机器人的核心 - 羁绊牌，抽到这张牌时自动获得3费，消耗
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(StatusCardPool))]
public class DefectRobotCoreCard : SnakeTheBiteCardModel
{
    public DefectRobotCoreCard() : base(-1, CardType.Status, CardRarity.Status, TargetType.None, shouldShowInCardLibrary: false)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this) return;

        await PlayerCmd.GainEnergy(3, Owner);
        await CardCmd.Exhaust(choiceContext, this);
    }
}
