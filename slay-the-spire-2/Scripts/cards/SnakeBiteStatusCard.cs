// 蛇噬 - 状态牌，抽到获得2层中毒
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 状态牌加入状态卡池
[Pool(typeof(StatusCardPool))]
public class SnakeBiteStatusCard : SnakeTheBiteCardModel
{
    // 不可打出（费用-1），状态牌
    private const int energyCost = -1;
    private const CardType type = CardType.Status;
    private const CardRarity rarity = CardRarity.Status;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = false;

    // 中毒层数
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        new[] { new PowerVar<PoisonPower>(2m) };

    // 不可打出、虚无（回合结束丢弃）
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        new[] { CardKeyword.Unplayable, CardKeyword.Ethereal };

    // 不可升级
    public override int MaxUpgradeLevel => 0;

    // 悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    public SnakeBiteStatusCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 当抽到此牌时触发
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this)
            return;

        // 自身获得2层中毒
        int poisonAmount = DynamicVars.Poison.IntValue;
        await Cmd.Wait(0.25f);
        await PowerCmd.Apply<PoisonPower>(Owner.Creature, poisonAmount, Owner.Creature, this);
    }
}
