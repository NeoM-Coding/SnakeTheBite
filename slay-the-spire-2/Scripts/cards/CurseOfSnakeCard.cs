// 蛇之诅咒 - 不可打出的诅咒牌，在手牌中时只能打出蛇标签牌
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(CurseCardPool))]
public class CurseOfSnakeCard : SnakeTheBiteCardModel
{
    private const int energyCost = -1;
    private const CardType type = CardType.Curse;
    private const CardRarity rarity = CardRarity.Curse;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Eternal];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Unplayable), HoverTipFactory.FromKeyword(CardKeyword.Eternal)];

    public CurseOfSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 当此诅咒在手牌中时，阻止非蛇标签牌的打出
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        // 只限制当前玩家的出牌
        if (card.Owner != Owner)
            return true;

        // 若此诅咒不在手牌中，不限制
        if (Pile?.Type != PileType.Hand)
            return true;

        // 只允许打出蛇标签牌
        return SnakeTheBiteCardTags.IsSnakeCard(card);
    }
}
