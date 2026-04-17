using BaseLib.Abstracts;
using MapleShadow.Scripts.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Powers;

// 吊蛇能力
// 被吊蛇攻击时，受到的伤害乘以层数
public class HangSnakePower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 修改受到的伤害倍数
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
            return 1m;
        if (cardSource is not HangSnakeCard)
            return 1m;

        return Amount;
    }
}
