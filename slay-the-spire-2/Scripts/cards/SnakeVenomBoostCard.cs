using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 蛇液补充
[Pool(typeof(IroncladCardPool))]
public class SnakeVenomBoostCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 2费(蓝卡)
    private const int energyCost = 2;
    // 卡牌类型 - 能力牌
    private const CardType type = CardType.Power;
    // 卡牌稀有度 - 罕见
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：每场战斗结束后增加的中毒数值(不升级1，升级2)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SnakeVenomBoostPower>(1m)];

    // 悬停提示中显示蛇液补充能力与蛇液强化附魔的效果
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>(HoverTipFactory.FromEnchantment<SnakeVenomBoostEnchantment>()) { HoverTipFactory.FromPower<SnakeVenomBoostPower>() };

    public SnakeVenomBoostCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身蛇液补充能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeVenomBoostPower>(Owner.Creature, DynamicVars["SnakeVenomBoostPower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 增加的中毒数值加1（1 -> 2）
    protected override void OnUpgrade()
    {
        DynamicVars["SnakeVenomBoostPower"].UpgradeValueBy(1m);
    }
}
