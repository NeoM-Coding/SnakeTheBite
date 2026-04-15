using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using System.Linq;

namespace MapleShadow.Scripts.Powers;

// 蛇液补充能力
// 每场战斗结束后，随机为 Amount 张蛇牌各增加 1 点中毒数值
public class SnakeVenomBoostPower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;

    // 强制使用 description 作为 smartDescription，以便 HoverTips 中注入 Amount 变量
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 1m)];

    // 战斗结束后触发
    // 注意：Power 的 AfterCombatVictory 会在玩家 Power 被清除后调用，因此使用 AfterCombatEnd。
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null)
            return;

        var deck = PileType.Deck.GetPile(Owner.Player);
        var snakeCards = deck.Cards
            .Where(c => IsSnakeCard(c) && c.DynamicVars.ContainsKey("PoisonPower"))
            .ToList();

        if (snakeCards.Count == 0)
            return;

        int boostCount = Math.Min(snakeCards.Count, (int)Amount);
        for (int i = 0; i < boostCount; i++)
        {
            var targetCard = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(snakeCards);
            if (targetCard == null)
                continue;

            Flash();
            targetCard.DynamicVars["PoisonPower"].UpgradeValueBy(1m);
            snakeCards.Remove(targetCard);

            // 播放升级动画（仅视觉效果，不真正升级卡牌）
            NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardUpgradeVfx.Create(targetCard));
        }

        await Task.CompletedTask;
    }

    // 判断卡牌是否属于蛇牌
    private static bool IsSnakeCard(CardModel card)
    {
        return card.GetType().Name.Contains("Snake", StringComparison.OrdinalIgnoreCase);
    }
}
