// 蛇之书 - 事件遗物
// 之后每场战斗额外掉落一组蛇卡牌
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MapleShadow.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MapleShadow.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeBookRelic : MapleShadowRelicModel
{
    // 遗物稀有度：事件
    public override RelicRarity Rarity => RelicRarity.Event;

    // 战斗胜利后：额外添加一组蛇卡牌奖励
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner.Creature.IsDead)
            return;

        var snakeCards = ModelDb.AllCards
            .Where(c => MapleShadowCardTags.IsSnakeCard(c)
                && c.Type != CardType.Status
                && c.Type != CardType.Curse)
            .ToList();

        if (snakeCards.Count == 0)
            return;

        var distinctCards = CardFactory.GetDistinctForCombat(
            Owner,
            snakeCards,
            3,
            Owner.RunState.Rng.CombatCardGeneration
        ).ToList();

        if (distinctCards.Count == 0)
            return;

        var options = new CardCreationOptions(
            new[] { Owner.Character.CardPool },
            CardCreationSource.Encounter,
            CardRarityOddsType.RegularEncounter,
            c => MapleShadowCardTags.IsSnakeCard(c) && c.Type != CardType.Status && c.Type != CardType.Curse
        );

        var reward = new CardReward(options, 3, Owner);
        room.AddExtraReward(Owner, reward);

        await Task.CompletedTask;
    }
}
