// 蛇之赠礼 - 3费无色能力，战斗结束随机升级非蛇牌
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MapleShadow.Scripts.Powers;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class SnakeGiftCard : MapleShadowCardModel
{
    // 基础耗能 - 3费
    private const int energyCost = 3;
    // 卡牌类型 - 能力牌
    private const CardType type = CardType.Power;
    // 卡牌稀有度 - 稀有(金卡)
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：能力层数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SnakeGiftPower>(1m)];

    public SnakeGiftCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身蛇之赠礼能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeGiftPower>(Owner.Creature, DynamicVars["SnakeGiftPower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 费用减1（3 → 2）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
