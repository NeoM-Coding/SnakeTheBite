using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Potions;

/// <summary>
/// 瓶中异蛇 - 稀有药水（金）
/// 
/// 效果：受到致命伤害时自动使用，丢弃这瓶药水，以25%生命值复活，
/// 但在复活时给予自身3层中毒。
/// </summary>
[Pool(typeof(SharedPotionPool))]
public class SnakeInABottlePotion : CustomPotionModel
{
    /// <summary>稀有度 - 稀有（金）。</summary>
    public override PotionRarity Rarity => PotionRarity.Rare;

    /// <summary>使用方式 - 自动（受到致命伤害时自动触发）。</summary>
    public override PotionUsage Usage => PotionUsage.Automatic;

    /// <summary>目标类型 - 自己。</summary>
    public override TargetType TargetType => TargetType.Self;

    /// <summary>不在战斗奖励中生成（参考瓶中精灵）。</summary>
    public override bool CanBeGeneratedInCombat => false;

    /// <summary>药水图片路径。</summary>
    public override string? PackedImagePath => $"res://MapleShadow/images/potions/{base.Id.Entry.ToLowerInvariant()}.png";

    /// <summary>药水轮廓图片路径。</summary>
    public override string? PackedOutlinePath => $"res://MapleShadow/images/potions/{base.Id.Entry.ToLowerInvariant()}.png";

    /// <summary>
    /// 使用时的效果逻辑：恢复至25%最大生命值，并施加3层中毒。
    /// </summary>
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        decimal healAmount = Math.Max((decimal)target.MaxHp * 0.25m, 1m);
        await CreatureCmd.Heal(target, healAmount);
        await PowerCmd.Apply<PoisonPower>(target, 3m, target, null);
    }

    /// <summary>
    /// 当持有者受到致命伤害时，阻止死亡（返回 false 表示不死亡）。
    /// </summary>
    public override bool ShouldDie(Creature creature)
    {
        if (creature != Owner.Creature)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 阻止死亡后触发药水效果（自动丢弃并恢复生命+中毒）。
    /// </summary>
    public override async Task AfterPreventingDeath(Creature creature)
    {
        await OnUseWrapper(new ThrowingPlayerChoiceContext(), creature);
    }
}
