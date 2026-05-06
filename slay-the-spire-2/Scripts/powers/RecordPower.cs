// 记录 - 游魂卡牌的Power，记录生命值，下回合每打出1张牌回复2点生命
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeTheBite.Scripts.Powers;

public class RecordPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 标记是否还在获得Power的本回合
    [SavedProperty]
    private bool _isFirstTurn = true;

    // 已回复的生命值总和
    [SavedProperty]
    private decimal _totalHealed;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
            return;

        if (_isFirstTurn)
        {
            _isFirstTurn = false;
        }
        else
        {
            // 下回合开始，移除Power
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (_isFirstTurn)
            return;

        decimal maxHeal = Amount - _totalHealed;
        decimal heal = Math.Min(2m, maxHeal);
        if (heal <= 0)
            return;

        _totalHealed += heal;
        Flash();
        await CreatureCmd.Heal(Owner, heal);
    }
}
