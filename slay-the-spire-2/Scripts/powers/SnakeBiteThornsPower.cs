using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Powers;

/// <summary>
/// 荆棘蛇咬能力（SnakeBiteThornsPower）——Buff 类型能力。
/// 
/// 效果：每当敌人攻击命中你一次，该敌人会被蛇咬，受到 7 层中毒（多段攻击每段都会触发）。
/// 能力层数仅表示剩余触发次数。
/// </summary>
public class SnakeBiteThornsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>不显示能力层数。</summary>
    public override int DisplayAmount => 0;

    /// <summary>大图标路径。若留空，CustomPowerModel 会回退到默认逻辑。</summary>
    // public override string? CustomPackedIconPath => $"res://MapleShadow/images/powers/{GetType().Name.ToLowerInvariant()}.png";

    /// <summary>
    /// 当能力持有者受到伤害前触发（参考荆棘能力的实现）。
    /// 
    /// 过滤条件：
    /// 1. 受击目标是能力持有者本人；
    /// 2. 攻击者存在且是敌人；
    /// 3. 该次伤害来自 Powered Attack（普通攻击/攻击意图）。
    /// 
    /// 满足条件后，对攻击者施加 7 层中毒，并播放受击与咬击动画特效。
    /// 使用 BeforeDamageReceived 可确保多段攻击的每一段都会独立触发。
    /// </summary>
    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || dealer == null || !dealer.IsEnemy)
            return;

        bool isPoweredAttack = props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
        if (!isPoweredAttack && cardSource is not Omnislice)
            return;

        Flash();
        await CreatureCmd.TriggerAnim(dealer, "Hit", 0f);
        VfxCmd.PlayOnCreatureCenter(dealer, "vfx/vfx_bite");
        await PowerCmd.Apply<PoisonPower>(dealer, 7m, base.Owner, null);
    }
}
