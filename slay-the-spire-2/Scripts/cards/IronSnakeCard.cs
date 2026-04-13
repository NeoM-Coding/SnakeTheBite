/// <summary>
/// 硬蛇 - 红色技能牌
/// 1费，蓝卡
/// 将两张蛇噬放入抽牌堆，获得15点格挡（升级20防）
/// </summary>
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MapleShadow.Scripts.Cards;


[Pool(typeof(IroncladCardPool))]
public class IronSnakeCard : MapleShadowCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 格挡数值：不升级15，升级20
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        new[] { new BlockVar(15m, ValueProp.Move) };

    // 悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        new[] { HoverTipFactory.Static(StaticHoverTip.Block) };

    public IronSnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放动画
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 获得格挡
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        // 创建两张蛇噬放入抽牌堆
        var snakeBites = new List<CardModel>();
        for (int i = 0; i < 2; i++)
        {
            CardModel snakeBite = CombatState!.CreateCard<SnakeBiteStatusCard>(Owner);
            snakeBites.Add(snakeBite);
        }

        // 放入抽牌堆并播放动画
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(
                snakeBites, 
                PileType.Draw, 
                addedByPlayer: true, 
                CardPilePosition.Random
            )
        );
    }

    // 升级效果
    protected override void OnUpgrade()
    {
        // 升级后增加5点格挡 (15 -> 20)
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}
