// 耶梦加得 - 先古之民
using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using SnakeTheBite.Scripts.Cards;
using SnakeTheBite.Scripts.Relics;

namespace SnakeTheBite.Scripts.Ancients;

public class JormungandrAncient : CustomAncientModel
{
    public override Color ButtonColor => new Color(0.05f, 0.25f, 0.1f, 0.6f);

    public override Color DialogueColor => new Color("1A3D2E");

    public override string? CustomScenePath => "res://SnakeTheBite/images/ancients/jormungandr_ancient_bg.tscn";

    public override string? CustomMapIconPath => "res://SnakeTheBite/images/ancients/jormungandr_ancient.png";

    public override string? CustomMapIconOutlinePath => "res://SnakeTheBite/images/ancients/jormungandr_ancient_outline.png";

    public override string? CustomRunHistoryIconOutlinePath => "res://SnakeTheBite/images/ancients/jormungandr_ancient_outline.png";

    public override string? CustomRunHistoryIconPath => "res://SnakeTheBite/images/ancients/jormungandr_ancient.png";

    protected override OptionPools MakeOptionPools => new(
        MakePool(
            AncientOption<OuroborosRelic>(),
            AncientOption<VenomSacRelic>(),
            AncientOption<LastTwilightRelic>()
        ),
        MakePool(
            AncientOption<SnakeSkinRelic>(),
            AncientOption<WitheredWoodRelic>(),
            AncientOption<SnakeHeartRelic>(),
            AncientOption<SnakeScalesRelic>()
        ),
        MakePool(
            AncientOption<VenomFangRelic>(),
            AncientOption<SnakeIdolRelic>(),
            AncientOption<FrozenBloodRelic>()
        )
    );

    public override bool IsValidForAct(ActModel act) => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = OptionPools.Roll(Rng);
        return options.Select(option =>
        {
            var relic = option.ModelForOption;
            if (relic is LastTwilightRelic lastTwilight)
            {
                var player = Owner;
                if (player == null) return RelicOption(relic);
                lastTwilight.SetCardNamesForPlayer(player);
                var eventOption = RelicOption(relic);

                // 为最后的黄昏选项添加对应两张命运牌的悬浮提示
                var cardModels = GetCardsForCharacter(player.Character).ToList();
                if (cardModels.Count > 0)
                {
                    var hoverTips = eventOption.HoverTips.ToList();
                    foreach (var card in cardModels)
                    {
                        hoverTips.Add(HoverTipFactory.FromCard(card));
                    }
                    eventOption.HoverTips = hoverTips;
                }

                return eventOption;
            }
            return RelicOption(relic);
        }).ToList();
    }

    private static IEnumerable<CardModel> GetCardsForCharacter(CharacterModel character)
    {
        return character switch
        {
            Ironclad => new CardModel[] { ModelDb.Card<AscensionDemonCard>(), ModelDb.Card<ThisBattleEndsCard>() },
            Silent => new CardModel[] { ModelDb.Card<PlagueCard>(), ModelDb.Card<WanderingSpiritCard>() },
            Regent => new CardModel[] { ModelDb.Card<ForwardForwardCard>(), ModelDb.Card<IgniteStarSeaCard>() },
            Necrobinder => new CardModel[] { ModelDb.Card<FleshArmorCard>(), ModelDb.Card<SufferingReincarnationCard>() },
            Defect => new CardModel[] { ModelDb.Card<DatabaseCard>(), ModelDb.Card<ScrappedBlueprintCard>() },
            _ => System.Array.Empty<CardModel>()
        };
    }
}
