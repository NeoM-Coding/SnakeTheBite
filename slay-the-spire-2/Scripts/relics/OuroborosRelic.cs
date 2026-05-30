// 衔尾蛇 - 每回合最后打出的牌下回合自动打出
// 参考原版 HistoryCourse 的实现方式：在 BeforePlayPhaseStart 中查询历史记录，并使用 CreateDupe() 创建副本打出
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(SharedRelicPool))]
public class OuroborosRelic : SnakeTheBiteRelicModel
{
    public override bool IsAllowedInShops => false;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    // 在玩家出牌阶段开始时触发（参考 HistoryCourse 的 BeforePlayPhaseStart）
    public override async Task BeforePlayPhaseStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在自己的回合触发，且不是第一回合（第一回合没有上一回合的记录）
        if (player != Owner || Owner.Creature.CombatState.RoundNumber == 1)
            return;

        // 从历史记录中查找上一回合（RoundNumber - 1）最后打出的一张牌
        CardModel? cardModel = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(e =>
        {
            // 必须是本角色打出的牌
            bool isOwner = e.CardPlay.Card.Owner == Owner;
            // 必须是上一回合的出牌记录
            bool isLastRound = e.RoundNumber == Owner.Creature.CombatState.RoundNumber - 1;
            // 排除 duplicate 卡牌（避免无限循环）
            bool notDupe = !e.CardPlay.Card.IsDupe;
            return isOwner && isLastRound && notDupe;
        })?.CardPlay.Card;

        if (cardModel == null)
            return;

        Flash();
        // 使用 CreateDupe() 创建副本打出（参考 HistoryCourse）
        await CardCmd.AutoPlay(choiceContext, cardModel.CreateDupe(), null);
    }
}
