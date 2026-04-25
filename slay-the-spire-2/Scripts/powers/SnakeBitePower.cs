using BaseLib.Abstracts;
using SnakeTheBite.Scripts.Cards;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

// 你好，蛇咬能力
// 在你的回合开始时，将 Amount 张非能力非状态的蛇牌放入手牌
public class SnakeBitePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 回合开始时触发
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;

        // 从所有卡牌中筛选出蛇牌，且非能力、非状态、非诅咒、非事件专属
        var snakeCards = ModelDb.AllCards
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                && c.Type != CardType.Power
                && c.Type != CardType.Status
                && c.Type != CardType.Curse
                && c is not SnakeFeastCard)
            .ToList();

        if (snakeCards.Count == 0)
            return;

        for (int i = 0; i < Amount; i++)
        {
            var selected = Owner!.Player.RunState.Rng.CombatCardSelection.NextItem(snakeCards);
            if (selected == null)
                continue;

            Flash();
            var card = CombatState.CreateCard(selected, Owner.Player!);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player);
        }
    }
}
