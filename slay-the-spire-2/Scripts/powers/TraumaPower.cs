// 创伤 - 受到物理攻击时额外流失生命，无视限伤
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

public class TraumaPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    // 标志位，用于 Harmony 补丁识别创伤造成的流失生命
    public static bool IsDealingDamage { get; set; }

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
            return;
        if (!props.IsPoweredAttack())
            return;
        // 防御性检查：目标已死亡或已离开战斗时不再触发
        if (Owner == null || Owner.IsDead || Owner.CombatState == null)
            return;

        IsDealingDamage = true;
        try
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.SkipHurtAnim, dealer, cardSource);
        }
        finally
        {
            IsDealingDamage = false;
        }
    }
}

// Harmony 补丁：阻止 IntangiblePower 限制创伤伤害
[HarmonyPatch(typeof(IntangiblePower), nameof(IntangiblePower.ModifyDamageCap))]
public static class TraumaIntangibleDamageCapPatch
{
    static bool Prefix(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
        {
            __result = decimal.MaxValue;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(IntangiblePower), nameof(IntangiblePower.ModifyHpLostAfterOsty))]
public static class TraumaIntangibleHpLostPatch
{
    static bool Prefix(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
        {
            __result = amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 HardToKillPower 限制创伤伤害
[HarmonyPatch(typeof(HardToKillPower), nameof(HardToKillPower.ModifyDamageCap))]
public static class TraumaHardToKillDamageCapPatch
{
    static bool Prefix(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
        {
            __result = decimal.MaxValue;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 SlipperyPower 限制创伤伤害
[HarmonyPatch(typeof(SlipperyPower), nameof(SlipperyPower.ModifyDamageCap))]
public static class TraumaSlipperyDamageCapPatch
{
    static bool Prefix(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
        {
            __result = decimal.MaxValue;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 BufferPower 抵消创伤伤害
[HarmonyPatch(typeof(BufferPower), nameof(BufferPower.ModifyHpLostAfterOstyLate))]
public static class TraumaBufferPatch
{
    static bool Prefix(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
        {
            __result = amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：阻止 HardenedShellPower 限制创伤伤害
[HarmonyPatch(typeof(HardenedShellPower), nameof(HardenedShellPower.ModifyHpLostBeforeOstyLate))]
public static class TraumaHardenedShellPatch
{
    static bool Prefix(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
        {
            __result = amount;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：统一阻止通过 ModifyDamageCap 限制创伤伤害的 Power（兜底）
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyDamageCap))]
public static class TraumaDamageCapPatch
{
    static bool Prefix(AbstractModel __instance, Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
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

// Harmony 补丁：阻止 HardenedShellPower 将创伤伤害计入计数
[HarmonyPatch(typeof(HardenedShellPower), nameof(HardenedShellPower.AfterDamageReceived))]
public static class TraumaHardenedShellAfterDamageReceivedPatch
{
    static bool Prefix()
    {
        return !TraumaPower.IsDealingDamage;
    }
}

// Harmony 补丁：统一阻止通过 ModifyHpLostBeforeOstyLate 限制创伤伤害（兜底）
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyHpLostBeforeOstyLate))]
public static class TraumaHpLostBeforeOstyLatePatch
{
    static bool Prefix(AbstractModel __instance, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
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

// Harmony 补丁：统一阻止通过 ModifyHpLostAfterOstyLate 抵消创伤伤害的 Power（兜底）
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.ModifyHpLostAfterOstyLate))]
public static class TraumaHpLostAfterOstyLatePatch
{
    static bool Prefix(AbstractModel __instance, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, ref decimal __result)
    {
        if (TraumaPower.IsDealingDamage)
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
