// 混沌蛇液 - 罕见药水，以蛇药水填满你的空药水栏
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace SnakeTheBite.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class ChaosSnakeLiquidPotion : SnakeTheBitePotionModel
{
    // 稀有度 - 稀有（金）
    public override PotionRarity Rarity => PotionRarity.Rare;

    // 使用方式 - 仅战斗中使用
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型 - 自己
    public override TargetType TargetType => TargetType.Self;

    // 使用时的效果逻辑：用蛇主题药水填满空药水栏
    protected override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var player = target!.Player;
        int emptySlots = 0;
        foreach (var slot in player.PotionSlots)
        {
            if (slot == null)
                emptySlots++;
        }

        if (emptySlots == 0)
            return Task.CompletedTask;

        // 收集所有蛇主题药水（排除自身避免递归）
        var snakePotions = ModelDb.AllPotions
            .Where(p => p is SnakeTheBitePotionModel && p.GetType() != typeof(ChaosSnakeLiquidPotion))
            .ToList();

        if (snakePotions.Count == 0)
            return Task.CompletedTask;

        var rng = Owner.RunState.Rng.CombatPotionGeneration;
        for (int i = 0; i < emptySlots; i++)
        {
            var prototype = snakePotions[rng.NextInt(snakePotions.Count)];
            var mutablePotion = prototype.ToMutable();
            player.AddPotionInternal(mutablePotion);
        }
        return Task.CompletedTask;
    }
}
