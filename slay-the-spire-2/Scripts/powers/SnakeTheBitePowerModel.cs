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

    // 让 SmartDescription 也使用 .description 键，这样玩家身上能力栏会独立注入实际 Amount
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // Description 用于 DumbHoverTip（卡牌悬浮提示），注入 DynamicVars 与 Amount
    public override LocString Description
    {
        get
        {
            var desc = base.Description;
            DynamicVars.AddTo(desc);
            desc.Add("Amount", Amount);
            return desc;
        }
    }
}
