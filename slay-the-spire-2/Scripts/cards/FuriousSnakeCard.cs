// 暴蛇 - 2费红卡技能，给予自身7层中毒并进入怒蛇状态
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class FuriousSnakeCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 2费
    private const int energyCost = 2;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 罕见(蓝卡)
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：中毒层数与怒蛇层数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(7m),
        new PowerVar<FuriousSnakePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<FuriousSnakePower>()];

    public FuriousSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 给予自身中毒
        await PowerCmd.Apply<PoisonPower>(Owner.Creature, DynamicVars.Poison.BaseValue, Owner.Creature, this);

        // 退出其他蛇姿态
        if (Owner.Creature.HasPower<ChargingSnakePower>())
            await PowerCmd.Remove<ChargingSnakePower>(Owner.Creature);
        if (Owner.Creature.HasPower<DivineSnakePower>())
            await PowerCmd.Remove<DivineSnakePower>(Owner.Creature);

        // 本回合进入怒蛇状态
        await PowerCmd.Apply<FuriousSnakePower>(Owner.Creature, DynamicVars["FuriousSnakePower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 费用减1（2 → 1）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
