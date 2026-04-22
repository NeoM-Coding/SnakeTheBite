// Relic base class
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;

namespace SnakeTheBite.Scripts.Relics;

public abstract class SnakeTheBiteRelicModel : CustomRelicModel
{
    private string? _imagePath;

    private string ImagePath => _imagePath ??= $"res://SnakeTheBite/images/relics/SnakeTheBite-{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";

    // 遗物小图标路径。
    public override string PackedIconPath => ImagePath;

    // 遗物轮廓图标路径。
    protected override string PackedIconOutlinePath => ImagePath;

    // 遗物大图标路径。
    protected override string BigIconPath => ImagePath;

    protected SnakeTheBiteRelicModel(bool autoAdd = true) : base(autoAdd)
    {
    }
}
