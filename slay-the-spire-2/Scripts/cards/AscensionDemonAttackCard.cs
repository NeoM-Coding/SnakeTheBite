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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0m, ValueProp.Move)];

    private static void OnCustomizeDescriptionPost(CardModel card, Creature? target, ref string description)
    {
        if (card is not AscensionDemonAttackCard attackCard || attackCard.SourceCards?.Count == 0)
            return;

        decimal totalDamage = 0;
        foreach (var sourceCard in attackCard.SourceCards)
        {
            var c = CardModel.FromSerializable(sourceCard);
            if (c.DynamicVars.TryGetValue("Damage", out var dv))
                totalDamage += dv.BaseValue;
        }

        if (totalDamage > 0)
        {
            var loc = new LocString("cards", "SNAKETHEBITE-ASCENSION_DEMON_ATTACK_CARD.combinedDescription");
            loc.Add("TotalDamage", totalDamage);
            loc.Add("CardCount", attackCard.SourceCards.Count);
            description = loc.GetFormattedText();
        }
        else
        {
            var loc = new LocString("cards", "SNAKETHEBITE-ASCENSION_DEMON_ATTACK_CARD.absorbedDescription");
            loc.Add("CardCount", attackCard.SourceCards.Count);
            description = loc.GetFormattedText();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 如果没有吸收效果，执行默认的0伤害攻击
        if (SourceCards == null || SourceCards.Count == 0)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .Execute(choiceContext);
            return;
        }

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
