// 强制第一场普通事件为蛇咬大图书馆
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using SnakeTheBite.Scripts.Events;

namespace SnakeTheBite.Scripts.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyNextEvent))]
public static class SnakeLibraryFirstEventPatch
{
    // BaseLib 的 PrefixIdPatch 会给 ICustomModel 的 entry 加前缀，
    // 因此必须通过 ModelDb 获取实际的 Id.Entry，不能硬编码。
    private static readonly string _snakeLibraryEventEntry = ModelDb.GetId<SnakeLibraryEvent>().Entry;

    public static void Postfix(IRunState runState, EventModel currentEvent, ref EventModel __result)
    {
        // 仅对 RunState 生效
        if (runState is not RunState rs)
            return;

        // 如果已经访问过蛇咬大图书馆，不再替换
        if (rs.VisitedEventIds.Any(id => id.Entry == _snakeLibraryEventEntry))
            return;

        // 强制替换为蛇咬大图书馆事件
        __result = ModelDb.Event<SnakeLibraryEvent>();
    }
}
