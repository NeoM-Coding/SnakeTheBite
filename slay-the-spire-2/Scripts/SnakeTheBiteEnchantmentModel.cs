using BaseLib.Abstracts;

namespace SnakeTheBite.Scripts;

// 为项目内所有自定义附魔提供 CustomIconPath 支持
public abstract class SnakeTheBiteEnchantmentModel : CustomEnchantmentModel
{
    protected override string? CustomIconPath => null;
}
