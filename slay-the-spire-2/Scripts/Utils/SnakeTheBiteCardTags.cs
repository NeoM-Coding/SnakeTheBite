using System;
using MegaCrit.Sts2.Core.Models;

namespace SnakeTheBite.Scripts.Utils;

// 卡牌标签判断工具类
// 统一处理“蛇牌”、“蛇咬牌”等分类逻辑，避免散落在各处的硬编码字符串匹配
public static class SnakeTheBiteCardTags
{
    // 蛇牌：类名中包含 "Snake"
    public static bool IsSnakeCard(CardModel card)
    {
        return card.GetType().Name.Contains("Snake", StringComparison.OrdinalIgnoreCase);
    }

    // 蛇咬牌：类名中同时包含 "Snake" 和 "Bite"
    public static bool IsSnakeBiteCard(CardModel card)
    {
        var name = card.GetType().Name;
        return name.Contains("Snake", StringComparison.OrdinalIgnoreCase)
            && name.Contains("Bite", StringComparison.OrdinalIgnoreCase);
    }
}
