using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Cards;

// 加入战士卡池
[Pool(typeof(IroncladCardPool))]
public class SnakeScatterCannonCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 2;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（Self表示自己）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（每去除1层中毒，给予7层中毒）
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(7m)
    ];

    // 悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    public SnakeScatterCannonCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放动画
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        // 获取自身当前的中毒层数
        int poisonRemoved = Owner.Creature.GetPowerAmount<PoisonPower>();
        
        // 如果有中毒层数，则移除并给予敌人
        if (poisonRemoved > 0)
        {
            // 移除自身所有中毒
            await PowerCmd.Remove<PoisonPower>(Owner.Creature);
            
            // 获取可攻击的敌人列表
            var enemies = CombatState?.HittableEnemies;
            if (enemies == null || enemies.Count == 0)
                return;
            
            // 每移除1层中毒，随机给予一名敌人对应层数的中毒，执行poisonRemoved次
            int poisonPerHit = DynamicVars.Poison.IntValue;
            for (int i = 0; i < poisonRemoved; i++)
            {
                Creature? enemy = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
                if (enemy == null)
                    continue;
                
                // 播放咬击特效
                VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_bite");
                
                // 给予该敌人中毒
                await PowerCmd.Apply<PoisonPower>(enemy, poisonPerHit, Owner.Creature, this);
            }
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(3m); // 升级后增加3层（7 -> 10）
    }
}
