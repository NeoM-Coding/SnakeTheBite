// 蛇，咬！ - 2费无色技能，立刻触发敌人中毒一次
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class SnakeBiteNowCard : SnakeTheBiteCardModel
{
    // 基础耗能
    private const int energyCost = 2;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度（蓝色 = 罕见）
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 该卡牌自带"消耗"关键词。
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public SnakeBiteNowCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    //
    // 打出时的效果逻辑。
    //
    // 获取目标当前的中毒层数，并立即对其造成同等数值的不可格挡、不受力量/敏捷影响的伤害，
    // 随后减少目标身上 1 层中毒。
    //
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        int poisonAmount = cardPlay.Target.GetPowerAmount<PoisonPower>();
        int truePoisonAmount = cardPlay.Target.GetPowerAmount<TruePoisonPower>();
        int totalPoison = poisonAmount + truePoisonAmount;

        if (totalPoison > 0)
        {
            await CreatureCmd.Damage(
                choiceContext,
                cardPlay.Target,
                totalPoison,
                ValueProp.Unblockable | ValueProp.Unpowered,
                Owner.Creature,
                this
            );

            PoisonPower? poisonPower = cardPlay.Target.GetPower<PoisonPower>();
            if (poisonPower != null)
            {
                await PowerCmd.Decrement(poisonPower);
            }
        }
    }

    // 升级后的效果：费用减1（2 → 1）。
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
