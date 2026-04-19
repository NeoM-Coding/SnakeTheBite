// 开心蛇花 - 罕见遗物
// 你的蛇咬牌获得一次重放
using BaseLib.Abstracts;
using BaseLib.Utils;
using MapleShadow.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MapleShadow.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class HappySnakeFlowerRelic : MapleShadowRelicModel
{
    // 遗物稀有度：罕见（蓝）
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // 战斗开始时：给所有蛇咬牌增加1点 BaseReplayCount
    public override async Task BeforeCombatStart()
    {
        var allCards = Owner.PlayerCombatState.AllCards;
        foreach (var card in allCards)
        {
            if (MapleShadowCardTags.IsSnakeBiteCard(card))
            {
                card.BaseReplayCount += 1;
            }
        }

        await Task.CompletedTask;
    }
}
