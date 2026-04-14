// Potion base class
using BaseLib.Abstracts;

namespace MapleShadow.Scripts.Potions;

public abstract class MapleShadowPotionModel : CustomPotionModel
{
    private string? _potionImagePath;

    private string PotionImagePath => _potionImagePath ??= $"res://MapleShadow/images/potions/mapleshadow-{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    /// <summary>药水图片路径。</summary>
    public override string? PackedImagePath => PotionImagePath;

    /// <summary>药水轮廓图片路径。</summary>
    public override string? PackedOutlinePath => PotionImagePath;
}
