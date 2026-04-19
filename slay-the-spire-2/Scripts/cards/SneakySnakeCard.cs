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

// 偷偷的蛇
[Pool(typeof(IroncladCardPool))]
public class SneakySnakeCard : MapleShadowCardModel
{
    // 基础耗能 - 1费(蓝卡)
    private const int energyCost = 1;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 罕见
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：伤害减免百分比(不升级25，升级50)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SneakySnakePower>(25m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<TruePoisonPower>()];

    public SneakySnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身偷偷的蛇能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SneakySnakePower>(Owner.Creature, DynamicVars["SneakySnakePower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 减免百分比加25（25 -> 50）
    protected override void OnUpgrade()
    {
        DynamicVars["SneakySnakePower"].UpgradeValueBy(25m);
    }
}
