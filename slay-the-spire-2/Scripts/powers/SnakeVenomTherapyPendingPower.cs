// 蛇毒疗法（延迟）- 下回合结束时给予自身中毒，然后移除
using System.Collections.Generic;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeVenomTherapyPendingPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    [SavedProperty] public int SnakeTheBite_PoisonAmount { get; set; }

    // 玩家回合结束时递减计数器，到1时触发中毒并移除
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> creatures)
    {
        if (side != Owner.Side)
            return;
        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
            return;
        }
        await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), Owner, SnakeTheBite_PoisonAmount, Owner, null, false);
        await PowerCmd.Remove(this);
    }
}
