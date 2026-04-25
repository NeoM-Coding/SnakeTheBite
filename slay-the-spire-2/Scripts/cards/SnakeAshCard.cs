// 蛇烬 - 2费红卡攻击，给予11层中毒并消耗抽牌堆顶牌
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 加入战士卡池
[Pool(typeof(IroncladCardPool))]
public class SnakeAshCard : SnakeTheBiteCardModel
{
    // 基础耗能
    private const int energyCost = 2;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（11点中毒）
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(11m)
    ];

    // 悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    public SnakeAshCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 播放攻击动画
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        // 播放咬击特效
        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");
        // 给予目标中毒，数值来源于卡牌的中毒属性
        await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars.Poison.BaseValue, Owner.Creature, this, false);

        // 消耗抽牌堆顶的一张牌（参考Cinder.cs写法）
        await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
        CardPile drawPile = PileType.Draw.GetPile(Owner);
        CardModel topCard = drawPile.Cards.FirstOrDefault();
        if (topCard != null)
        {
            await CardCmd.Exhaust(choiceContext, topCard);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(3m); // 升级后增加3层中毒（11 -> 14）
    }
}
