// 进阶之蛇的 Harmony 补丁（已禁用）
// using System.Collections.Generic;
// using System.Linq;
// using HarmonyLib;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Localization;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Models.Cards;
// using SnakeTheBite.Scripts.Relics;
//
// namespace SnakeTheBite.Scripts.Patches;
//
// // 进阶5：移除进阶之灾的永恒效果
// [HarmonyPatch(typeof(AscendersBane), nameof(AscendersBane.CanonicalKeywords), MethodType.Getter)]
// public static class AdvancedSnakeAscendersBanePatch
// {
//     static void Postfix(AscendersBane __instance, ref IEnumerable<CardKeyword> __result)
//     {
//         if (!__instance.IsMutable)
//             return;
//         var owner = __instance.Owner;
//         if (owner == null)
//             return;
//         var relic = owner.GetRelic<AdvancedSnakeRelic>();
//         if (relic == null)
//             return;
//         if (owner.RunState.AscensionLevel >= 5)
//         {
//             __result = __result.Where(k => k != CardKeyword.Eternal).ToList();
//         }
//     }
// }
//
// // 进阶之蛇描述动态注入
// [HarmonyPatch(typeof(RelicModel), "DynamicDescription", MethodType.Getter)]
// public static class AdvancedSnakeRelicDynamicDescriptionPatch
// {
//     static void Postfix(RelicModel __instance, ref LocString __result)
//     {
//         if (__instance is not AdvancedSnakeRelic relic)
//             return;
//         if (!relic.IsMutable)
//             return;
//         int asc = relic.Owner?.RunState.AscensionLevel ?? 0;
//         __result.Add("HasAsc1", asc >= 1 ? 1m : 0m);
//         __result.Add("HasAsc2", asc >= 2 ? 1m : 0m);
//         __result.Add("HasAsc3", asc >= 3 ? 1m : 0m);
//         __result.Add("HasAsc4", asc >= 4 ? 1m : 0m);
//         __result.Add("HasAsc5", asc >= 5 ? 1m : 0m);
//         __result.Add("HasAsc6", asc >= 6 ? 1m : 0m);
//         __result.Add("HasAsc7", asc >= 7 ? 1m : 0m);
//         __result.Add("HasAsc8", asc >= 8 ? 1m : 0m);
//         __result.Add("HasAsc9", asc >= 9 ? 1m : 0m);
//         __result.Add("HasAsc10", asc >= 10 ? 1m : 0m);
//         bool boosted = asc >= 10;
//         __result.Add("Precision", boosted ? 2m : 1m);
//         __result.Add("AncientHeal", boosted ? 10m : 7m);
//         __result.Add("BonusGold", boosted ? 10m : 7m);
//         __result.Add("PotionCount", boosted ? 2m : 1m);
//         __result.Add("RemovalDiscount", boosted ? 10m : 7m);
//         __result.Add("UpgradeCount", boosted ? 2m : 1m);
//         __result.Add("CombatDamage", boosted ? 10m : 7m);
//         __result.Add("TempStrengthLoss", boosted ? 2m : 1m);
//     }
// }
