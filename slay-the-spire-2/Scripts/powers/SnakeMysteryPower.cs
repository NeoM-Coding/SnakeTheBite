using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Linq;

namespace MapleShadow.Scripts.Powers;

/// <summary>
/// 隐秘 —— 蛇之神秘对应的能力。
/// 
/// 效果：每当你打出蛇咬时，获得若干点隐秘点数；当点数到达7时，
/// 消耗7点并获得一层无实体。
/// </summary>
public class SnakeMysteryPower : CustomPowerModel
{
    /// <summary>内部数据，用于保存当前隐秘点数。</summary>
    private class Data
    {
        public int points = 0;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>显示当前隐秘点数作为能力层数。</summary>
    public override int DisplayAmount => GetInternalData<Data>().points;

    /// <summary>该能力使用实例化内部数据。</summary>
    public override bool IsInstanced => true;

    /// <summary>每次打出蛇咬获得的隐秘点数（默认2点）。</summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("PointsPerSnakeBite", 2m) };

    /// <summary>初始化内部数据。</summary>
    protected override object InitInternalData()
    {
        return new Data();
    }

    /// <summary>
    /// 打出卡牌后触发：若打出的是蛇咬，则获得隐秘点数。
    /// 累积满7点时，消耗7点并施加一层无实体；支持连续多次触发。
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
            return;

        if (!IsSnakeBiteCard(cardPlay.Card))
            return;

        Data data = GetInternalData<Data>();
        int pointsPerPlay = DynamicVars["PointsPerSnakeBite"].IntValue;
        data.points += pointsPerPlay;
        InvokeDisplayAmountChanged();

        while (data.points >= 7)
        {
            data.points -= 7;
            InvokeDisplayAmountChanged();
            Flash();
            await PowerCmd.Apply<IntangiblePower>(base.Owner, 1m, base.Owner, null);
        }
    }

    /// <summary>判断卡牌是否属于蛇咬牌（类名含 Snakebite）。</summary>
    private static bool IsSnakeBiteCard(CardModel card)
    {
        return card.GetType().Name.Contains("Snakebite", StringComparison.OrdinalIgnoreCase);
    }
}
