// 蛇蜕 - 手牌上限+2，本场战斗累计抽到第11/12张牌时获得保留
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeSkinRelic : SnakeTheBiteRelicModel, IMaxHandSizeModifier
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;
    public override bool ShowCounter => true;

    // 本场战斗已抽到的牌数（1-12循环，每场战斗开始时重置）
    [SavedProperty] public int SnakeTheBite_TotalCardsDrawn { get; set; }

    public override int DisplayAmount => SnakeTheBite_TotalCardsDrawn;

    // 手牌上限 +2
    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player == Owner)
            return currentMaxHandSize + 2;
        return currentMaxHandSize;
    }

    public override Task BeforeCombatStart()
    {
        SnakeTheBite_TotalCardsDrawn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        // 只处理当前玩家的卡牌
        if (card.Owner != Owner)
            return Task.CompletedTask;

        // 只计数加入手牌的情况，避免从手牌移出时重复计数
        if (card.Pile?.Type != PileType.Hand || oldPileType == PileType.Hand)
            return Task.CompletedTask;

        SnakeTheBite_TotalCardsDrawn = SnakeTheBite_TotalCardsDrawn % 12 + 1;
        InvokeDisplayAmountChanged();

        if (SnakeTheBite_TotalCardsDrawn is 11 or 12 && !card.Keywords.Contains(CardKeyword.Retain))
            card.AddKeyword(CardKeyword.Retain);

        return Task.CompletedTask;
    }
}
