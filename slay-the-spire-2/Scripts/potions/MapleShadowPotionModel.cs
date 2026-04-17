// Potion base class
using BaseLib.Abstracts;

namespace MapleShadow.Scripts.Potions;

public abstract class MapleShadowPotionModel : CustomPotionModel
{
    private string? _potionImagePath;

    private string PotionImagePath => _potionImagePath ??= $"res://MapleShadow/images/potions/MapleShadow-{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    // 药水自定义图片路径。
    public override string? CustomPackedImagePath => PotionImagePath;

    // 药水自定义轮廓图片路径。
    public override string? CustomPackedOutlinePath => PotionImagePath;
}
