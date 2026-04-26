// 真实要蛇了能力
// 每回合抽完牌后，为 Amount 张手牌中的蛇牌毒牌添加真实中毒效果
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using HarmonyLib;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;

namespace SnakeTheBite.Scripts.Powers;

public class TrueSnakePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 当前被标记的毒牌（用于本实例）
    private readonly List<CardModel> _targetCards = new List<CardModel>();

    // 跟踪本回合创建的升级动画 VFX，确保战斗结束或回合结束时被清理
    private readonly List<Godot.Node> _activeVfx = new List<Godot.Node>();

    // 全局被标记的卡牌集合（用于描述修改和 PowerCmd 拦截）
    public static HashSet<CardModel> MarkedCards { get; } = new HashSet<CardModel>();

    // 角色回合开始并抽完牌后触发
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        var hand = PileType.Hand.GetPile(player);
        if (hand == null)
            return;

        // 筛选手牌中的蛇牌毒牌：类名含 Snake 且 DynamicVars 包含 PoisonPower
        var poisonCards = hand.Cards
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c) && (c.DynamicVars.ContainsKey("PoisonPower") || c.DynamicVars.ContainsKey("TruePoisonPower")))
            .ToList();

        if (poisonCards.Count == 0)
            return;

        for (int i = 0; i < Amount; i++)
        {
            if (poisonCards.Count == 0)
                break;

            var card = player.RunState.Rng.CombatCardSelection.NextItem(poisonCards);
            if (card == null)
                continue;

            _targetCards.Add(card);
            MarkedCards.Add(card);
            poisonCards.Remove(card);
            Flash();
            // 播放升级动画（视觉效果），仅本地玩家可见
            if (LocalContext.IsMe(player))
            {
                var vfx = NCardUpgradeVfx.Create(card);
                if (vfx != null)
                {
                    NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
                    _activeVfx.Add(vfx);
                }
            }
        }

        await Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_targetCards.Count == 0)
            return Task.CompletedTask;

        if (_targetCards.Remove(cardPlay.Card))
        {
            MarkedCards.Remove(cardPlay.Card);
            Flash();
        }
        return Task.CompletedTask;
    }

    // 回合结束时清除未打出的标记和残留动画
    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;

        foreach (var card in _targetCards)
        {
            MarkedCards.Remove(card);
        }
        _targetCards.Clear();

        // 清理本回合创建的升级动画，防止战斗结束时卡死残留
        foreach (var vfx in _activeVfx)
        {
            if (Godot.GodotObject.IsInstanceValid(vfx))
                vfx.QueueFree();
        }
        _activeVfx.Clear();

        await Task.CompletedTask;
    }

    // 战斗结束时清理残留动画（兜底）
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        foreach (var vfx in _activeVfx)
        {
            if (Godot.GodotObject.IsInstanceValid(vfx))
                vfx.QueueFree();
        }
        _activeVfx.Clear();

        await Task.CompletedTask;
    }


}

// Harmony 补丁：将被标记卡牌描述中的带数值"中毒"替换为紫色的"真实中毒"
[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetDescriptionForPile), new[] { typeof(PileType), typeof(Creature) })]
public static class TrueSnakeCardDescriptionPatch
{
    private static readonly Regex PoisonRegex = new Regex(@"(\d+)(\s*)(层|点)\s*中毒", RegexOptions.Compiled);

    static void Postfix(CardModel __instance, ref string __result)
    {
        if (TrueSnakePower.MarkedCards.Contains(__instance))
        {
            __result = PoisonRegex.Replace(__result, "$1$2$3[purple]真实中毒[/purple]");
        }
    }
}

// Harmony 补丁：拦截 PowerCmd.Apply 非泛型方法，将标记卡牌新施加的 PoisonPower 替换为 TruePoisonPower
// 0.104 beta 中 Apply 新增了 PlayerChoiceContext 参数，使用 TargetMethod 动态查找以兼容新旧版本
[HarmonyPatch]
public static class TrueSnakeApplyPatch
{
    static MethodBase TargetMethod()
    {
        var targetMethod = AccessTools.DeclaredMethod(typeof(PowerCmd), "Apply",
            [typeof(PlayerChoiceContext), typeof(PowerModel), typeof(Creature), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool)]);
        if (targetMethod == null)
            targetMethod = AccessTools.DeclaredMethod(typeof(PowerCmd), "Apply",
                [typeof(PowerModel), typeof(Creature), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool)]);
        return targetMethod;
    }

    static bool Prefix(object[] __args, ref Task __result)
    {
        // 0.104 新签名含 7 个参数（第一位是 PlayerChoiceContext），旧签名 6 个
        int off = __args.Length == 7 ? 1 : 0;
        var power = (PowerModel)__args[off];
        var target = (Creature)__args[off + 1];
        var amount = (decimal)__args[off + 2];
        var applier = (Creature?)__args[off + 3];
        var cardSource = (CardModel?)__args[off + 4];
        var silent = (bool)__args[off + 5];

        if (power is PoisonPower && cardSource != null && TrueSnakePower.MarkedCards.Contains(cardSource))
        {
            __result = PowerCmd.Apply<TruePoisonPower>(new ThrowingPlayerChoiceContext(), target, amount, applier, cardSource, silent);
            return false;
        }
        return true;
    }
}

// Harmony 补丁：拦截 PowerCmd.ModifyAmount，将标记卡牌追加的 PoisonPower 层数改为追加 TruePoisonPower
[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.ModifyAmount), new[] { typeof(PlayerChoiceContext), typeof(PowerModel), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool) })]
public static class TrueSnakeModifyAmountPatch
{
    static bool Prefix(PowerModel power, decimal offset, Creature? applier, CardModel? cardSource, bool silent, ref Task<int> __result)
    {
        if (power is PoisonPower && cardSource != null && TrueSnakePower.MarkedCards.Contains(cardSource) && power.Owner != null)
        {
            __result = RedirectToTruePoison(power.Owner, offset, applier, cardSource, silent);
            return false;
        }
        return true;
    }

    private static async Task<int> RedirectToTruePoison(Creature target, decimal offset, Creature? applier, CardModel? cardSource, bool silent)
    {
        var existing = target.GetPower<TruePoisonPower>();
        if (existing == null)
        {
            await PowerCmd.Apply<TruePoisonPower>(new ThrowingPlayerChoiceContext(), target, offset, applier, cardSource, silent);
            return (int)offset;
        }
        else
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), existing, offset, applier, cardSource, silent);
            return existing.Amount;
        }
    }
}
