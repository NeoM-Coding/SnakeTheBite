using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using System.Linq;

namespace MapleShadow.Scripts.Powers;

// 蛇液补充能力
// 每场战斗结束后，随机为一张蛇牌增加其中毒数值
public class SnakeVenomBoostPower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override int DisplayAmount => 0;

    // 战斗胜利后触发
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner.Player == null)
            return;

        var deck = PileType.Deck.GetPile(Owner.Player);
        var snakeCards = deck.Cards
            .Where(c => IsSnakeCard(c) && c.DynamicVars.ContainsKey("PoisonPower"))
            .ToList();

        if (snakeCards.Count == 0)
            return;

        var targetCard = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(snakeCards);
        if (targetCard == null)
            return;

        Flash();
        targetCard.DynamicVars["PoisonPower"].UpgradeValueBy(Amount);
        await Task.CompletedTask;
    }

    // 判断卡牌是否属于蛇牌
    private static bool IsSnakeCard(CardModel card)
    {
        return card.GetType().Name.Contains("Snake", StringComparison.OrdinalIgnoreCase);
    }
}
