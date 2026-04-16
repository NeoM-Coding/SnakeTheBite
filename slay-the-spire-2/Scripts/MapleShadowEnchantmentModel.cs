using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace MapleShadow.Scripts;

// 为项目内所有自定义附魔提供 CustomIconPath 支持
public abstract class MapleShadowEnchantmentModel : EnchantmentModel
{
    public virtual string? CustomIconPath => null;
}

// 拦截 EnchantmentModel.IconPath 的 getter，使其支持 CustomIconPath
[HarmonyPatch(typeof(EnchantmentModel), nameof(EnchantmentModel.IconPath), MethodType.Getter)]
public static class MapleShadowEnchantmentIconPathPatch
{
    [HarmonyPrefix]
    private static bool Prefix(EnchantmentModel __instance, ref string __result)
    {
        if (__instance is MapleShadowEnchantmentModel custom && custom.CustomIconPath != null)
        {
            __result = custom.CustomIconPath;
            return false;
        }
        return true;
    }
}
