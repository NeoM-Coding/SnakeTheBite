// 蛇鳞 - 所有能力牌费用减2
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeScalesRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        // 只影响当前遗物拥有者的卡牌，避免多人模式下影响队友
        if (card.Owner == Owner && card.Type == CardType.Power)
        {
            modifiedCost = System.Math.Max(0, originalCost - 2);
            return true;
        }
        modifiedCost = originalCost;
        return false;
    }
}
