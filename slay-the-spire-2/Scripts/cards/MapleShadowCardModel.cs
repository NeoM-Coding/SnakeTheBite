using System.Text;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace MapleShadow.Scripts.Cards;

/// <summary>
/// MapleShadow 自定义卡牌基类，自动根据类名生成卡图路径，避免每张卡都手写 PortraitPath。
/// </summary>
public abstract class MapleShadowCardModel : CustomCardModel
{
    /// <summary>
    /// 自动映射卡图路径，规则：res://MapleShadow/images/cards/MapleShadow-{类名_snake_case}.png
    /// 例如 TestCard -> MapleShadow-test_card.png
    /// </summary>
    public override string PortraitPath => $"res://MapleShadow/images/cards/MapleShadow-{ToSnakeCase(GetType().Name)}.png";

    protected MapleShadowCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    private static string ToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        StringBuilder result = new StringBuilder();
        result.Append(char.ToLowerInvariant(text[0]));

        for (int i = 1; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
