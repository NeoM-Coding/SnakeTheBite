// 双截蛇 - 普通遗物，每打出10张蛇标签牌获得2点能量
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class DoubleSectionSnakeRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：普通（白）
    public override RelicRarity Rarity => RelicRarity.Common;

    // 显示计数器
    public override bool ShowCounter => true;

    public override int DisplayAmount => DynamicVars.Cards.IntValue - (_totalSnakeCardsPlayed % DynamicVars.Cards.IntValue);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(10),
        new EnergyVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 总共已打出的蛇牌数量（跨回合累计）
    [SavedProperty]
    private int _totalSnakeCardsPlayed;

    // 打出蛇牌时计数
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        if (!SnakeTheBiteCardTags.IsSnakeCard(cardPlay.Card))
            return;

        _totalSnakeCardsPlayed++;
        int threshold = DynamicVars.Cards.IntValue;
        if (_totalSnakeCardsPlayed % threshold == 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
        InvokeDisplayAmountChanged();
    }
}
