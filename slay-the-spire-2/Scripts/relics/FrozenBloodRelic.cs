// 寒冰之血 - 每损失7点生命值，下回合获得1层无实体
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class FrozenBloodRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    [SavedProperty]
    private decimal _hpLostSinceLastTurn;

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature == Owner.Creature && delta < 0)
        {
            _hpLostSinceLastTurn += -delta;
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;

        if (_hpLostSinceLastTurn >= 7)
        {
            int stacks = (int)(_hpLostSinceLastTurn / 7);
            Flash();
            await PowerCmd.Apply<IntangiblePower>(Owner.Creature, stacks, Owner.Creature, null);
            _hpLostSinceLastTurn = 0;
        }
    }

    public override Task BeforeCombatStart()
    {
        _hpLostSinceLastTurn = 0;
        return Task.CompletedTask;
    }
}
