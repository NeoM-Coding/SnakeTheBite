// Card base class
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace MapleShadow.Scripts.Cards;

public abstract class MapleShadowCardModel : CustomCardModel
{
    /// <summary>
    /// 自动映射卡图路径，规则：res://MapleShadow/images/cards/MapleShadow-{类名_snake_case}.png
    /// 例如 TestCard -> MapleShadow-test_card.png
    /// </summary>
    public override string PortraitPath => $"res://MapleShadow/images/cards/MapleShadow-{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    protected MapleShadowCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}
