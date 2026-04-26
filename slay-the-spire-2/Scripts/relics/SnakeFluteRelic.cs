// 蛇之长笛 - 普通遗物，战斗开始时获得3层蛇之活力
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeFluteRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：普通（白）
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SnakeVitalityPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SnakeVitalityPower>()];

    // 战斗开始时获得蛇之活力
    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<SnakeVitalityPower>(Owner.Creature, DynamicVars["SnakeVitalityPower"].BaseValue, Owner.Creature, null);
    }
}
