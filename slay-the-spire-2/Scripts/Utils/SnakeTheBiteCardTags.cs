using System;
using MegaCrit.Sts2.Core.Models;

namespace SnakeTheBite.Scripts.Utils;

// 卡牌标签判断工具类
// 统一处理"蛇牌"、"蛇咬牌"等分类逻辑，避免散落在各处的硬编码字符串匹配
// 仅对本 mod（SnakeTheBite）卡牌与原版 Snakebite 生效，防止与其他 mod 冲突
public static class SnakeTheBiteCardTags
{
    private const string OriginalSnakebiteFullName = "MegaCrit.Sts2.Core.Models.Cards.Snakebite";

    // 判断是否为 SnakeTheBite mod 的自定义卡牌
    private static bool IsSnakeTheBiteCard(CardModel card)
    {
        return card.GetType().Namespace?.StartsWith("SnakeTheBite") == true;
    }

    // 判断是否为原版 Snakebite 卡牌
    private static bool IsOriginalSnakebite(CardModel card)
    {
        return card.GetType().FullName == OriginalSnakebiteFullName;
    }

    // 蛇牌：类名中包含 "Snake"
    // 仅识别本 mod 卡牌 + 原版 Snakebite
    public static bool IsSnakeCard(CardModel card)
    {
        if (IsOriginalSnakebite(card))
            return true;

        if (!IsSnakeTheBiteCard(card))
            return false;

        return card.GetType().Name.Contains("Snake", StringComparison.OrdinalIgnoreCase);
    }

    // 蛇咬牌：类名中同时包含 "Snake" 和 "Bite"
    // 仅识别本 mod 卡牌 + 原版 Snakebite
    public static bool IsSnakeBiteCard(CardModel card)
    {
        if (IsOriginalSnakebite(card))
            return true;

        if (!IsSnakeTheBiteCard(card))
            return false;

        var name = card.GetType().Name;
        return name.Contains("Snake", StringComparison.OrdinalIgnoreCase)
            && name.Contains("Bite", StringComparison.OrdinalIgnoreCase);
    }
}
