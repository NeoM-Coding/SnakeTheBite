// 进阶之蛇的 Harmony 补丁
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using SnakeTheBite.Scripts.Relics;

namespace SnakeTheBite.Scripts.Patches;

// 在捏奥（Neow）的选项中添加进阶之蛇作为第四个选项
[HarmonyPatch(typeof(Neow))]
public static class NeowAdvancedSnakeOptionPatch
{
    static System.Reflection.MethodBase TargetMethod() => AccessTools.Method(typeof(Neow), "GenerateInitialOptions");

    static void Postfix(Neow __instance, ref IReadOnlyList<EventOption> __result)
    {
        // 只在标准模式（无自定义 modifiers）下添加，且确保不重复
        if (__instance.Owner.RunState.Modifiers.Count > 0)
            return;
        if (__result.Any(o => o.Relic is AdvancedSnakeRelic))
            return;

        var relic = ModelDb.Relic<AdvancedSnakeRelic>().ToMutable();
        if (relic is not AdvancedSnakeRelic advancedRelic)
            return;
        advancedRelic.Owner = __instance.Owner;
        advancedRelic.UpdateDescription();

        var option = EventOption.FromRelic(
            advancedRelic,
            __instance,
            async () =>
            {
                await RelicCmd.Obtain(relic, __instance.Owner);
                // 结束 Neow 事件，显示标准完成页面
                __instance.StartPreFinished();
            },
            "NEOW.pages.INITIAL.SNAKETHEBITE-ADVANCED_SNAKE_RELIC"
        );

        var list = __result.ToList();
        list.Add(option);
        __result = list;
    }
}
