// 蛇之书 - 事件遗物
// 之后每场战斗额外掉落一组蛇卡牌
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Cards;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeBookRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：事件
    public override RelicRarity Rarity => RelicRarity.Event;

    // 战斗胜利后：额外添加一组蛇卡牌奖励
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner.Creature.IsDead)
            return;

        // 从当前角色卡池获取蛇牌（权重高）
        var characterCards = CardFactory.FilterForCombat(
            Owner.Character.CardPool
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                    && c.Type != CardType.Status
                    && c.Type != CardType.Curse
                    && c is not SnakeFeastCard)
        ).ToList();

        // 从无色卡池获取蛇牌（权重低）
        var colorlessCards = CardFactory.FilterForCombat(
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                    && c.Type != CardType.Status
                    && c.Type != CardType.Curse
                    && c is not SnakeFeastCard)
        ).ToList();

        // 保底：完全找不到蛇牌时不生成奖励
        if (characterCards.Count == 0 && colorlessCards.Count == 0)
            return;

        // 构建加权候选列表：角色卡池权重3，无色卡池权重1
        var weightedCandidates = new List<(CardModel Card, float Weight)>();
        foreach (var c in characterCards)
            weightedCandidates.Add((c, 3f));
        foreach (var c in colorlessCards)
            weightedCandidates.Add((c, 1f));

        var rng = Owner.RunState.Rng.CombatCardGeneration;
        var selectedCanonical = new HashSet<CardModel>();
        var offerCards = new List<CardModel>();
        int maxDistinct = weightedCandidates.Select(x => x.Card.CanonicalInstance).Distinct().Count();

        // 加权随机抽取3张不同的卡
        while (offerCards.Count < 3 && selectedCanonical.Count < maxDistinct)
        {
            var pick = rng.WeightedNextItem(weightedCandidates, x => x.Weight);
            if (pick.Card == null)
                break;

            var canonical = pick.Card.CanonicalInstance;
            if (selectedCanonical.Add(canonical))
            {
                var card = Owner.RunState.CreateCard(canonical, Owner);
                RollUpgrade(card, rng);
                offerCards.Add(card);
            }
        }

        if (offerCards.Count == 0)
            return;

        var reward = new CardReward(offerCards, CardCreationSource.Encounter, Owner);
        room.AddExtraReward(Owner, reward);

        await Task.CompletedTask;
    }

    // 为奖励卡牌roll升级概率，逻辑与CardFactory保持一致
    private void RollUpgrade(CardModel card, Rng rng)
    {
        if (!card.IsUpgradable)
            return;

        decimal odds = 0m;
        if (card.Rarity != CardRarity.Rare)
        {
            decimal scaling = AscensionHelper.GetValueIfAscension(
                MegaCrit.Sts2.Core.Entities.Ascension.AscensionLevel.Scarcity, 0.125m, 0.25m);
            odds += (decimal)Owner.RunState.CurrentActIndex * scaling;
        }

        odds = Hook.ModifyCardRewardUpgradeOdds(Owner.RunState, Owner, card, odds);

        if ((decimal)rng.NextFloat() <= odds)
        {
            CardCmd.Upgrade(card);
        }
    }
}
