// Power base class
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;

namespace MapleShadow.Scripts.Powers;

public abstract class MapleShadowPowerModel : CustomPowerModel
{
    private string? _imagePath;

    private string ImagePath => _imagePath ??= $"res://MapleShadow/images/powers/{MapleShadowModelHelper.ToSnakeCase(GetType().Name)}.png";

    /// <summary>能力小图标路径（power atlas）。</summary>
    public override string? CustomPackedIconPath => ImagePath;

    /// <summary>能力大图标路径。</summary>
    public override string? CustomBigIconPath => ImagePath;

    /// <summary>
    /// 在 Description 中自动注入当前 Amount，确保 DumbHoverTip 等场景也能正确显示数值。
    /// </summary>
    public override LocString Description
    {
        get
        {
            var desc = base.Description;
            desc.Add("Amount", Amount);
            return desc;
        }
    }
}
