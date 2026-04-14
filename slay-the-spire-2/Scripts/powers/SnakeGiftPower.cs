using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using System.Linq;

namespace MapleShadow.Scripts.Powers;

/// <summary>
/// 蛇之赠礼 —— 战斗结束时随机升级一张非蛇的卡牌。
/// </summary>
public class SnakeGiftPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.None;

    public override int DisplayAmount => 0;

    /// <summary>
    /// 战斗胜利后触发：从牌库中筛选出可升级且非蛇的卡牌，随机升级其中一张。
    /// </summary>
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (base.Owner.Player == null)
            return;

        var deck = PileType.Deck.GetPile(base.Owner.Player);
        var upgradableCards = deck.Cards
            .Where(c => c.IsUpgradable && !IsSnakeCard(c))
            .ToList();

        if (upgradableCards.Count > 0)
        {
            var cardToUpgrade = base.Owner.Player.RunState.Rng.CombatCardSelection.NextItem(upgradableCards);
            if (cardToUpgrade != null)
            {
                CardCmd.Upgrade(cardToUpgrade);
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>判断卡牌是否属于蛇牌（类名含 Snake）。</summary>
    private static bool IsSnakeCard(CardModel card)
    {
        return card.GetType().Name.Contains("Snake", StringComparison.OrdinalIgnoreCase);
    }
}
