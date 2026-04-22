// Card base class
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace SnakeTheBite.Scripts.Cards;

public abstract class SnakeTheBiteCardModel : CustomCardModel
{
    // 自动映射卡图路径，规则：res://SnakeTheBite/images/cards/SnakeTheBite-{类名_snake_case}.png
    // 例如 TestCard -> SnakeTheBite-test_card.png
    public override string? CustomPortraitPath => $"res://SnakeTheBite/images/cards/SnakeTheBite-{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";

    protected SnakeTheBiteCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
