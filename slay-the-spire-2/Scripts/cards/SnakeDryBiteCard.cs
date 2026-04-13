using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Cards;

/// <summary>
/// 蛇干咬——战士（红色）攻击牌。
/// 
/// 效果：1费，造成7点伤害。
/// 升级后：造成10点伤害。
/// </summary>
[Pool(typeof(IroncladCardPool))]
public class SnakeDryBiteCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度（白色 = 普通）
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    /// <summary>卡牌动态变量：7点伤害（升级后10点）。</summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move)
    ];

    public SnakeDryBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 打出时的效果逻辑：对目标造成 DynamicVars.Damage 点伤害。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    /// <summary>升级后的效果：伤害 +3（7 → 10）。</summary>
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
