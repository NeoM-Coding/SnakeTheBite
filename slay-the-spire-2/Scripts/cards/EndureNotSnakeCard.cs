// 忍住不蛇 - 4费无色技能，保留减费，给予17层中毒
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
using System;

namespace SnakeTheBite.Scripts.Cards;

// 加入无色卡池
[Pool(typeof(ColorlessCardPool))]
public class EndureNotSnakeCard : SnakeTheBiteCardModel
{
    // 基础耗能 - 4费
    private const int energyCost = 4;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 稀有(蓝卡)
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型 - 任意敌人
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 定义变量：中毒层数(不升级17，升级22)
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<PoisonPower>(17m)  // 不升级17层中毒
    ];

    // 保留关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
        new[] { CardKeyword.Retain };

    // 悬停提示 - 显示中毒 power 的提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        new[] { HoverTipFactory.FromPower<PoisonPower>() };

    // 费用减少计数器 - 每回合在手牌中时增加，本场战斗耗能减1
    private int _costReductionCount;

    [SavedProperty]
    public int CostReductionCount
    {
        get => _costReductionCount;
        set
        {
            AssertMutable();
            _costReductionCount = value;
        }
    }

    public EndureNotSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        _costReductionCount = 0;
    }

    // 打出时的效果逻辑 - 给予敌人中毒
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        // 获取中毒层数
        int poisonAmount = DynamicVars.Poison.IntValue;
        
        // 参考Snakebite.cs，播放咬击动画
        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");
        
        // 给予目标中毒
        await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, poisonAmount, Owner.Creature, this, false);
    }

    // 每回合开始时触发的效果 - 被保留时本场战斗耗能-1
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
        // 只在自己的回合触发，且卡牌在手牌中时（被保留）减少费用
        if (side == Owner.Creature.Side && Pile?.Type == PileType.Hand)
        {
            CostReductionCount++;
            // 本场战斗耗能-1
            EnergyCost.AddThisCombat(-1);
        }
        return Task.CompletedTask;
    }

    // 升级后的效果逻辑 - 升级后中毒层数从17增加到22
    protected override void OnUpgrade()
    {
        // 升级后增加5层中毒 (17 -> 22)
        DynamicVars.Poison.UpgradeValueBy(5m);
    }
}
