// Potion base class
using BaseLib.Abstracts;

namespace SnakeTheBite.Scripts.Potions;

public abstract class SnakeTheBitePotionModel : CustomPotionModel
{
    private string? _potionImagePath;

    private string PotionImagePath => _potionImagePath ??= $"res://SnakeTheBite/images/potions/SnakeTheBite-{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";

    // 药水自定义图片路径。
    public override string? CustomPackedImagePath => PotionImagePath;

    // 药水自定义轮廓图片路径。
    public override string? CustomPackedOutlinePath => PotionImagePath;
}
