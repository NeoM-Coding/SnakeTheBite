// 血肉加身 - Power，奥斯提造成伤害时玩家与奥斯提获得等量格挡
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

public class FleshArmorPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner.Player?.Osty)
            return;

        foreach (var result in command.Results)
        {
            if (result.TotalDamage <= 0)
                continue;

            Flash();
            await CreatureCmd.GainBlock(Owner, result.TotalDamage, ValueProp.Move, null);
            if (Owner.Player?.Osty != null)
            {
                await CreatureCmd.GainBlock(Owner.Player.Osty, result.TotalDamage, ValueProp.Move, null);
            }
        }
    }
}
