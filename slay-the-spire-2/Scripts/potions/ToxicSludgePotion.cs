// 浑浊毒液 - 普通药水，给予所有人（包括我方）9层中毒。非商店获取，权重为正常的1/3
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace SnakeTheBite.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class ToxicSludgePotion : SnakeTheBitePotionModel
{
    // 稀有度 - 普通（白）
    public override PotionRarity Rarity => PotionRarity.Common;

    // 使用方式 - 仅战斗中使用
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型 - 自己（效果自动作用于全场，无需选择目标）
    public override TargetType TargetType => TargetType.Self;

    // 动态变量 - 9层中毒
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PoisonPower>(9m)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

    // 使用时的效果逻辑：给予所有可命中角色（敌我双方）9层中毒
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;

        var allCreatures = combatState.GetCreaturesOnSide(CombatSide.Player)
            .Concat(combatState.GetCreaturesOnSide(CombatSide.Enemy))
            .Where(c => c.IsHittable);

        foreach (var creature in allCreatures)
        {
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), creature, DynamicVars["PoisonPower"].BaseValue, Owner.Creature, null, false);
        }
    }
}

// 药水选项过滤器：商店排除 + 非商店时权重为 1/3
[HarmonyPatch(typeof(PotionFactory), nameof(PotionFactory.GetPotionOptions))]
public static class ToxicSludgePotionOptionsPatch
{
    static void Postfix(Player player, ref IEnumerable<PotionModel> __result)
    {
        if (player == null)
            return;

        // 复用黄金蛇血的商店生成标志
        if (GoldenSnakeBloodPotion.IsInShopGeneration)
        {
            __result = __result.Where(p => p is not ToxicSludgePotion);
        }
        else
        {
            // 非商店时，以 1/3 概率保留在候选池中
            __result = __result.Where(p =>
            {
                if (p is not ToxicSludgePotion)
                    return true;
                return player.RunState.Rng.UpFront.NextFloat() < 1f / 3f;
            });
        }
    }
}
