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
    // 卡牌悬浮提示（DumbHoverTip）统一显示"对应层数"，能力悬浮（SmartDescription）保持实际层数。
    //
    public override LocString Description
    {
        get
        {
            var desc = base.Description;
            string rawText = desc.GetRawText();
            if (rawText.Contains("{Amount}"))
            {
                rawText = rawText.Replace("{Amount}", "对应层数");
                return new LocString(desc.LocTable, rawText);
            }
            return desc;
        }
    }
}
