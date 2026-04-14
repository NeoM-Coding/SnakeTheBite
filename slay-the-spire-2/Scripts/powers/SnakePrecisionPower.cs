using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MapleShadow.Scripts.Powers;

// 蛇之精准能力
// 给予中毒时额外给予一次同样的层数
public class SnakePrecisionPower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override int DisplayAmount => 0;

    // 防止递归触发的标志
    private bool _isApplyingBonus;

    // 当战场上任意 Power 层数变化后触发
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_isApplyingBonus)
            return;
        if (applier != Owner)
            return;
        if (amount <= 0)
            return;
        if (power is not PoisonPower)
            return;
        if (!power.Owner.IsEnemy)
            return;

        _isApplyingBonus = true;
        Flash();
        await PowerCmd.Apply<PoisonPower>(power.Owner, amount, Owner, cardSource);
        _isApplyingBonus = false;
    }
}
