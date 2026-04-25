// 隐秘能力 - 打出蛇咬累积点数获得无实体
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeTheBite.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeMysteryPower : SnakeTheBitePowerModel
{
    // 内部数据，用于保存当前隐秘点数。
    private class Data
    {
        public int points = 0;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 显示当前隐秘点数作为能力层数。
    public override int DisplayAmount => GetInternalData<Data>().points;

    // 该能力使用实例化内部数据。
    public override bool IsInstanced => true;

    // 每次打出蛇咬获得的隐秘点数（默认2点）。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("PointsPerSnakeBite", 2m) };

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 初始化内部数据。
    protected override object InitInternalData()
    {
        return new Data();
    }

    //
    // 打出卡牌后触发：若打出的是蛇咬，则获得隐秘点数。
    // 累积满7点时，消耗7点并施加一层无实体；支持连续多次触发。
    //
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
            await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), base.Owner, 1m, base.Owner, null, false);
        }
    }

    // 判断卡牌是否属于蛇咬牌（使用统一工具类，兼容本 mod 与原版）。
    private static bool IsSnakeBiteCard(CardModel card)
    {
        return SnakeTheBiteCardTags.IsSnakeBiteCard(card);
    }
}
