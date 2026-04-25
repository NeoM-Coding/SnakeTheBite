// 荆棘蛇咬 - 3费无色能力，敌人每攻击一次被蛇咬（受到7层中毒）
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class SnakeBiteThornsCard : SnakeTheBiteCardModel
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

    //
    // 卡牌动态变量：荆棘蛇咬能力层数为 1。
    // 在本地化中可用 {SnakeBiteThornsPower} 引用。
    //
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SnakeBiteThornsPower>(1m)
    ];

    // 悬停提示：显示荆棘蛇咬能力的提示信息。
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<SnakeBiteThornsPower>() };

    public SnakeBiteThornsCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    //
    // 打出时的效果逻辑：给自己施加 1 层荆棘蛇咬能力。
    //
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeBiteThornsPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["SnakeBiteThornsPower"].BaseValue, Owner.Creature, this, false);
    }

    // 升级后的效果：费用减1（3 → 2）。
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
