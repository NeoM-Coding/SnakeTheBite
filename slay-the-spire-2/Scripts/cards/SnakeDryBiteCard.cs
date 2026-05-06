// 蛇干咬 - 1费红卡攻击，造成9点伤害
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class SnakeDryBiteCard : SnakeTheBiteCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度（白色 = 普通）
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌动态变量：5点伤害（升级后8点），给予1层创伤（升级后2层）。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new PowerVar<TraumaPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TraumaPower>()];

    public SnakeDryBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    //
    // 打出时的效果逻辑：对目标造成 DynamicVars.Damage 点伤害。
    //
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<TraumaPower>(cardPlay.Target, DynamicVars["TraumaPower"].BaseValue, Owner.Creature, this);
    }

    // 升级后的效果：伤害 +3（5 → 8），创伤 +1（1 → 2）。
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["TraumaPower"].UpgradeValueBy(1m);
    }
}
