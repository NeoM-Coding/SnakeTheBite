// Power base class
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;

namespace SnakeTheBite.Scripts.Powers;

public abstract class SnakeTheBitePowerModel : CustomPowerModel
{
    private string? _imagePath;

    private string ImagePath => _imagePath ??= $"res://SnakeTheBite/images/powers/{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";

    // 能力小图标路径（power atlas）。
    public override string? CustomPackedIconPath => ImagePath;

    // 能力大图标路径。
    public override string? CustomBigIconPath => ImagePath;

    //
    // 在 Description 中自动注入当前 Amount，确保 DumbHoverTip 等场景也能正确显示数值。
    //
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
