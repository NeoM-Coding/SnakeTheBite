using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Relics;

/// <summary>
/// 异蛇之鳞（SnakeScaleRelic）——普通遗物。
/// 
/// 效果：每当你对敌人施加中毒时，获得 1 点格挡。
/// 获取限制：通过 RelicFactoryShopExclusionPatch 排除在商店生成之外，
/// 但仍可通过战斗奖励、宝箱、事件等非商店途径获得。
/// </summary>
[Pool(typeof(SharedRelicPool))]
public class SnakeScaleRelic : CustomRelicModel
{
    /// <summary>遗物稀有度：普通（白）。</summary>
    public override RelicRarity Rarity => RelicRarity.Common;

    /// <summary>
    /// 定义遗物的核心动态变量。
    /// 此处使用 BlockVar(1, Unpowered)，在本地化文本中可用 {Block} 引用该数值。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(1, ValueProp.Unpowered)];

    /// <summary>
    /// 遗物图标路径（小图标，通常 85×85）。
    /// 使用遗物 ID 的小写形式作为文件名，对应 res://MapleShadow/images/relics/mapleshadow-snake_scale_relic.png。
    /// </summary>
    // public override string PackedIconPath => $"res://MapleShadow/images/relics/{Id.Entry.ToLowerInvariant()}.png";

    /// <summary>遗物轮廓图标路径（通常 85×85）。</summary>
    // protected override string PackedIconOutlinePath => $"res://MapleShadow/images/relics/{Id.Entry.ToLowerInvariant()}.png";

    /// <summary>遗物大图标路径（通常 256×256）。</summary>
    // protected override string BigIconPath => $"res://MapleShadow/images/relics/{Id.Entry.ToLowerInvariant()}.png";

    /// <summary>
    /// 当战场上任意 Power（状态/能力）的层数发生变化后触发。
    /// 
    /// 逻辑流程：
    /// 1. 确认施加者（applier）是遗物持有者本人；
    /// 2. 确认变化量 &gt; 0（只响应中毒增加）；
    /// 3. 确认该 Power 是 PoisonPower（中毒）；
    /// 4. 确认中毒目标是敌人；
    /// 5. 使玩家角色获得 DynamicVars.Block（1点）格挡。
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
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.IntValue,
            ValueProp.Unpowered,
            null
        );
    }
}
