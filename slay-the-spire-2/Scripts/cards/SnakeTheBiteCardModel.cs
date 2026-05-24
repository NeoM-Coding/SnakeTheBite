// Card base class
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace SnakeTheBite.Scripts.Cards;

public abstract class SnakeTheBiteCardModel : CustomCardModel
{
    // 自动映射卡图路径，规则：res://SnakeTheBite/images/cards/SnakeTheBite-{类名_snake_case}.png
    // 例如 TestCard -> SnakeTheBite-test_card.png
    public override string? CustomPortraitPath
    {
        get
        {
            var path = $"res://SnakeTheBite/images/cards/SnakeTheBite-{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";
            // 资源不存在时返回 null，避免 BaseLib 的 CustomCardPortraitPath 补丁在加载缺失资源时 NullReferenceException
            return ResourceLoader.Exists(path) ? path : null;
        }
    }

    protected SnakeTheBiteCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true, bool autoAdd = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary, autoAdd)
    {
    }
}
