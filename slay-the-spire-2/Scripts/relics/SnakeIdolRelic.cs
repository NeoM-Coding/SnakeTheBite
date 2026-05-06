// 白蛇雕像 - 金色遗物
// 战斗结束后额外掉落1瓶蛇药，独立于正常药水掉落。
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using SnakeTheBite.Scripts.Potions;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeIdolRelic : SnakeTheBiteRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override bool IsAllowedInShops => true;

    private static readonly List<Type> _snakePotionTypes = new()
    {
        typeof(DeadlySnakeVenomPotion),
        typeof(ToxicSludgePotion),
        typeof(SnakeDenseBrewPotion),
        typeof(SnakeHoneyBrewPotion),
        typeof(SnakeSecretBrewPotion),
        typeof(SnakeSerumPotion),
        typeof(GoldenSnakeBloodPotion),
        typeof(ChaosSnakeLiquidPotion),
        typeof(SnakeInABottlePotion),
        typeof(EssenceSnakeVenomPotion)
    };

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        Flash();
        var rng = Owner.RunState.Rng.CombatPotionGeneration;
        var potionType = rng.NextItem(_snakePotionTypes);
        var method = typeof(ModelDb).GetMethod(nameof(ModelDb.Potion))!.MakeGenericMethod(potionType);
        var potion = ((PotionModel)method.Invoke(null, null)!).ToMutable();
        await PotionCmd.TryToProcure(potion, Owner);
    }
}
