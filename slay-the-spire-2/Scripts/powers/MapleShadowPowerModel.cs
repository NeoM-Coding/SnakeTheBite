// Power base class
using BaseLib.Abstracts;

namespace MapleShadow.Scripts.Powers;

public abstract class MapleShadowPowerModel : CustomPowerModel
{
    private string? _imagePath;

    private string ImagePath => _imagePath ??= $"res://MapleShadow/images/powers/{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    /// <summary>能力小图标路径（power atlas）。</summary>
    public override string? CustomPackedIconPath => ImagePath;

    /// <summary>能力大图标路径。</summary>
    public override string? CustomBigIconPath => ImagePath;
}
