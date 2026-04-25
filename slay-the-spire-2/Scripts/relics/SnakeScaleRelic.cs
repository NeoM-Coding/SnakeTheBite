// 异蛇之鳞 - 普通遗物，给敌人上毒时获得1格挡
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeScaleRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：普通（白）。
    public override RelicRarity Rarity => RelicRarity.Common;

    //
    // 定义遗物的核心动态变量。
    // 此处使用 BlockVar(1, Unpowered)，在本地化文本中可用 {Block} 引用该数值。
    //
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(1, ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<TruePoisonPower>()];

    //
    // 当战场上任意 Power（状态/能力）的层数发生变化后触发。
    //
    // 逻辑流程：
    // 1. 确认施加者（applier）是遗物持有者本人；
    // 2. 确认变化量 > 0（只响应中毒增加）；
    // 3. 确认该 Power 是 PoisonPower（中毒）；
    // 4. 确认中毒目标是敌人；
    // 5. 使玩家角色获得 DynamicVars.Block（1点）格挡。
    //
    // 发生层数变化的 Power 实例。
    // 层数变化量（正值表示增加）。
    // 施加该 Power 的生物（此处应为玩家角色）。
    // 触发该变化的卡牌来源（若有）。
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner.Creature)
            return;
        if (amount <= 0)
            return;
        if (power is not PoisonPower and not TruePoisonPower)
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
