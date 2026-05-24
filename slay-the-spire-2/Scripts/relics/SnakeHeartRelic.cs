// 蛇之心 - 每3场战斗首次死亡时以1HP复活并获得额外回合
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeHeartRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;
    public override bool ShowCounter => true;
    public override int DisplayAmount => SnakeTheBite_BattlesSinceLastProc;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Cooldown", 3m)];

    [SavedProperty]
    private int SnakeTheBite_BattlesSinceLastProc { get; set; }

    [SavedProperty]
    private bool SnakeTheBite_ExtraTurnPending { get; set; }

    public override Task AfterObtained()
    {
        SnakeTheBite_BattlesSinceLastProc = 1;
        UpdateStatus();
        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        SnakeTheBite_ExtraTurnPending = false;
        return Task.CompletedTask;
    }

    public override bool ShouldDieLate(Creature creature)
    {
        if (creature == Owner.Creature && SnakeTheBite_BattlesSinceLastProc >= 3)
        {
            return false;
        }
        return true;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner.Creature) return;

        SnakeTheBite_BattlesSinceLastProc = 1;
        SnakeTheBite_ExtraTurnPending = true;
        UpdateStatus();
        Flash();
        // 如果玩家受到过量伤害（CurrentHp 为负数），仅治疗1点无法使其复活。
        // 计算所需治疗量，确保复活后 CurrentHp 至少为1。
        decimal healAmount = Math.Max(1m, 1m - creature.CurrentHp);
        await CreatureCmd.Heal(creature, healAmount);
    }

    public override bool ShouldTakeExtraTurn(Player player)
    {
        if (player == Owner && SnakeTheBite_ExtraTurnPending)
        {
            SnakeTheBite_ExtraTurnPending = false;
            return true;
        }
        return false;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (!SnakeTheBite_ExtraTurnPending)
        {
            SnakeTheBite_BattlesSinceLastProc++;
            InvokeDisplayAmountChanged();
            UpdateStatus();
        }
        return Task.CompletedTask;
    }

    private void UpdateStatus()
    {
        Status = SnakeTheBite_BattlesSinceLastProc >= 3 ? RelicStatus.Active : RelicStatus.Normal;
    }
}
