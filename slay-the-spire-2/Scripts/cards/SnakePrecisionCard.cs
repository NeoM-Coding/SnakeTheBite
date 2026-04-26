using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 蛇精要准
[Pool(typeof(IroncladCardPool))]
public class SnakePrecisionCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 2费(蓝卡)
    private const int energyCost = 2;
    // 卡牌类型 - 能力牌
    private const CardType type = CardType.Power;
    // 卡牌稀有度 - 罕见
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 保留关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    // 定义变量：能力层数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SnakePrecisionPower>(2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SnakePrecisionPower>()];

    public SnakePrecisionCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身蛇之精准能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakePrecisionPower>(Owner.Creature, DynamicVars["SnakePrecisionPower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 费用减1（2 → 1），额外层数 +1（2 → 3）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["SnakePrecisionPower"].UpgradeValueBy(1m);
    }
}
