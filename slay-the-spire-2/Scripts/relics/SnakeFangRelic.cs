// 异蛇之牙 - 普通遗物，给敌人上毒时随机打1
using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeFangRelic : MapleShadowRelicModel
{
    /// <summary>遗物稀有度：普通（白）。</summary>
    public override RelicRarity Rarity => RelicRarity.Common;

    /// <summary>
    /// 定义遗物的核心动态变量。
    /// 此处使用 DamageVar(1, Unpowered)，在本地化文本中可用 {Damage} 引用该数值。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Unpowered)];

    /// <summary>
    /// 当战场上任意 Power（状态/能力）的层数发生变化后触发。
    /// 
    /// 逻辑流程：
    /// 1. 确认施加者（applier）是遗物持有者本人；
    /// 2. 确认变化量 &gt; 0（只响应中毒增加）；
    /// 3. 确认该 Power 是 PoisonPower（中毒）；
    /// 4. 确认中毒目标是敌人；
    /// 5. 从当前可攻击敌人中随机选取一名，造成 DynamicVars.Damage（1点）伤害。
    /// </summary>
    /// <param name="power">发生层数变化的 Power 实例。</param>
    /// <param name="amount">层数变化量（正值表示增加）。</param>
    /// <param name="applier">施加该 Power 的生物（此处应为玩家角色）。</param>
    /// <param name="cardSource">触发该变化的卡牌来源（若有）。</param>
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner.Creature)
            return;
        if (amount <= 0)
            return;
        if (power is not PoisonPower)
            return;
        if (!power.Owner.IsEnemy)
            return;

        Flash();
        var hittableEnemies = Owner.Creature.CombatState?.HittableEnemies;
        if (hittableEnemies == null) return;

        Creature? enemy = Owner.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
        if (enemy != null)
        {
            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                enemy,
                DynamicVars.Damage.IntValue,
                ValueProp.Unpowered,
                Owner.Creature,
                null
            );
        }
    }
}

/// <summary>
/// Harmony 补丁：阻止异蛇之牙与异蛇之鳞在商店生成。
/// 
/// 原理：RelicFactory.PullNextRelicFromBack(Player, RelicRarity, Func&lt;RelicModel, bool&gt;)
/// 是商店填充/补货遗物时的专用方法。通过 Prefix 将两个遗物的 canonical 实例追加到过滤条件中，
/// 使商店抽奖池在过滤时自动跳过它们。
/// </summary>
[HarmonyPatch(typeof(RelicFactory), nameof(RelicFactory.PullNextRelicFromBack), new[] { typeof(Player), typeof(RelicRarity), typeof(Func<RelicModel, bool>) })]
public static class RelicFactoryShopExclusionPatch
{
    /// <summary>
    /// 在 RelicFactory.PullNextRelicFromBack 执行前，将 SnakeFangRelic 与 SnakeScaleRelic 加入过滤器。
    /// </summary>
    /// <param name="filter">商店当前已使用的遗物过滤函数（ref 参数，可直接修改）。</param>
    static void Prefix(ref Func<RelicModel, bool> filter)
    {
        var originalFilter = filter;
        filter = relic =>
        {
            if (relic is SnakeFangRelic || relic is SnakeScaleRelic)
                return false;
            return originalFilter?.Invoke(relic) ?? true;
        };
    }
}
