// 致命蛇毒 - 普通药水
// 扔出，对一名敌人造成15点真实中毒
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class DeadlySnakeVenomPotion : SnakeTheBitePotionModel
{
    // 稀有度 - 普通（白）
    public override PotionRarity Rarity => PotionRarity.Common;

    // 使用方式 - 仅战斗中使用（可投掷）
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型 - 任意敌人
    public override TargetType TargetType => TargetType.AnyEnemy;

    // 动态变量 - 15层真实中毒
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TruePoisonPower>(15m)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TruePoisonPower>()];

    // 使用时的效果逻辑
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        await PowerCmd.Apply<TruePoisonPower>(new ThrowingPlayerChoiceContext(), target!, DynamicVars["TruePoisonPower"].BaseValue, target!, null, false);
    }
}
