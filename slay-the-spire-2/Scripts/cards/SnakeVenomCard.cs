using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Cards;

/// <summary>
/// 毒液——无色技能牌。
/// 
/// 效果：1费，给予目标5层中毒。
/// 升级后：给予目标7层中毒。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public class SnakeVenomCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度（白色 = 普通）
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    /// <summary>卡牌动态变量：5层中毒（升级后7层）。</summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(5m)
    ];

    /// <summary>悬停提示：显示中毒效果的提示信息。</summary>
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    public SnakeVenomCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 打出时的效果逻辑。
    /// 1. 播放攻击动画；
    /// 2. 在目标身上播放咬击特效；
    /// 3. 给予目标 DynamicVars.Poison 层数的中毒。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");
        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, DynamicVars.Poison.BaseValue, Owner.Creature, this);
    }

    /// <summary>升级后的效果：中毒层数 +2（5 → 7）。</summary>
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(2m);
    }
}
