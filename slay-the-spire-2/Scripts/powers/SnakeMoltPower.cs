// 蛇蜕 - 增加2张手牌上限，回合结束时保留超出原始上限的手牌（最多2张）
using BaseLib.Abstracts;
using BaseLib.Hooks;
using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeMoltPower : SnakeTheBitePowerModel, IMaxHandSizeModifier
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    // 手牌上限 +2
    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player != Owner.Player)
            return currentMaxHandSize;

        return currentMaxHandSize + 2;
    }

    // 回合结束弃牌前，给超出原始上限的手牌添加单回合 Retain
    public override Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return Task.CompletedTask;

        var hand = player.PlayerCombatState!.Hand.Cards.ToList();
        int baseLimit = MaxHandSizePatch.DefaultMaxHandSize;

        for (int i = baseLimit; i < hand.Count && i < baseLimit + 2; i++)
        {
            hand[i].GiveSingleTurnRetain();
        }

        return Task.CompletedTask;
    }

    // 敌方回合结束时减少1层持续时间
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> creatures)
    {
        if (side != CombatSide.Enemy)
            return;

        await PowerCmd.TickDownDuration(this);
    }
}
