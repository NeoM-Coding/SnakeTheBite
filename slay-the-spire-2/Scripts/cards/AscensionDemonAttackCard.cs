// 升魔·破 - 升魔生成的临时攻击牌
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(TokenCardPool))]
public class AscensionDemonAttackCard : SnakeTheBiteCardModel
{
    // 存储被吸收的原始攻击牌数据列表
    public List<SerializableCard>? SourceCards { get; set; }

    static AscensionDemonAttackCard()
    {
        DescriptionOverrides.CustomizeDescriptionPost += OnCustomizeDescriptionPost;
    }

    public AscensionDemonAttackCard() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, shouldShowInCardLibrary: false)
    {
    }

    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal];

    // 不定义伤害变量，所有效果来自被吸收牌的反射调用

    private static void OnCustomizeDescriptionPost(CardModel card, Creature? target, ref string description)
    {
        if (card is not AscensionDemonAttackCard attackCard || attackCard.SourceCards == null || attackCard.SourceCards.Count == 0)
            return;

        var loc = new LocString("cards", "SNAKETHEBITE-ASCENSION_DEMON_ATTACK_CARD.absorbedDescription");
        loc.Add("CardCount", attackCard.SourceCards.Count);
        description = loc.GetFormattedText();
    }

    public override Task BeforeCombatStart()
    {
        if (Owner != null)
        {
            var demonCard = Owner.Deck.Cards.OfType<AscensionDemonCard>().FirstOrDefault();
            if (demonCard != null)
            {
                SourceCards = demonCard.SnakeTheBite_SelectedAttackCards;
            }
        }
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 如果没有吸收效果，无事发生
        if (SourceCards == null || SourceCards.Count == 0)
            return;

        // 依次执行所有被吸收的攻击牌效果
        foreach (var sourceCard in SourceCards)
        {
            var cardModel = CardModel.FromSerializable(sourceCard);
            cardModel.Owner = Owner;

            var target = ResolveTarget(cardModel, cardPlay);
            var newCardPlay = new CardPlay
            {
                Card = cardModel,
                Target = target,
                ResultPile = PileType.Discard,
                Resources = new ResourceInfo { EnergySpent = 0, EnergyValue = 0, StarsSpent = 0, StarValue = 0 },
                IsAutoPlay = true,
                PlayIndex = 0,
                PlayCount = 1,
            };

            var method = typeof(CardModel).GetMethod("OnPlay",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                var result = method.Invoke(cardModel, new object[] { choiceContext, newCardPlay });
                if (result is Task task)
                    await task;
            }
        }
    }

    private static Creature? ResolveTarget(CardModel sourceCardModel, CardPlay cardPlay)
    {
        return sourceCardModel.TargetType switch
        {
            TargetType.Self => sourceCardModel.Owner?.Creature,
            TargetType.AnyEnemy => cardPlay.Target,
            _ => cardPlay.Target,
        };
    }
}
