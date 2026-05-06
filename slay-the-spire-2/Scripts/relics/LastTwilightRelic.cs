// 最后的黄昏 - 耶梦加得遗物，每个角色获得一张特殊卡牌
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SnakeTheBite.Scripts.Cards;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class LastTwilightRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterObtained()
    {
        var specialCards = ModelDb.AllCards
            .Where(c => c is ThisBattleEndsCard or PlagueCard
                or WanderingSpiritCard or ForwardForwardCard or IgniteStarSeaCard
                or FleshArmorCard or SufferingReincarnationCard or DatabaseCard
                or ScrappedBlueprintCard or DefectRobotClawCard or DefectRobotArmorCard
                or DefectRobotCoreCard or DefectRobotCard)
            .ToList();

        if (specialCards.Count == 0)
            return;

        var rng = Owner.RunState.Rng;
        foreach (var player in Owner.RunState.Players)
        {
            var targetCanonical = specialCards[rng.Shuffle.NextInt(specialCards.Count)];
            var mutableCard = Owner.RunState.CreateCard(targetCanonical, player);
            player.Deck.AddInternal(mutableCard, -1, silent: true);
        }
    }
}
