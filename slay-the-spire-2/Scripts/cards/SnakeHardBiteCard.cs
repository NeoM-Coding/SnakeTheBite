using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 蛇大力咬
[Pool(typeof(IroncladCardPool))]
public class SnakeHardBiteCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 3费(蓝卡)
    private const int energyCost = 3;
    // 卡牌类型 - 攻击牌
    private const CardType type = CardType.Attack;
    // 卡牌稀有度 - 罕见
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 任意敌人
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：中毒层数(不升级15，升级19)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>(15m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    // 悬停提示 - 显示中毒 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>()];

    public SnakeHardBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予敌人中毒
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");
        await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars.Poison.BaseValue, Owner.Creature, this, false);
    }

    // 升级后的效果逻辑 - 升级后增加4层中毒 (15 -> 19)
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(4m);
    }
}
