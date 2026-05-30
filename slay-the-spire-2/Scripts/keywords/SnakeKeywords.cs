// 蛇主题自定义关键字
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SnakeTheBite.Scripts.Keywords;

public class SnakeKeywords
{
    // 命定：获得这张牌时，将升魔·破与升魔·御同时加入你的牌组
    [CustomEnum("FATED")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Fated;
}
