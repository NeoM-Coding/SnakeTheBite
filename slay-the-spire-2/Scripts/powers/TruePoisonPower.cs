// 真实中毒 - 无视人工、缓冲、无实体等限伤效果的中毒
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.addons.mega_text;

namespace SnakeTheBite.Scripts.Powers;

public class TruePoisonPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 计算下回合将要造成的伤害（用于血条绿色预览）
    // 适配触媒（AccelerantPower）：每有一层触媒额外触发一次
    private int TriggerCount
    {
        get
        {
            IEnumerable<Creature> source = from c in Owner.CombatState.GetOpponentsOf(Owner)
                where c.IsAlive
                select c;
            return Math.Min((int)Amount, 1 + source.Sum(a => a.GetPowerAmount<AccelerantPower>()));
        }
    }

    public int CalculateTotalDamageNextTurn()
    {
        decimal num = default(decimal);
        int iterations = TriggerCount;
        for (int i = 0; i < iterations; i++)
        {
            decimal damage = Amount;
            damage = Hook.ModifyDamage(Owner.CombatState.RunState, Owner.CombatState, Owner, null, damage, ValueProp.Unblockable | ValueProp.Unpowered, null, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);
            num += damage;
        }
        return (int)num;
    }

    // 标志位，用于 Harmony 补丁识别真实中毒造成的伤害
    public static bool IsDealingDamage { get; set; }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;

        int iterations = TriggerCount;
        for (int i = 0; i < iterations; i++)
        {
            int damage = (int)Amount;
            IsDealingDamage = true;
            try
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, damage, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            }
            finally
            {
                IsDealingDamage = false;
            }

            if (!Owner.IsAlive)
            {
                await Cmd.CustomScaledWait(0.1f, 0.25f);
                break;
            }
        }
    }
}

// 血条绿色预览辅助方法
internal static class TruePoisonHealthBarHelper
{
    private static float GetFgWidth(int amount, float maxFgWidth, Creature creature)
    {
        if (creature.MaxHp <= 0)
            return 0f;
        float val = (float)amount / (float)creature.MaxHp * maxFgWidth;
        return Math.Max(val, creature.CurrentHp > 0 ? 12f : 0f);
    }

    public static void PostfixRefreshForeground(NHealthBar __instance, Creature creature)
    {
        if (creature == null)
            return;

        int poisonDamage = creature.GetPower<PoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
        int truePoisonDamage = creature.GetPower<TruePoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
        int totalPoison = poisonDamage + truePoisonDamage;
        if (truePoisonDamage <= 0)
            return;

        var poisonFg = Traverse.Create(__instance).Field<Control>("_poisonForeground").Value;
        var hpFg = Traverse.Create(__instance).Field<Control>("_hpForeground").Value;
        var doomFg = Traverse.Create(__instance).Field<Control>("_doomForeground").Value;
        float maxFgWidth = Traverse.Create(__instance).Property<float>("MaxFgWidth").Value;

        float offsetRight = GetFgWidth(creature.CurrentHp, maxFgWidth, creature) - maxFgWidth;

        if (totalPoison > 0)
        {
            poisonFg.Visible = true;
            if (totalPoison >= creature.CurrentHp)
            {
                poisonFg.OffsetLeft = 0f;
                poisonFg.OffsetRight = offsetRight;
                hpFg.Visible = false;
            }
            else
            {
                float fgWidth = GetFgWidth(creature.CurrentHp - totalPoison, maxFgWidth, creature);
                hpFg.OffsetRight = fgWidth - maxFgWidth;
                hpFg.Visible = true;
                int patchMarginLeft = ((NinePatchRect)poisonFg).PatchMarginLeft;
                poisonFg.OffsetLeft = Math.Max(0f, fgWidth - patchMarginLeft);
                poisonFg.OffsetRight = offsetRight;
            }
        }
        else
        {
            poisonFg.Visible = false;
            poisonFg.OffsetLeft = 0f;
        }

        int doomAmount = creature.GetPowerAmount<DoomPower>();
        if (doomAmount > 0)
        {
            doomFg.Visible = true;
            float doomRight = GetFgWidth(doomAmount, maxFgWidth, creature) - maxFgWidth;
            if (doomAmount >= creature.CurrentHp - totalPoison)
            {
                if (totalPoison < creature.CurrentHp)
                {
                    doomFg.OffsetRight = hpFg.OffsetRight;
                    hpFg.Visible = false;
                }
                else
                {
                    hpFg.Visible = false;
                    doomFg.Visible = false;
                }
            }
            else
            {
                int patchMarginRight = ((NinePatchRect)doomFg).PatchMarginRight;
                doomFg.OffsetRight = Math.Min(0f, doomRight + patchMarginRight);
                hpFg.Visible = true;
            }
        }
    }

