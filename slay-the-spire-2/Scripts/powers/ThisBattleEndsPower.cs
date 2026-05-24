// 此战方休 - Power，所有打击+5数值，防御+7数值，战后按基础牌打出数回血
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Powers;

public class ThisBattleEndsPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    [SavedProperty]
    private int SnakeTheBite_BasicCardsPlayed { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        var type = cardPlay.Card.GetType().Name;
        if (type.StartsWith("Strike") || type.StartsWith("Defend"))
        {
            SnakeTheBite_BasicCardsPlayed++;
        }
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.GetType().Name.StartsWith("Strike"))
        {
            return 5m;
        }
        return 0m;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != null && cardSource.GetType().Name.StartsWith("Defend"))
        {
            return 7m;
        }
        return 0m;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (SnakeTheBite_BasicCardsPlayed > 0)
        {
            await CreatureCmd.Heal(Owner, SnakeTheBite_BasicCardsPlayed);
            SnakeTheBite_BasicCardsPlayed = 0;
        }
    }
}
