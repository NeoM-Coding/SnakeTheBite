// 蛇不咬 - 2费诅咒牌，保留、永恒
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(CurseCardPool))]
public class SnakeNoBiteCard : MapleShadowCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Curse;
    private const CardRarity rarity = CardRarity.Curse;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Eternal];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain), HoverTipFactory.FromKeyword(CardKeyword.Eternal)];

    public SnakeNoBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
