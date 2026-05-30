// 升魔能力 - 战斗结束后选择牌并累积效果
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using SnakeTheBite.Scripts.Cards;

namespace SnakeTheBite.Scripts.Powers;

public class AscensionDemonPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override int DisplayAmount => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<AscensionDemonAttackCard>(),
        HoverTipFactory.FromCard<AscensionDemonSkillCard>(),
        HoverTipFactory.FromPower<DisintegrationPower>()
    ];

    public override LocString Description
    {
        get
        {
            var desc = base.Description;
            // 避免在 canonical 实例上访问 Owner（如 HoverTipFactory.FromPower 使用时）
            var disintegrationDesc = "";
            if (IsMutable)
            {
                var demonCard = Owner?.Player?.Deck.Cards.OfType<AscensionDemonCard>().FirstOrDefault();
                if (demonCard?.SnakeTheBite_SelectionCount > 0)
                    disintegrationDesc = $"下一场战斗开始时，获得[blue]{demonCard.SnakeTheBite_SelectionCount}[/blue]层[gold]瓦解[/gold]。";
            }
            desc.Add("DisintegrationDescription", disintegrationDesc);
            return desc;
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner?.Player == null) return;

        var demonCard = Owner.Player.Deck.Cards.OfType<AscensionDemonCard>().FirstOrDefault();
        if (demonCard == null) return;

        var deckCards = Owner.Player.Deck.Cards
            .Where(c => (c.Type == CardType.Attack || c.Type == CardType.Skill) && !c.Keywords.Contains(CardKeyword.Eternal))
            .ToList();
        if (deckCards.Count == 0) return;

        var selected = await CardSelectCmd.FromDeckGeneric(
            Owner.Player,
            new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1),
            filter: c => (c.Type == CardType.Attack || c.Type == CardType.Skill) && !c.Keywords.Contains(CardKeyword.Eternal)
        );

        var card = selected.FirstOrDefault();
        if (card == null) return;

        if (card.Type == CardType.Attack)
            demonCard.SnakeTheBite_SelectedAttackCards.Add(card.ToSerializable());
        else
            demonCard.SnakeTheBite_SelectedSkillCards.Add(card.ToSerializable());

        demonCard.SnakeTheBite_SelectionCount++;
        await CardPileCmd.RemoveFromDeck(card);
    }
}
