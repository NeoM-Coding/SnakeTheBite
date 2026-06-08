// 废弃设计图 - 命运技能牌，抽到这张牌时获得1点力量与敏捷
using System.Collections.Generic;
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
public class ScrappedBlueprintCard : SnakeTheBiteCardModel
{
    public ScrappedBlueprintCard() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this)
            return;
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
