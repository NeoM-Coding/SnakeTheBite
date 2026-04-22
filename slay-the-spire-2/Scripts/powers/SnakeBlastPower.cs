// 蛇爆术能力 - 死亡时对其他敌人造成等于层数×自身最大生命值的伤害
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeBlastPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task BeforeDeath(Creature target)
    {
        if (target != Owner)
            return;

        var otherEnemies = Owner!.CombatState.Enemies
            .Where(c => c.IsAlive && c != Owner)
            .ToList();

        if (otherEnemies.Count == 0)
            return;

        Flash();
        decimal damage = Amount * target.MaxHp;
        // dealer 传 null：
        // 1. 绕过 "dealer.IsDead" 检查（Owner 已死，Applier 会导致力量/虚弱等加成生效）
        // 2. 使 Weak/Vulnerable/Thorns 等效果不生效（它们依赖 dealer 或 IsPoweredAttack）
        // 3. 保留 Block 格挡和 Intangible 无实体限伤的正常运作
        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), otherEnemies, damage, ValueProp.Unpowered, null, null);
    }
}