    public static void PostfixRefreshText(NHealthBar __instance, Creature creature)
    {
        if (creature == null)
            return;

        int poisonDamage = creature.GetPower<PoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
        int truePoisonDamage = creature.GetPower<TruePoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
        int totalPoison = poisonDamage + truePoisonDamage;
        if (truePoisonDamage <= 0)
            return;

        int doomAmount = creature.GetPowerAmount<DoomPower>();
        var hpLabel = Traverse.Create(__instance).Field<Godot.Label>("_hpLabel").Value;

        if (totalPoison > 0 && totalPoison >= creature.CurrentHp)
        {
            hpLabel.AddThemeColorOverride("font_color", new Color("76FF40"));
            hpLabel.AddThemeColorOverride("font_outline_color", new Color("074700"));
        }
        else if (doomAmount > 0 && doomAmount >= creature.CurrentHp - totalPoison)
        {
            hpLabel.AddThemeColorOverride("font_color", new Color("FB8DFF"));
            hpLabel.AddThemeColorOverride("font_outline_color", new Color("2D1263"));
        }
    }
}

// Harmony 补丁：为 NHealthBar 添加真实中毒的绿色预览
[HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
public static class NHealthBarRefreshForegroundPatch
{
    static void Postfix(NHealthBar __instance)
    {
        var creature = Traverse.Create(__instance).Field<Creature>("_creature").Value;
        TruePoisonHealthBarHelper.PostfixRefreshForeground(__instance, creature);
    }
}

[HarmonyPatch(typeof(NHealthBar), "RefreshText")]
public static class NHealthBarRefreshTextPatch
{
    static void Postfix(NHealthBar __instance)
    {
        var creature = Traverse.Create(__instance).Field<Creature>("_creature").Value;
        TruePoisonHealthBarHelper.PostfixRefreshText(__instance, creature);
    }
}

// Harmony 补丁：阻止 ArtifactPower 抵消真实中毒
[HarmonyPatch(typeof(ArtifactPower), nameof(ArtifactPower.TryModifyPowerAmountReceived))]
public static class TruePoisonArtifactPatch
{
    static bool Prefix(PowerModel canonicalPower, ref bool __result, ref decimal modifiedAmount)
    {
        if (canonicalPower is TruePoisonPower)
        {
            __result = false;
            modifiedAmount = canonicalPower.Amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 IntangiblePower 限制真实中毒伤害（ModifyDamageCap 与 ModifyHpLostAfterOsty 双保险）
[HarmonyPatch(typeof(IntangiblePower), nameof(IntangiblePower.ModifyDamageCap))]
public static class TruePoisonIntangibleDamageCapPatch
{
    static bool Prefix(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = decimal.MaxValue;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(IntangiblePower), nameof(IntangiblePower.ModifyHpLostAfterOsty))]
public static class TruePoisonIntangiblePatch
{
    static bool Prefix(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 HardToKillPower 限制真实中毒伤害
[HarmonyPatch(typeof(HardToKillPower), nameof(HardToKillPower.ModifyDamageCap))]
public static class TruePoisonHardToKillDamageCapPatch
{
    static bool Prefix(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = decimal.MaxValue;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 SlipperyPower 限制真实中毒伤害
[HarmonyPatch(typeof(SlipperyPower), nameof(SlipperyPower.ModifyDamageCap))]
public static class TruePoisonSlipperyDamageCapPatch
{
    static bool Prefix(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = decimal.MaxValue;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 BufferPower 抵消真实中毒伤害
[HarmonyPatch(typeof(BufferPower), nameof(BufferPower.ModifyHpLostAfterOstyLate))]
public static class TruePoisonBufferPatch
{
    static bool Prefix(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 HardenedShellPower（硬化外壳）限制真实中毒伤害
[HarmonyPatch(typeof(HardenedShellPower), nameof(HardenedShellPower.ModifyHpLostBeforeOstyLate))]
public static class TruePoisonHardenedShellPatch
{
    static bool Prefix(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：统一阻止通过 ModifyDamageCap 限制真实中毒伤害的 Power（兜底）
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyDamageCap))]
public static class TruePoisonDamageCapPatch
{
    static bool Prefix(AbstractModel __instance, Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            if (__instance is IntangiblePower or HardToKillPower or SlipperyPower)
            {
                __result = decimal.MaxValue;
                return false;
            }
        }
        return true;
    }
}

// Harmony 补丁：阻止 HardenedShellPower 将真实中毒伤害计入计数
[HarmonyPatch(typeof(HardenedShellPower), nameof(HardenedShellPower.AfterDamageReceived))]
public static class TruePoisonHardenedShellAfterDamageReceivedPatch
{
    static bool Prefix(ref Task __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            __result = Task.CompletedTask;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：统一阻止通过 ModifyHpLostBeforeOstyLate 限制真实中毒伤害（兜底）
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyHpLostBeforeOstyLate))]
public static class TruePoisonHpLostBeforeOstyLatePatch
{
    static bool Prefix(AbstractModel __instance, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            if (__instance is HardenedShellPower)
            {
                __result = amount;
                return false;
            }
        }
        return true;
    }
}

// Harmony 补丁：统一阻止通过 ModifyHpLostAfterOstyLate 抵消真实中毒伤害的 Power（兜底）
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyHpLostAfterOstyLate))]
public static class TruePoisonHpLostAfterOstyLatePatch
{
    static bool Prefix(AbstractModel __instance, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TruePoisonPower.IsDealingDamage)
        {
            if (__instance is BufferPower)
            {
                __result = amount;
                return false;
            }
        }
        return true;
    }
}
