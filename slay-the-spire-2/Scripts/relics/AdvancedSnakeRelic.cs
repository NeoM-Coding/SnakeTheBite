// 进阶之蛇 - 稀有遗物（当前已禁用所有效果）
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(EventRelicPool))]
public class AdvancedSnakeRelic : SnakeTheBiteRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override bool IsAllowedInShops => false;

    // 所有进阶效果已注释掉
    // public override bool HasUponPickupEffect => false;

    // protected override IEnumerable<DynamicVar> CanonicalVars => [];

    // protected override void DeepCloneFields() { }

    // public override Task BeforeCombatStart() => Task.CompletedTask;
    // public override Task AfterRoomEntered(AbstractRoom room) => Task.CompletedTask;
    // public override Task AfterCombatEnd(CombatRoom room) => Task.CompletedTask;
    // public override Task AfterObtained() => Task.CompletedTask;
    // public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal cost) => cost;
}
