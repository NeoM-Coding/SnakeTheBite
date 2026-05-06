// 灾厄之蛇 - 2费红卡技能，给予7层灾厄，根据玩家中毒额外给予灾厄
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 加入战士卡池
[Pool(typeof(IroncladCardPool))]
public class SnakeCalamityCard : SnakeTheBiteCardModel
{
    // 基础耗能
    private const int energyCost = 2;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（7点灾厄，额外5点灾厄）
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DoomPower>(7m),
        new DynamicVar("ExtraDoom", 5m)
    ];

    // 保留关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Retain };

    // 悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<TruePoisonPower>()
    ];

    public SnakeCalamityCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 播放动画
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        // 给予目标灾厄，数值来源于卡牌的灾厄属性
        await PowerCmd.Apply<DoomPower>(cardPlay.Target, DynamicVars.Doom.BaseValue, Owner.Creature, this);

        // 额外灾厄：你身上每有一层中毒或真实中毒，额外给予灾厄
        int poisonAmount = (int)(Owner.Creature.GetPower<PoisonPower>()?.Amount ?? 0);
        int truePoisonAmount = (int)(Owner.Creature.GetPower<TruePoisonPower>()?.Amount ?? 0);
        int totalPoison = poisonAmount + truePoisonAmount;

        if (totalPoison > 0)
        {
            decimal extraDoom = DynamicVars["ExtraDoom"].BaseValue * totalPoison;
            await PowerCmd.Apply<DoomPower>(cardPlay.Target, extraDoom, Owner.Creature, this);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Doom.UpgradeValueBy(3m); // 升级后增加3层灾厄（7 -> 10）
        DynamicVars["ExtraDoom"].UpgradeValueBy(2m); // 额外灾厄 5 -> 7
    }
}
