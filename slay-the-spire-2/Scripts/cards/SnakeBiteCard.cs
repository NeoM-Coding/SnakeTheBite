using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MapleShadow.Scripts.Powers;

namespace MapleShadow.Scripts.Cards;

// 你好，蛇咬
[Pool(typeof(IroncladCardPool))]
public class SnakeBiteCard : MapleShadowCardModel
{
    // 基础耗能 - 1费(白卡)
    private const int energyCost = 1;
    // 卡牌类型 - 能力牌
    private const CardType type = CardType.Power;
    // 卡牌稀有度 - 普通
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型 - 自己
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：能力层数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SnakeBitePower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SnakeBitePower>()];

    public SnakeBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予自身你好，蛇咬能力
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeBitePower>(Owner.Creature, DynamicVars["SnakeBitePower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果逻辑 - 获得固有
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
