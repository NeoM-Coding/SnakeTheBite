// 毒囊 - 每两回合，为自己添加蛇之活力
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class VenomSacRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    // 是否正在播放激活闪光
    private bool _isActivating;

    // 回合计数，每2回合触发一次
    [SavedProperty]
    private int SnakeTheBite_TurnCount { get; set; }

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            if (_isActivating)
                return 2;
            return SnakeTheBite_TurnCount;
        }
    }

    public override Task BeforeCombatStart()
    {
        SnakeTheBite_TurnCount = 0;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> creatures, ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;

        SnakeTheBite_TurnCount++;
        if (SnakeTheBite_TurnCount >= 2)
        {
            SnakeTheBite_TurnCount = 0;
            await DoActivateVisuals();
            await PowerCmd.Apply<SnakeVitalityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 2m, Owner.Creature, null, false);
        }
        else
        {
            InvokeDisplayAmountChanged();
        }
    }

    private async Task DoActivateVisuals()
    {
        _isActivating = true;
        InvokeDisplayAmountChanged();
        Flash();
        await Cmd.Wait(0.8f);
        _isActivating = false;
        InvokeDisplayAmountChanged();
    }
}
