// 泪滴蛇吊盒 - 罕见遗物，战斗开始时进入蓄蛇
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class TearDropSnakePendantRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：罕见（蓝）
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ChargingSnakePower>()];

    // 战斗开始时进入蓄蛇
    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<ChargingSnakePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, null, false);
    }
}
