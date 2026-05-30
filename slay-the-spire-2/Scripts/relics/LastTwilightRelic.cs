// 最后的黄昏 - 耶梦加得遗物，根据角色给予对应的命运卡牌对
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SnakeTheBite.Scripts.Cards;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class LastTwilightRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CardName1", "???"),
        new StringVar("CardName2", "???")
    ];

    // 在事件选项展示前调用，根据当前玩家角色更新描述中的卡牌名
    public void SetCardNamesForPlayer(Player player)
    {
        var (name1, name2) = GetCardNames(player.Character);
        ((StringVar)DynamicVars["CardName1"]).StringValue = name1;
        ((StringVar)DynamicVars["CardName2"]).StringValue = name2;
    }

    public override async Task AfterObtained()
    {
        var rng = Owner.RunState.Rng.Shuffle;
        CardModel? targetCanonical = Owner.Character switch
        {
            Ironclad => rng.NextBool() ? ModelDb.Card<AscensionDemonCard>() : ModelDb.Card<ThisBattleEndsCard>(),
            Silent => rng.NextBool() ? ModelDb.Card<PlagueCard>() : ModelDb.Card<WanderingSpiritCard>(),
            Regent => rng.NextBool() ? ModelDb.Card<ForwardForwardCard>() : ModelDb.Card<IgniteStarSeaCard>(),
            Necrobinder => rng.NextBool() ? ModelDb.Card<FleshArmorCard>() : ModelDb.Card<SufferingReincarnationCard>(),
            Defect => rng.NextBool() ? ModelDb.Card<DatabaseCard>() : ModelDb.Card<ScrappedBlueprintCard>(),
            _ => null
        };

        if (targetCanonical != null)
        {
            var mutableCard = Owner.RunState.CreateCard(targetCanonical, Owner);
            var result = await CardPileCmd.Add(mutableCard, PileType.Deck, CardPilePosition.Bottom, this);
            CardCmd.PreviewCardPileAdd(result);
        }
    }

    private static (string name1, string name2) GetCardNames(CharacterModel character)
    {
        return character switch
        {
            Ironclad => (ModelDb.Card<AscensionDemonCard>().TitleLocString.GetFormattedText(), ModelDb.Card<ThisBattleEndsCard>().TitleLocString.GetFormattedText()),
            Silent => (ModelDb.Card<PlagueCard>().TitleLocString.GetFormattedText(), ModelDb.Card<WanderingSpiritCard>().TitleLocString.GetFormattedText()),
            Regent => (ModelDb.Card<ForwardForwardCard>().TitleLocString.GetFormattedText(), ModelDb.Card<IgniteStarSeaCard>().TitleLocString.GetFormattedText()),
            Necrobinder => (ModelDb.Card<FleshArmorCard>().TitleLocString.GetFormattedText(), ModelDb.Card<SufferingReincarnationCard>().TitleLocString.GetFormattedText()),
            Defect => (ModelDb.Card<DatabaseCard>().TitleLocString.GetFormattedText(), ModelDb.Card<ScrappedBlueprintCard>().TitleLocString.GetFormattedText()),
            _ => ("???", "???")
        };
    }
}
