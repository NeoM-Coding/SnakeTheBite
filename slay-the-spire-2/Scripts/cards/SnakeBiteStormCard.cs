using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MapleShadow.Scripts.Cards;

// 蛇咬风暴
[Pool(typeof(IroncladCardPool))]
public class SnakeBiteStormCard : MapleShadowCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [base.EnergyHoverTip];

    public SnakeBiteStormCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 丢弃所有手牌，换成等量的本回合1费蛇咬
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var handPile = PileType.Hand.GetPile(Owner);
        int count = handPile.Cards.Count;

        if (count == 0)
            return;

        // 复制列表避免遍历时修改集合
        var cardsToDiscard = handPile.Cards.ToList();

        // 丢弃所有当前手牌
        await CardCmd.Discard(choiceContext, cardsToDiscard);

        // 创建等量的蛇咬
        var snakeBites = new List<CardModel>();
        for (int i = 0; i < count; i++)
        {
            CardModel snakeBite = CombatState!.CreateCard<Snakebite>(Owner);

            // 升级后生成蛇咬+
            if (IsUpgraded)
            {
                CardCmd.Upgrade(snakeBite);
            }

            // 本回合费用设为 EnergyVar 数值
            snakeBite.EnergyCost.SetThisTurn(DynamicVars.Energy.IntValue);
            snakeBites.Add(snakeBite);
        }

        // 批量加入手牌
        await CardPileCmd.AddGeneratedCardsToCombat(snakeBites, PileType.Hand, addedByPlayer: true);
    }

    // 升级后的效果逻辑 - 在 OnPlay 中通过 IsUpgraded 判断生成蛇咬+
    protected override void OnUpgrade()
    {
    }
}
