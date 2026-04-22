// 蓄蛇能力 - 蛇之姿态。退出蓄蛇时，随机使手中2张蛇咬牌本回合免费。
using BaseLib.Abstracts;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class ChargingSnakePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    //
    // 退出蓄蛇时，随机使手中2张蛇咬牌本回合免费。
    //
    public override async Task AfterRemoved(Creature oldOwner)
    {
        var player = oldOwner.Player;
        if (player == null)
            return;

        var hand = PileType.Hand.GetPile(player);
        var snakeBites = hand.Cards
            .Where(c => SnakeTheBiteCardTags.IsSnakeBiteCard(c))
            .ToList();

        if (snakeBites.Count == 0)
            return;

        var selected = snakeBites.TakeRandom(Math.Min(2, snakeBites.Count), player.RunState.Rng.CombatCardSelection);

        foreach (CardModel card in selected)
        {
            card.SetToFreeThisTurn();
        }
    }
}
