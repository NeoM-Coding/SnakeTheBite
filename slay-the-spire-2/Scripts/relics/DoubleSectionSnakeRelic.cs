// 双截蛇 - 普通遗物，每打出10张蛇标签牌获得2点能量
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private bool _isActivating;

    public override int DisplayAmount
    {
        get
        {
            if (IsActivating)
                return DynamicVars.Cards.IntValue;
            return SnakeTheBite_TotalSnakeCardsPlayed;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(10),
        new EnergyVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 总共已打出的蛇牌数量（跨回合累计，取模存储）
    [SavedProperty]
    private int SnakeTheBite_TotalSnakeCardsPlayed { get; set; }

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    // 打出蛇牌时计数
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        if (!SnakeTheBiteCardTags.IsSnakeCard(cardPlay.Card))
            return;

        int threshold = DynamicVars.Cards.IntValue;
        SnakeTheBite_TotalSnakeCardsPlayed = (SnakeTheBite_TotalSnakeCardsPlayed + 1) % threshold;
        base.Status = (SnakeTheBite_TotalSnakeCardsPlayed == threshold - 1) ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();

        if (SnakeTheBite_TotalSnakeCardsPlayed == 0)
        {
            await DoActivateVisuals();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}
