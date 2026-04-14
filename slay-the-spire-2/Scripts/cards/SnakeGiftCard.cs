// 蛇之赠礼
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

/// <summary>
/// 蛇之赠礼 —— 无色能力金卡。
/// 
/// 效果：3费，战斗结束时随机升级一张非蛇的卡牌。
/// 升级后费用减1（3 → 2）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public class SnakeGiftCard : MapleShadowCardModel
{
    // 基础耗能 - 3费
    private const int energyCost = 3;
    // 卡牌类型 - 能力牌
    private const CardType type = CardType.Power;
    // 卡牌稀有度 - 稀有(金卡)
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 悬停提示 - 显示蛇之赠礼 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<SnakeGiftPower>() };

    public SnakeGiftCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身蛇之赠礼能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeGiftPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 费用减1（3 → 2）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
