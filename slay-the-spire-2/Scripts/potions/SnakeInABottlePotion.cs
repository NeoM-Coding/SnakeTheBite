// 瓶中异蛇 - 稀有自动药水，致死时复活并中毒
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class SnakeInABottlePotion : MapleShadowPotionModel
{
    // 稀有度 - 稀有（金）。
    public override PotionRarity Rarity => PotionRarity.Rare;

    // 使用方式 - 自动（受到致命伤害时自动触发）。
    public override PotionUsage Usage => PotionUsage.Automatic;

    // 目标类型 - 自己。
    public override TargetType TargetType => TargetType.Self;

    // 不在战斗奖励中生成（参考瓶中精灵）。
    public override bool CanBeGeneratedInCombat => false;

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

    //
    // 使用时的效果逻辑：恢复至25%最大生命值，并施加3层中毒。
    //
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        decimal healAmount = Math.Max((decimal)target.MaxHp * 0.25m, 1m);
        await CreatureCmd.Heal(target, healAmount);
        await PowerCmd.Apply<PoisonPower>(target, 3m, target, null);
    }

    //
    // 当持有者受到致命伤害时，阻止死亡（返回 false 表示不死亡）。
    //
    public override bool ShouldDie(Creature creature)
    {
        if (creature != Owner.Creature)
        {
            return true;
        }
        return false;
    }

    //
    // 阻止死亡后触发药水效果（自动丢弃并恢复生命+中毒）。
    //
    public override async Task AfterPreventingDeath(Creature creature)
    {
        await OnUseWrapper(new ThrowingPlayerChoiceContext(), creature);
    }
}
