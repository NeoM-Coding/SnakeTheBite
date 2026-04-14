// 蛇之神秘 - 2费无色能力，虚无，打出蛇咬累积隐秘点数获得无实体
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MapleShadow.Scripts.Powers;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class SnakeMysteryCard : MapleShadowCardModel
{
    // 基础耗能 - 2费
    private const int energyCost = 2;
    // 卡牌类型 - 能力牌
    private const CardType type = CardType.Power;
    // 卡牌稀有度 - 稀有(金卡)
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 自带虚无关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    // 悬停提示 - 显示隐秘 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<SnakeMysteryPower>() };

    public SnakeMysteryCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身隐秘能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeMysteryPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 费用减1（2 → 1）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
