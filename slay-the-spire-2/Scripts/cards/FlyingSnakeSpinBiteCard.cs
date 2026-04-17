// 飞蛇回旋咬 - 1费红卡攻击，随机3个敌人分别给予2层中毒
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

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class FlyingSnakeSpinBiteCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人，实际效果在OnPlay中处理随机目标）
    private const TargetType targetType = TargetType.AllEnemies;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    //
    // 卡牌动态变量：2层中毒（升级后3层）。
    // 在本地化描述中可用 {PoisonPower:diff()} 引用。
    //
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(2m)
    ];

    // 悬停提示：显示中毒效果的提示信息。
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    public FlyingSnakeSpinBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    //
    // 打出时的效果逻辑。
    //
    // 流程：
    // 1. 播放攻击动画；
    // 2. 获取当前所有可攻击敌人；
    // 3. 随机选取3名敌人（若敌人少于3名，允许重复命中同一敌人），
    // 每次命中播放咬击特效并施加 DynamicVars.Poison 层数的中毒。
    //
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放攻击动画
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        // 获取可攻击敌人列表
        var enemies = CombatState?.HittableEnemies;
        if (enemies == null || enemies.Count == 0)
            return;

        int hitCount = 3;
        int poisonAmount = DynamicVars.Poison.IntValue;

        for (int i = 0; i < hitCount; i++)
        {
            Creature? enemy = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
            if (enemy == null)
                continue;

            // 播放咬击特效
            VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_bite");

            // 施加中毒
            await PowerCmd.Apply<PoisonPower>(enemy, poisonAmount, Owner.Creature, this);
        }
    }

    // 升级后的效果：中毒层数 +1（2 → 3）。
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(1m);
    }
}
