// 蛇蜕 - 手牌上限+2，第11/12张抽到的牌获得保留
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeSkinRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private int _cardsDrawnThisTurn;

    public override Task BeforeCombatStart()
    {
        _cardsDrawnThisTurn = 0;
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Creature.Side)
            _cardsDrawnThisTurn = 0;
        return Task.CompletedTask;
    }

    public override decimal ModifyHandDrawLate(Player player, decimal count)
    {
        if (player == Owner)
            return count + 2;
        return count;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (!fromHandDraw)
            return Task.CompletedTask;

        _cardsDrawnThisTurn++;
        if (_cardsDrawnThisTurn == 11 || _cardsDrawnThisTurn == 12)
        {
            if (!card.Keywords.Contains(CardKeyword.Retain))
            {
                card.AddKeyword(CardKeyword.Retain);
            }
        }
        return Task.CompletedTask;
    }
}
