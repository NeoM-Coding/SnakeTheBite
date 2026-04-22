// 要蛇喽！能力 - 下回合开始时给予所有敌人中毒
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeRainPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        var enemies = Owner.CombatState?.HittableEnemies;
        if (enemies == null)
            return;

        Flash();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<PoisonPower>(enemy, Amount, Owner, null);
        }

        await PowerCmd.Remove(this);
    }
}
