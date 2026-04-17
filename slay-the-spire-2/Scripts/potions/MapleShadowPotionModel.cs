// Potion base class
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace MapleShadow.Scripts.Potions;

public abstract class MapleShadowPotionModel : CustomPotionModel
{
    private string? _potionImagePath;

    private string PotionImagePath => _potionImagePath ??= $"res://MapleShadow/images/potions/MapleShadow-{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    /// <summary>药水自定义图片路径。</summary>
    public virtual string? CustomImagePath => PotionImagePath;

    /// <summary>药水自定义轮廓图片路径。</summary>
    public virtual string? CustomOutlinePath => PotionImagePath;
}

// Harmony 补丁：拦截 PotionModel.PackedImagePath 的 getter，使其支持 CustomImagePath
[HarmonyPatch(typeof(PotionModel), "PackedImagePath", MethodType.Getter)]
public static class MapleShadowPotionImagePathPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PotionModel __instance, ref string __result)
    {
        if (__instance is MapleShadowPotionModel custom && custom.CustomImagePath != null)
        {
            __result = custom.CustomImagePath;
            return false;
        }
        return true;
    }
}

// Harmony 补丁：拦截 PotionModel.PackedOutlinePath 的 getter，使其支持 CustomOutlinePath
[HarmonyPatch(typeof(PotionModel), "PackedOutlinePath", MethodType.Getter)]
public static class MapleShadowPotionOutlinePathPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PotionModel __instance, ref string __result)
    {
        if (__instance is MapleShadowPotionModel custom && custom.CustomOutlinePath != null)
        {
            __result = custom.CustomOutlinePath;
            return false;
        }
        return true;
    }
}
