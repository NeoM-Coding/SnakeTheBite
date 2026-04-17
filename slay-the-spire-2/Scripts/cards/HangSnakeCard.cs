using System;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MapleShadow.Scripts.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Cards;

// 吊蛇
[Pool(typeof(IroncladCardPool))]
public class HangSnakeCard : MapleShadowCardModel
{
    // 基础耗能 - 1费(金卡)
    private const int energyCost = 1;
    // 卡牌类型 - 攻击牌
    private const CardType type = CardType.Attack;
    // 卡牌稀有度 - 稀有
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 任意敌人
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：伤害(不升级10，升级14)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move)];

    public HangSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 造成伤害并施加吊蛇状态
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        int powerAmount = cardPlay.Target.GetPowerAmount<HangSnakePower>();
        int num = Math.Max(2, powerAmount);
        if (powerAmount + num > 999)
        {
            num = Math.Max(0, 999 - powerAmount);
        }

        await PowerCmd.Apply<HangSnakePower>(cardPlay.Target, num, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 伤害加4
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
