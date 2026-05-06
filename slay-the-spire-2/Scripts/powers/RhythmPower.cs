// 律动 - 荒疫卡牌的Debuff，每回合开始时受到等同于层数的伤害
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

public class RhythmPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
            return;
        if (Amount <= 0)
            return;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Move, (CardModel?)null);
    }
}
