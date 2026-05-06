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
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Cooldown", 3m)];

    [SavedProperty]
    private int _battlesSinceLastProc;

    [SavedProperty]
    private bool _shouldTakeExtraTurn;

    public override Task BeforeCombatStart()
    {
        _shouldTakeExtraTurn = false;
        return Task.CompletedTask;
    }

    public override bool ShouldDieLate(Creature creature)
    {
        if (creature == Owner.Creature && _battlesSinceLastProc >= 3)
        {
            return false;
        }
        return true;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner.Creature) return;

        _battlesSinceLastProc = 0;
        _shouldTakeExtraTurn = true;
        Flash();
        await CreatureCmd.Heal(creature, 1);
    }

    public override bool ShouldTakeExtraTurn(Player player)
    {
        if (player == Owner && _shouldTakeExtraTurn)
        {
            _shouldTakeExtraTurn = false;
            return true;
        }
        return false;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (!_shouldTakeExtraTurn)
        {
            _battlesSinceLastProc++;
        }
        return Task.CompletedTask;
    }
}
