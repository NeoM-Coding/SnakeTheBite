// 神秘蛇教集会所 - 普通事件，选项一获得蛇宴+3张蛇不咬，选项二获得50-100金币
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using SnakeTheBite.Scripts.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace SnakeTheBite.Scripts.Events;

public class MysteriousSnakeCultEvent : CustomEventModel
{
    // 仅出现在第二幕（Act 2）
    public override bool IsAllowed(IRunState runState) => runState.CurrentActIndex == 1;

    public override string? CustomInitialPortraitPath => "res://SnakeTheBite/images/events/snakethebite-mysterious_snake_cult_event.png";

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new List<EventOption>
        {
            Option(JoinCult, "INITIAL", HoverTipFactory.FromCard<SnakeFeastCard>(), HoverTipFactory.FromCard<SnakeNoBiteCard>(), HoverTipFactory.FromCard<CurseOfSnakeCard>()),
            Option(TakeMoneyAndRun)
        };
    }

    private async Task JoinCult()
    {
        // 获得蛇宴
        CardModel snakeFeast = Owner!.RunState.CreateCard<SnakeFeastCard>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(snakeFeast, PileType.Deck), 2f);

        // 获得三张蛇不咬
        for (int i = 0; i < 5; i++)
        {
            CardModel snakeNoBite = Owner.RunState.CreateCard<SnakeNoBiteCard>(Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(snakeNoBite, PileType.Deck), 2f);
        }

        // 获得一张蛇之诅咒
        CardModel curseOfSnake = Owner.RunState.CreateCard<CurseOfSnakeCard>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(curseOfSnake, PileType.Deck), 2f);

        SetEventFinished(PageDescription("JOIN_CULT"));
    }

    private async Task TakeMoneyAndRun()
    {
        int gold = Rng.NextInt(51) + 50;
        await PlayerCmd.GainGold(gold, Owner);
        SetEventFinished(PageDescription("TAKE_MONEY_AND_RUN"));
    }
}
