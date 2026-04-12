using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MapleShadow.Scripts.Cards;

// 加入无色卡池
[Pool(typeof(ColorlessCardPool))]
public class SpinningSnakebiteCard : CustomCardModel
{
    // 基础耗能 - 3费
    private const int energyCost = 3;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 稀有(金卡)
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 所有敌人
    private const TargetType targetType = TargetType.AllEnemies;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    // 基础重复次数
    private const int baseRepeat = 1;

    // 定义变量：中毒层数(不升级5，升级7)
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<PoisonPower>(5m),  // 不升级5层中毒
        new RepeatVar(baseRepeat)       // 基础重复次数
    ];

    // 保留关键词 - 参考Snakebite.cs
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        new[] { CardKeyword.Retain };

    // 悬停提示 - 显示中毒 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    // 额外触发次数计数器 - 每回合在手牌中时增加，打出后重置
    private int _extraRepeatCount;

    [SavedProperty]
    public int ExtraRepeatCount
    {
        get => _extraRepeatCount;
        set
        {
            AssertMutable();
            _extraRepeatCount = value;
            // 同步更新动态变量的显示值
            UpdateRepeatDisplay();
        }
    }

    public SpinningSnakebiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        _extraRepeatCount = 0;
    }

    // 更新 Repeat 动态变量的显示值，使其与额外次数同步
    private void UpdateRepeatDisplay()
    {
        if (DynamicVars.Repeat != null)
        {
            DynamicVars.Repeat.BaseValue = baseRepeat + _extraRepeatCount;
        }
    }

    // 打出时的效果逻辑 - 给予所有敌人中毒
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        // 获取中毒层数
        int poisonAmount = DynamicVars.Poison.IntValue;
        // 使用动态变量的当前值作为总次数
        int totalRepeatCount = DynamicVars.Repeat.IntValue;
        
        // 给所有敌人施加中毒，重复总次数
        for (int i = 0; i < totalRepeatCount; i++)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                //播放咬击动画
                VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_bite");
                await PowerCmd.Apply<PoisonPower>(enemy, poisonAmount, Owner.Creature, this);
            }
        }
        
        // 打出后重置额外次数计数器
        ExtraRepeatCount = 0;
    }

    // 每回合开始时触发的效果 - 增加下次打出时的中毒次数
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        // 只在自己的回合触发，且卡牌在手牌中时增加计数
        if (side == Owner.Creature.Side && Pile?.Type == PileType.Hand)
        {
            ExtraRepeatCount++;
        }
        return Task.CompletedTask;
    }

    // 升级后的效果逻辑 - 升级后中毒层数从5增加到7
    protected override void OnUpgrade()
    {
        // 升级后增加2层中毒 (5 -> 7)
        DynamicVars.Poison.UpgradeValueBy(2m);
    }
}
