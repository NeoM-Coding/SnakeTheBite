// 缺陷机器人 - Power，模拟缺陷机器人的核心效果
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

public class DefectRobotPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 每回合开始时获得3能量
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> creatures, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;

        Flash();
        await PlayerCmd.GainEnergy(3, Owner.Player!);
    }

    // 承伤逻辑：受到伤害时减少30%
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
            return 0m;
        if (amount <= 0)
            return 0m;

        return -(amount * 0.3m);
    }
}
