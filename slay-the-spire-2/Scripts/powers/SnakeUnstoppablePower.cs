using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

// 蛇不可挡能力
// 每获得一次格挡，给予随机一名敌人 Amount 层真实中毒（可叠加）
public class SnakeUnstoppablePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (amount <= 0m || creature != Owner)
            return;

        var hittableEnemies = Owner.CombatState.HittableEnemies;
        if (hittableEnemies.Count == 0)
            return;

        Flash();
        var target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
        await PowerCmd.Apply<TruePoisonPower>(target, Amount, Owner, null);
    }
}
