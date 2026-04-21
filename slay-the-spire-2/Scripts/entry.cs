// Mod entry point
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace MapleShadow.Scripts;

// 必须要加的属性，用于注册Mod。字符串和初始化函数命名一致。
[ModInitializer(nameof(Init))]
public class Entry
{
    // 初始化函数
    public static void Init()
    {
        // 打patch（即修改游戏代码的功能）用
        // 传入参数随意，只要不和其他人撞车即可
        var harmony = new Harmony("sts2.reme.testmod");
        harmony.PatchAll();
        // 使得tscn可以加载自定义脚本
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);

        // 手动注入附魔本地化（enchantments 表不走 BaseLib 的 Source Generator）
        try
        {
            LocManager.Instance.GetTable("enchantments").MergeWith(new Dictionary<string, string>
            {
                { "SNAKE_VENOM_BOOST_ENCHANTMENT.title", "蛇液强化" },
                { "SNAKE_VENOM_BOOST_ENCHANTMENT.description", "中毒数值 +{Amount}。" }
            });
        }
        catch (Exception ex)
        {
            Log.Warn($"注入附魔本地化失败: {ex.Message}");
        }

        // 注入卡选提示本地化
        try
        {
            LocManager.Instance.GetTable("card_selection").MergeWith(new Dictionary<string, string>
            {
                { "TO_DRAW", "选择 {Amount} 张蛇牌加入手牌" }
            });
        }
        catch (Exception ex)
        {
            Log.Warn($"注入 card_selection 本地化失败: {ex.Message}");
        }

        Log.Debug("模组加载成功 By:MapleShadow");
    }
}
