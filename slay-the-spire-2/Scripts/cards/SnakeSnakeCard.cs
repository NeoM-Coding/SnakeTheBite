// ！？蛇蛇？！ - 3费无色技能，将2张蛇咬放入手牌
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Cards;

// 加入无色卡池
[Pool(typeof(ColorlessCardPool))]
public class SnakeSnakeCard : SnakeTheBiteCardModel
{
    // 基础耗能
    private const int energyCost = 3;
    // 卡牌类型 - 技能牌
    private const CardType type = CardType.Skill;
    // 卡牌稀有度 - 罕见
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型 - 自己（无目标）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public SnakeSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑（参考Guards.cs和CrashLanding.cs写法）
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 创建两张蛇咬（使用泛型，参考Guards.cs）
        List<CardModel> list = new List<CardModel>();
        for (int i = 0; i < 2; i++)
        {
            // 直接泛型创建，不需要 ModelDb.GetById（参考Guards.cs）
            CardModel snakeBite = CombatState.CreateCard<Snakebite>(Owner);
            
            // 升级后生成蛇咬+（参考Guards.cs）
            if (IsUpgraded)
            {
                CardCmd.Upgrade(snakeBite);
            }
            
            // 设置本回合免费打出（参考Splash.cs）
            snakeBite.SetToFreeThisTurn();
            
            list.Add(snakeBite);
        }
        
        // 批量添加到手牌（参考CrashLanding.cs）
        await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true);
    }

    // 升级后的效果逻辑 - 升级后费用减1（3 → 2）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
