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

// 蛇行
[Pool(typeof(IroncladCardPool))]
public class SnakeWalkCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 1费(白卡)
    private const int energyCost = 1;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 普通
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：抽牌数固定4张，中毒层数基础3/升级后2
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Draw", 4m),
        new PowerVar<PoisonPower>(3m)
    ];

    // 悬停提示 - 显示中毒 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>()];

    public SnakeWalkCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 抽牌并给予自身中毒
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
        await PowerCmd.Apply<PoisonPower>(Owner.Creature, DynamicVars.Poison.BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 中毒层数减少1（3 -> 2）
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(-1m);
    }
}
