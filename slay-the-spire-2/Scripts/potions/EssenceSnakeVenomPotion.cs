// 精粹蛇毒 - 稀有药水，仅商店
// 本场战斗所有蛇牌获得重放1；若持有开心蛇花，蛇咬牌获得重放2
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MapleShadow.Scripts.Relics;
using MapleShadow.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;

namespace MapleShadow.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class EssenceSnakeVenomPotion : MapleShadowPotionModel
{
    // 稀有度 - 稀有（金）
    public override PotionRarity Rarity => PotionRarity.Rare;

    // 使用方式 - 仅战斗中使用
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型 - 自己
    public override TargetType TargetType => TargetType.Self;

    // 不在战斗奖励中生成（仅商店）
    public override bool CanBeGeneratedInCombat => false;

    // 使用时的效果逻辑
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);

        var allCards = target!.Player.PlayerCombatState.AllCards;
        bool hasHappySnakeFlower = target.Player.Relics.Any(r => r is HappySnakeFlowerRelic);

        foreach (var card in allCards)
        {
            if (!MapleShadowCardTags.IsSnakeCard(card))
                continue;

            int bonus = 1;
            if (hasHappySnakeFlower && MapleShadowCardTags.IsSnakeBiteCard(card))
                bonus = 2;

            card.BaseReplayCount += bonus;
        }

        await Task.CompletedTask;
    }
}
