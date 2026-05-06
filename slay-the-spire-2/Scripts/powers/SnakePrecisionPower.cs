using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

// 蛇之精准能力
// 你施加的中毒和真实中毒数值增加 Amount 层
public class SnakePrecisionPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 在 Power 被施加前修改数值，避免触发两次“给予中毒后”的逻辑
    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver != Owner)
            return amount;
        if (power is not PoisonPower and not TruePoisonPower)
            return amount;
        if (target == null || !target.IsEnemy)
            return amount;

        return amount + Amount;
    }
}
