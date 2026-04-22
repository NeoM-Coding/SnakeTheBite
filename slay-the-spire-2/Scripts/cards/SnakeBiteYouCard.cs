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

// 蛇咬着你
[Pool(typeof(ColorlessCardPool))]
public class SnakeBiteYouCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 1费(蓝卡)
    private const int energyCost = 0;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 罕见（蓝色）
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 任意队友
    private const TargetType targetType = TargetType.AnyAlly;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 仅多人模式可用
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 定义变量：中毒层数(不升级7，升级9)，抽牌数2，获得能量1
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(7m),
        new CardsVar(2),
        new EnergyVar(1)
    ];

    // 悬停提示 - 显示中毒与能量的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), base.EnergyHoverTip];

    public SnakeBiteYouCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予目标队友中毒，自己抽牌并获得能量
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<PoisonPower>(cardPlay.Target, DynamicVars.Poison.BaseValue, Owner.Creature, this);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].IntValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    // 升级后的效果逻辑 - 中毒层数加2（7 -> 9），能量加1（1 -> 2）
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(2m);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
