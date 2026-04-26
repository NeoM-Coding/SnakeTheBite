using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

// 蛇之活力
// 下次给予中毒时额外给予对应层数层中毒/真实中毒
public class SnakeVitalityPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 防止递归移除的标志
    private bool _isRemoving;

    // 修改给予的中毒层数，额外增加 Amount 层
    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver != Owner)
            return amount;
        if (power is not PoisonPower and not TruePoisonPower)
            return amount;
        if (Amount <= 0)
            return amount;

        return amount + Amount;
    }

    // 中毒施加完成后，移除本能力
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_isRemoving)
            return;
        if (applier != Owner)
            return;
        if (amount <= 0)
            return;
        if (power is not PoisonPower and not TruePoisonPower)
            return;

        _isRemoving = true;
        Flash();
        await PowerCmd.Remove(this);
        _isRemoving = false;
    }
}
