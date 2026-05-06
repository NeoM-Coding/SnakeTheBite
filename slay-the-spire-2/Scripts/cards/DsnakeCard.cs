// D SANKE! - 稀有技能牌，选择手牌中的蛇标签牌变化为另一张随机蛇标签牌，变化后本回合免费。升级后0费。
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class DsnakeCard : SnakeTheBiteCardModel
{
    public DsnakeCard() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 从手牌中选择一张蛇标签牌
        var selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1),
            c => SnakeTheBiteCardTags.IsSnakeCard(c),
            this
        );

        var originalCard = selectedCards.FirstOrDefault();
        if (originalCard == null)
            return;

        // 收集可变化的蛇标签牌选项（排除原卡牌自身、诅咒与状态牌）
        var snakeOptions = CardFactory.FilterForCombat(
            ModelDb.AllCards.Where(c => SnakeTheBiteCardTags.IsSnakeCard(c)
                && c.Id != originalCard.Id
                && c.Type != CardType.Curse
                && c.Type != CardType.Status)
        ).ToList();

        if (snakeOptions.Count == 0)
            return;

        // 随机选择一张作为变化目标
        var targetCanonical = snakeOptions[Owner.RunState.Rng.CombatCardGeneration.NextInt(snakeOptions.Count)];

        // 创建战斗实例
        var replacement = originalCard.CardScope!.CreateCard(targetCanonical, Owner);

        // 变化后本回合免费
        replacement.EnergyCost.SetThisTurn(0);

        // 执行变化
        await CardCmd.Transform(originalCard, replacement);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
