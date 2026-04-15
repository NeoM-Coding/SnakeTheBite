// 蛇之赠礼能力 - 战斗结束随机升级非蛇牌
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

public class SnakeGiftPower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    /// <summary>
    /// 战斗结束后触发：从牌库中筛选出可升级且非蛇的卡牌，随机升级其中 Amount 张。
    /// 注意：Power 的 AfterCombatVictory 会在玩家 Power 被清除后调用，因此使用 AfterCombatEnd。
    /// </summary>
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Owner.Player == null)
            return;

        var deck = PileType.Deck.GetPile(base.Owner.Player);
        var upgradableCards = deck.Cards
            .Where(c => c.IsUpgradable && !IsSnakeCard(c))
            .ToList();

        int upgradeCount = Math.Min(upgradableCards.Count, (int)Amount);
        for (int i = 0; i < upgradeCount; i++)
        {
            var cardToUpgrade = base.Owner.Player.RunState.Rng.CombatCardSelection.NextItem(upgradableCards);
            if (cardToUpgrade != null)
            {
                CardCmd.Upgrade(cardToUpgrade);
                upgradableCards.Remove(cardToUpgrade);
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
