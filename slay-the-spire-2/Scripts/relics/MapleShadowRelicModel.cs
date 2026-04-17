// Relic base class
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;

namespace MapleShadow.Scripts.Relics;

public abstract class MapleShadowRelicModel : CustomRelicModel
{
    private string? _imagePath;

    private string ImagePath => _imagePath ??= $"res://MapleShadow/images/relics/MapleShadow-{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    // 遗物小图标路径。
    public override string PackedIconPath => ImagePath;

    // 遗物轮廓图标路径。
    protected override string PackedIconOutlinePath => ImagePath;

    // 遗物大图标路径。
    protected override string BigIconPath => ImagePath;

    protected MapleShadowRelicModel(bool autoAdd = true) : base(autoAdd)
    {
    }
}
