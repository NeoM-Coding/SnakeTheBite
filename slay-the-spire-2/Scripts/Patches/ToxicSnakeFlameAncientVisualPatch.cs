// 让 ToxicSnakeFlameCard 使用远古卡牌牌面，但保持原有稀有度不变
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SnakeTheBite.Scripts.Cards;

namespace SnakeTheBite.Scripts.Patches;

public static class ToxicSnakeFlameAncientVisualPatch
{
    // 在 NCard.Reload 后强制切换为远古节点显示
    // 0.104 中 Reload 为 private 方法，需使用 TargetMethod 动态查找
    [HarmonyPatch]
    public static class NCardReloadPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(NCard), "Reload");
        }

        static void Postfix(NCard __instance)
        {
            if (__instance.Model is not ToxicSnakeFlameCard)
                return;

            var traverse = Traverse.Create(__instance);

            // 获取节点引用
            var ancientPortrait = traverse.Field<TextureRect>("_ancientPortrait").Value;
            var ancientBorder = traverse.Field<TextureRect>("_ancientBorder").Value;
            var ancientTextBg = traverse.Field<TextureRect>("_ancientTextBg").Value;
            var ancientBanner = traverse.Field<Control>("_ancientBanner").Value;
            var ancientHighlight = traverse.Field<TextureRect>("_ancientHighlight").Value;
            var portrait = traverse.Field<TextureRect>("_portrait").Value;
            var portraitBorder = traverse.Field<TextureRect>("_portraitBorder").Value;
            var frame = traverse.Field<TextureRect>("_frame").Value;
            var banner = traverse.Field<TextureRect>("_banner").Value;
            var portraitCanvasGroup = traverse.Field<CanvasGroup>("_portraitCanvasGroup").Value;

            // 显示远古节点
            ancientPortrait.Visible = true;
            ancientBorder.Visible = true;
            ancientTextBg.Visible = true;
            ancientBanner.Visible = true;
            ancientHighlight.Visible = true;

            // 隐藏普通节点
            portrait.Visible = false;
            portraitBorder.Visible = false;
            frame.Visible = false;
            banner.Visible = false;

            // 设置 portraitCanvasGroup 材质（根据可见性状态）
            if (portraitCanvasGroup != null)
            {
                if (__instance.Visibility != ModelVisibility.Visible)
                {
                    var blurMat = PreloadManager.Cache.GetMaterial("res://scenes/cards/card_canvas_group_mask_blur_material.tres");
                    portraitCanvasGroup.Material = blurMat;
                }
                else
                {
                    var maskMat = PreloadManager.Cache.GetMaterial("res://scenes/cards/card_canvas_group_mask_material.tres");
                    portraitCanvasGroup.Material = maskMat;
                }
            }

            // 设置远古资源
            ancientPortrait.Texture = __instance.Model.Portrait;
            string textBgPath = ImageHelper.GetImagePath(
                "atlases/compressed.sprites/card_template/ancient_card_text_bg_"
                + __instance.Model.Type.ToString().ToLowerInvariant()
                + ".tres");
            ancientTextBg.Texture = ResourceLoader.Load<Texture2D>(textBgPath, null, ResourceLoader.CacheMode.Reuse);
        }
    }

    // 让 BannerMaterial 返回远古材质，确保类型标签等也使用远古风格
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.BannerMaterial), MethodType.Getter)]
    public static class CardModelBannerMaterialPatch
    {
        static void Postfix(CardModel __instance, ref Material __result)
        {
            if (__instance is ToxicSnakeFlameCard)
            {
                __result = PreloadManager.Cache.GetMaterial("res://materials/cards/banners/card_banner_ancient_mat.tres");
            }
        }
    }
}
