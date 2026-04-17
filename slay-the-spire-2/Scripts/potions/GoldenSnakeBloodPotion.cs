// 黄金蛇血 - 罕见药水，获得25金币
using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace MapleShadow.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class GoldenSnakeBloodPotion : MapleShadowPotionModel
{
    // 稀有度 - 罕见（蓝）
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    // 使用方式 - 任何时候都能使用（战斗内外皆可）
    public override PotionUsage Usage => PotionUsage.AnyTime;

    // 目标类型 - 任意玩家（可以扔给队友）
    public override TargetType TargetType => TargetType.AnyPlayer;

    // 动态变量 - 获得25金币
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(25)];

    // 药水自定义图片路径
    public override string? CustomPackedImagePath => "res://MapleShadow/images/potions/MapleShadow-golden_snake_blood_potion.png";
    public override string? CustomPackedOutlinePath => "res://MapleShadow/images/potions/MapleShadow-golden_snake_blood_potion.png";

    // 标记是否正在商店生成过程中
    public static bool IsInShopGeneration { get; set; }

    // 使用时的效果逻辑
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, target.Player!);
    }
}

// 商店库存生成补丁：在商店生成药水时将黄金蛇血加入黑名单
[HarmonyPatch(typeof(MerchantInventory), "PopulatePotionEntries")]
public static class MerchantInventoryPotionPatch
{
    static void Prefix()
    {
        GoldenSnakeBloodPotion.IsInShopGeneration = true;
    }

    static void Finalizer()
    {
        GoldenSnakeBloodPotion.IsInShopGeneration = false;
    }
}

// 商店补货补丁：在商店补货时将黄金蛇血加入黑名单
[HarmonyPatch(typeof(MerchantPotionEntry), "FillSlot")]
public static class MerchantPotionEntryRestockPatch
{
    static void Prefix()
    {
        GoldenSnakeBloodPotion.IsInShopGeneration = true;
    }

    static void Finalizer()
    {
        GoldenSnakeBloodPotion.IsInShopGeneration = false;
    }
}

// 药水选项过滤器：当处于商店生成流程时，排除黄金蛇血
[HarmonyPatch(typeof(PotionFactory), nameof(PotionFactory.GetPotionOptions))]
public static class PotionFactoryShopExclusionPatch
{
    static void Postfix(ref IEnumerable<PotionModel> __result)
    {
        if (GoldenSnakeBloodPotion.IsInShopGeneration)
        {
            __result = __result.Where(p => p is not GoldenSnakeBloodPotion);
        }
    }
}
