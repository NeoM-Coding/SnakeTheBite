using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MapleShadow.Scripts.Powers;

namespace MapleShadow.Scripts.Cards;

/// <summary>
/// 荆棘蛇咬——无色能力牌。
/// 
/// 效果：3费，获得荆棘蛇咬能力：敌人每攻击一次，就会被蛇咬一次（受到1层中毒）。
/// 升级后：费用减1（3 → 2）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public class SnakeBiteThornsCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 3;
    // 卡牌类型
    private const CardType type = CardType.Power;
    // 卡牌稀有度（金色 = 稀有）
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（自己）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 卡牌动态变量：荆棘蛇咬能力层数为 1。
    /// 在本地化中可用 {SnakeBiteThornsPower} 引用。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SnakeBiteThornsPower>(1m)
    ];

    /// <summary>悬停提示：显示荆棘蛇咬能力的提示信息。</summary>
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<SnakeBiteThornsPower>() };

    public SnakeBiteThornsCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 打出时的效果逻辑：给自己施加 1 层荆棘蛇咬能力。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeBiteThornsPower>(Owner.Creature, DynamicVars["SnakeBiteThornsPower"].BaseValue, Owner.Creature, this);
    }

    /// <summary>升级后的效果：费用减1（3 → 2）。</summary>
    protected override void OnUpgrade()
    {
        if (DynamicVars.TryGetValue("Cost", out DynamicVar? costVar))
        {
            costVar.BaseValue = 2;
        }
    }
}
