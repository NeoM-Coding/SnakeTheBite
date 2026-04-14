// 蛇之舞 - 1费红卡技能，获得7格挡，下回合获得1点能量
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class SnakeDanceCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度（蓝色 = 罕见）
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    /// <summary>标记该卡牌会产生格挡，用于UI显示。</summary>
    public override bool GainsBlock => true;

    /// <summary>
    /// 卡牌动态变量：7点格挡 + 1点下回合能量。
    /// 升级后分别为9点格挡和2点能量。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new EnergyVar(1)
    ];

    /// <summary>额外的悬停提示：显示能量相关的提示信息。</summary>
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { EnergyHoverTip };

    public SnakeDanceCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 打出时的效果逻辑。
    /// 1. 获得 BlockVar 数值的格挡；
    /// 2. 给自己施加 EnergyNextTurnPower，使下回合开始时获得 EnergyVar 数值的能量。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<EnergyNextTurnPower>(Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
    }

    /// <summary>升级后的效果：格挡 +2，下回合能量 +1。</summary>
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);  // 7 -> 9
        DynamicVars.Energy.UpgradeValueBy(1m); // 1 -> 2
    }
}
