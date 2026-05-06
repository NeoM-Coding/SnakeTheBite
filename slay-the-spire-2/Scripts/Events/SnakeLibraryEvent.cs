// 蛇之图书馆事件
using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using SnakeTheBite.Scripts.Cards;
using SnakeTheBite.Scripts.Relics;
using SnakeTheBite.Scripts.Utils;

namespace SnakeTheBite.Scripts.Events;

public class SnakeLibraryEvent : CustomEventModel
{
    // 不出现在正常事件池中，仅通过 Harmony 补丁强制触发
    public override bool IsAllowed(IRunState runState) => false;

    // 事件立绘路径
    public override string? CustomInitialPortraitPath => $"res://SnakeTheBite/images/events/SnakeTheBite-{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new List<EventOption>
        {
            Option(ReadBook),
            Option(TakeRelic, HoverTipFactory.FromRelic<SnakeBookRelic>()),
            Option(TakeMedusaEye, HoverTipFactory.FromRelic<MedusaEyeRelic>())
        };
    }

    private async Task ReadBook()
    {
        // 从所有已注册卡牌中筛选蛇牌，排除状态、诅咒、基础与古代稀有度
        var snakeCards = ModelDb.AllCards
            .Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                && c.Type != CardType.Status
                && c.Type != CardType.Curse
                && c.Rarity != CardRarity.Basic
                && c.Rarity != CardRarity.Ancient
                && c is not SnakeFeastCard)
            .ToList();

        // 根据单/多人模式过滤卡牌（排除仅限另一种模式的卡牌）
        var constraint = Owner!.RunState.CardMultiplayerConstraint;
        switch (constraint)
        {
            case CardMultiplayerConstraint.MultiplayerOnly:
                snakeCards.RemoveAll(c => c.MultiplayerConstraint == CardMultiplayerConstraint.SingleplayerOnly);
                break;
            case CardMultiplayerConstraint.SingleplayerOnly:
                snakeCards.RemoveAll(c => c.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly);
                break;
        }

        if (snakeCards.Count == 0)
        {
            SetEventFinished(PageDescription("READ_BOOK"));
            return;
        }

        // 随机抽取20张（若总数不足则取全部）
        int count = System.Math.Min(20, snakeCards.Count);
        var selected = snakeCards
            .OrderBy(_ => Rng.NextInt(int.MaxValue))
            .Take(count)
            .ToList();

        List<CardCreationResult> cards = selected
            .Select(c => Owner!.RunState.CreateCard(c, Owner))
            .Select(c => new CardCreationResult(c))
            .ToList();

        // 弹出网格选择界面，让玩家从中选择3张
        var selectedCards = await CardSelectCmd.FromSimpleGridForRewards(
            new BlockingPlayerChoiceContext(),
            cards,
            Owner!,
            new CardSelectorPrefs(PageDescription("READ_BOOK_SELECTION_PROMPT"), 3)
        );

        foreach (CardModel card in selectedCards)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        }

        SetEventFinished(PageDescription("READ_BOOK"));
    }

    private async Task TakeRelic()
    {
        await RelicCmd.Obtain<SnakeBookRelic>(Owner!);
        SetEventFinished(PageDescription("TAKE_RELIC"));
    }

    private async Task TakeMedusaEye()
    {
        await RelicCmd.Obtain<MedusaEyeRelic>(Owner!);
        SetEventFinished(PageDescription("TAKE_MEDUSA_EYE"));
    }
}
