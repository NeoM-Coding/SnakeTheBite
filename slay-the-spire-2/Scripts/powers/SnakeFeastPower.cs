// 蛇宴能力 - 若敌人因中毒死亡，玩家获得其最大生命加到血上限（爪牙不生效）
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeFeastPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task BeforeDeath(Creature target)
    {
        if (target != Owner)
            return;
        // 对爪牙怪不生效
        if (target.HasPower<MinionPower>())
            return;
        // 简化为：死亡时若身上有中毒或真实中毒，即视为因中毒死亡
        if (!target.HasPower<PoisonPower>() && !target.HasPower<TruePoisonPower>())
            return;

        Flash();
        if (Applier != null)
            await CreatureCmd.GainMaxHp(Applier, target.MaxHp);
    }
}
