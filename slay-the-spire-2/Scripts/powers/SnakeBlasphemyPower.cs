// 渎蛇能力 - 每回合开始时，获得21*Amount层中毒。
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeBlasphemyPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override LocString Description
    {
        get
        {
            var desc = base.Description;
            desc.Add("Amount", Amount);
            desc.Add("NextTurnPoison", 21m * Amount);
            return desc;
        }
    }

    // 每回合开始时给予自身21*Amount层中毒
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side)
            return;

        Flash();
        await PowerCmd.Apply<PoisonPower>(Owner, 21m * Amount, Owner, null);
    }
}
