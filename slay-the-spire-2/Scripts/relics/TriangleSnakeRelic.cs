// 三角蛇 - 普通遗物，每回合打出4张蛇标签牌获得3点能量，只能打精英获得
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
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class TriangleSnakeRelic : SnakeTheBiteRelicModel
{
    // 遗物稀有度：普通（白）
    public override RelicRarity Rarity => RelicRarity.Common;

    // 不在商店出现
    public override bool IsAllowedInShops => false;

    // 显示计数器
    public override bool ShowCounter => true;

    public override int DisplayAmount => DynamicVars.Cards.IntValue - (_snakeCardsPlayedThisTurn % DynamicVars.Cards.IntValue);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4),
        new EnergyVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 本回合已打出的蛇牌数量
    [SavedProperty]
    private int _snakeCardsPlayedThisTurn;

    // 只能打精英获得
    public override bool IsAllowed(IRunState runState)
    {
        return runState.CurrentMapPoint?.PointType == MapPointType.Elite;
    }

    // 回合开始时重置计数
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;

        _snakeCardsPlayedThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    // 打出蛇牌时计数
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        if (!SnakeTheBiteCardTags.IsSnakeCard(cardPlay.Card))
            return;

        _snakeCardsPlayedThisTurn++;
        int threshold = DynamicVars.Cards.IntValue;
        if (_snakeCardsPlayedThisTurn % threshold == 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
        InvokeDisplayAmountChanged();
    }
}
