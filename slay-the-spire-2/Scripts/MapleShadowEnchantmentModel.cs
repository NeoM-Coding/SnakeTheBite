using BaseLib.Abstracts;

namespace MapleShadow.Scripts;

// 为项目内所有自定义附魔提供 CustomIconPath 支持
public abstract class MapleShadowEnchantmentModel : CustomEnchantmentModel
{
    protected override string? CustomIconPath => null;
}
