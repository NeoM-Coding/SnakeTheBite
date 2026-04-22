using System;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using SnakeTheBite.Scripts.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 吊蛇
[Pool(typeof(IroncladCardPool))]
public class HangSnakeCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 1费(金卡)
    private const int energyCost = 1;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 稀有
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 任意敌人
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：中毒(不升级10，升级14)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>(10m)];

    // 悬停提示：显示吊蛇能力的提示信息
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<HangSnakePower>() };

    public HangSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 造成中毒并施加吊蛇状态（翻倍中毒）
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int basePoison = DynamicVars.Poison.IntValue;
        int hangSnakeAmount = cardPlay.Target.GetPowerAmount<HangSnakePower>();
        int multiplier = Math.Max(1, hangSnakeAmount);
        int totalPoison = basePoison * multiplier;

        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");
        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, totalPoison, Owner.Creature, this);

        int num = Math.Max(2, hangSnakeAmount);
        if (hangSnakeAmount + num > 999)
        {
            num = Math.Max(0, 999 - hangSnakeAmount);
        }

        await PowerCmd.Apply<HangSnakePower>(cardPlay.Target, num, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 中毒加4
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(4m);
    }
}
