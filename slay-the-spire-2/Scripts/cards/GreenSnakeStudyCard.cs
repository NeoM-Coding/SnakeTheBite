// 青蛇大学习 - 1费无色攻击，造成14点伤害，斩杀升级蛇牌
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Cards;

// 加入无色卡池
[Pool(typeof(ColorlessCardPool))]
public class GreenSnakeStudyCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 1费
    private const int energyCost = 1;
    // 卡牌类型 - 攻击牌
    private const CardType type = CardType.Attack;
    // 卡牌稀有度 - 稀有(金卡)
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 任意敌人
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 该卡牌自带"消耗"关键词。
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // 定义变量：伤害(不升级14，升级18)
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move)];

    public GreenSnakeStudyCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 造成伤害，如果斩杀敌人则随机升级牌库中的一张蛇咬牌
    // 斩杀判定参考原版 Feed.cs（狂宴）的实现
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 在造成伤害前判断目标是否满足斩杀触发条件（参考 Feed.cs）
        bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());

        // 执行伤害并获取攻击命令结果
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 若确实造成斩杀，随机升级牌库中的一张可升级蛇咬牌
        if (shouldTriggerFatal && attackCommand.Results.Any((DamageResult r) => r.WasTargetKilled))
        {
            var deck = PileType.Deck.GetPile(Owner);
            var snakeBiteCards = deck.Cards
                .Where(c => c.IsUpgradable && SnakeTheBiteCardTags.IsSnakeBiteCard(c))
                .ToList();

            if (snakeBiteCards.Count > 0)
            {
                var cardToUpgrade = Owner.RunState.Rng.CombatCardSelection.NextItem(snakeBiteCards);
                if (cardToUpgrade != null)
                {
                    CardCmd.Upgrade(cardToUpgrade);
                    // 等待升级动画（NCardUpgradeVfx）播放完毕
                    await Cmd.Wait(1.5f);
                }
            }
        }
    }



    // 升级后的效果逻辑 - 升级后增加4点伤害 (14 -> 18)
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
