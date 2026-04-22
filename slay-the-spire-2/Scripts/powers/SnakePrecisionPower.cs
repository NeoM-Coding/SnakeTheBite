using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MapleShadow.Scripts.Powers;

// 蛇之精准能力
// 给予中毒时额外给予 Amount 层中毒
public class SnakePrecisionPower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

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
        if (power is not PoisonPower and not TruePoisonPower)
            return;
        if (!power.Owner.IsEnemy)
            return;

        _isApplyingBonus = true;
        Flash();
        if (power is TruePoisonPower)
            await PowerCmd.Apply<TruePoisonPower>(power.Owner, Amount, Owner, cardSource);
        else
            await PowerCmd.Apply<PoisonPower>(power.Owner, Amount, Owner, cardSource);
        _isApplyingBonus = false;
    }
}
