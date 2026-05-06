// 蛇蜕抽牌上限补丁 - 修改 CardPileCmd.Draw 中的手牌上限硬编码
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using SnakeTheBite.Scripts.Relics;

namespace SnakeTheBite.Scripts.Patches;

[HarmonyPatch(typeof(CardPileCmd), "Draw")]
[HarmonyPatch(new[] { typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool) })]
public class SnakeSkinDrawPatch
{
    public static int GetMaxHandSize(Player player)
    {
        if (player?.Relics?.OfType<SnakeSkinRelic>().Any() == true)
            return 12;
        return 10;
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var getMaxHandSize = AccessTools.Method(typeof(SnakeSkinDrawPatch), nameof(GetMaxHandSize));

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 10)
            {
                codes[i] = new CodeInstruction(OpCodes.Ldarg_2);
                codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, getMaxHandSize));
                i++;
            }
        }
        return codes;
    }
}
