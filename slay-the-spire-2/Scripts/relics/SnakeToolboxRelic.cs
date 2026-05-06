// 蛇具箱 - 蓝色遗物，每场战斗开始时选择一张无色蛇标签牌加入手牌
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class SnakeToolboxRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：罕见（蓝）
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeCombatStart()
    {
        var candidates = ModelDb.CardPool<ColorlessCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                && c.Type != CardType.Status
                && c.Type != CardType.Curse);

        var cards = CardFactory.GetDistinctForCombat(Owner, candidates, 3, Owner.RunState.Rng.CombatCardGeneration).ToList();

        if (cards.Count == 0)
            return;

        var selected = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, Owner, canSkip: true);

        if (selected != null)
        {
            Flash();
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, addedByPlayer: true);
        }
    }
}
