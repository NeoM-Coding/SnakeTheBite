using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 弹跳蛇瓶
[Pool(typeof(IroncladCardPool))]
public class BouncingSnakeBottleCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 3费(金卡)
    private const int energyCost = 3;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 稀有
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 所有敌人（实际随机选取）
    private const TargetType targetType = TargetType.AllEnemies;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 基础命中次数
    private const int baseHitCount = 3;

    // 定义变量：中毒层数(不升级6)，命中次数(不升级3)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(6m),
        new RepeatVar(baseHitCount)
    ];

    // 悬停提示 - 显示中毒 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>()];

    public BouncingSnakeBottleCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 对随机多个敌人给予中毒
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var enemies = CombatState?.HittableEnemies;
        if (enemies == null || enemies.Count == 0)
            return;

        int hitCount = DynamicVars.Repeat.IntValue;
        int poisonAmount = DynamicVars.Poison.IntValue;

        for (int i = 0; i < hitCount; i++)
        {
            Creature? enemy = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
            if (enemy == null)
                continue;

            VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_bite");
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), enemy, poisonAmount, Owner.Creature, this, false);
        }
    }

    // 升级后的效果逻辑 - 命中次数加1（3 -> 4）
    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}
