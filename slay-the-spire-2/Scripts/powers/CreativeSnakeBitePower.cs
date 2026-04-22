using BaseLib.Abstracts;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

// 创造性蛇咬能力
// 在你的回合开始时，将 Amount 张蛇能力牌放入手牌
public class CreativeSnakeBitePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 回合开始时触发
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
            return;

        // 从所有卡牌中筛选出类名含 Snake 的能力牌
        var snakePowers = ModelDb.AllCards
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c) && c.Type == CardType.Power)
            .ToList();

        if (snakePowers.Count == 0)
            return;

        for (int i = 0; i < Amount; i++)
        {
            var selected = Owner.Player!.RunState.Rng.CombatCardSelection.NextItem(snakePowers);
            if (selected == null)
                continue;

            Flash();
            var card = CombatState.CreateCard(selected, Owner.Player!);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }
    }
}
