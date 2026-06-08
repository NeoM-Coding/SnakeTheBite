// 衔尾蛇 - 每回合最后打出的牌下回合自动打出
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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

    // 上一场战斗最后打出的非复制卡牌
    private CardModel? _lastCardPlayed;

    public override Task BeforeCombatStart()
    {
        _lastCardPlayed = null;
        return Task.CompletedTask;
    }

    // 记录本回合最后打出的非复制卡牌
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;
        if (cardPlay.Card.IsDupe)
            return Task.CompletedTask;

        _lastCardPlayed = cardPlay.Card;
        return Task.CompletedTask;
    }

    // 在玩家出牌阶段开始时自动打出上一回合最后打出的牌
    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在自己的回合触发，且不是第一回合（第一回合没有上一回合的记录）
        if (player != Owner || Owner.Creature.CombatState.RoundNumber == 1)
            return;

        if (_lastCardPlayed == null)
            return;

        Flash();
        // 使用 CreateDupe() 创建副本打出（参考 HistoryCourse）
        await CardCmd.AutoPlay(choiceContext, _lastCardPlayed.CreateDupe(), null);
    }
}
