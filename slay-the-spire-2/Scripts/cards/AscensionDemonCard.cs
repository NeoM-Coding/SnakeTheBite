// 升魔 - 命运能力牌
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class AscensionDemonCard : SnakeTheBiteCardModel
{
    // 存储被吸收的临时攻击牌列表
    [SavedProperty] public List<SerializableCard> SnakeTheBite_SelectedAttackCards { get; set; } = [];
    // 存储被吸收的临时技能牌列表
    [SavedProperty] public List<SerializableCard> SnakeTheBite_SelectedSkillCards { get; set; } = [];
    // 选择次数
    [SavedProperty] public int SnakeTheBite_SelectionCount { get; set; }
    // 防止重复添加token的标志
    [SavedProperty] public bool SnakeTheBite_HasAddedTokens { get; set; }

    public AscensionDemonCard() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
    }

    // 悬浮提示：显示命定效果、升魔·破与升魔·御
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(
            new LocString("cards", "SNAKETHEBITE-ASCENSION_DEMON_CARD.fatedTitle"),
            new LocString("cards", "SNAKETHEBITE-ASCENSION_DEMON_CARD.fatedDescription")
        ),
        HoverTipFactory.FromCard<AscensionDemonAttackCard>(),
        HoverTipFactory.FromCard<AscensionDemonSkillCard>()
    ];

    public override void AfterCreated()
    {
        base.AfterCreated();
        TryAddTokens();
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card == this)
        {
            TryAddTokens();
        }
        return Task.CompletedTask;
    }

    private void TryAddTokens()
    {
        if (SnakeTheBite_HasAddedTokens || Owner == null || Pile?.Type != PileType.Deck)
            return;

        SnakeTheBite_HasAddedTokens = true;
        var attackCard = ModelDb.Card<AscensionDemonAttackCard>().ToMutable();
        attackCard.Owner = Owner;
        var skillCard = ModelDb.Card<AscensionDemonSkillCard>().ToMutable();
        skillCard.Owner = Owner;
        Owner.Deck.AddInternal(attackCard);
        Owner.Deck.AddInternal(skillCard);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 主牌打出时不再创建临时token（token已通过命定机制加入牌组）
        await Task.CompletedTask;
    }

    // 战斗结束后选择一张攻击或技能牌，将其效果存储并移除
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        var deckCards = Owner.Deck.Cards
            .Where(c => c.Type == CardType.Attack || c.Type == CardType.Skill)
            .ToList();
        if (deckCards.Count == 0)
            return;

        var selected = await CardSelectCmd.FromDeckGeneric(
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1),
            filter: c => c.Type == CardType.Attack || c.Type == CardType.Skill
        );

        var card = selected.FirstOrDefault();
        if (card == null)
            return;

        if (card.Type == CardType.Attack)
            SnakeTheBite_SelectedAttackCards.Add(card.ToSerializable());
        else
            SnakeTheBite_SelectedSkillCards.Add(card.ToSerializable());

        SnakeTheBite_SelectionCount++;
        await CardPileCmd.RemoveFromDeck(card);
    }

    // 每场战斗开始时获得等于选择次数的瓦解层数
    public override async Task BeforeCombatStart()
    {
        // 防止战斗中的克隆卡牌重复触发（PopulateCombatState 会克隆 Deck 中的卡牌）
        if (CombatState != null)
            return;

        if (SnakeTheBite_SelectionCount > 0)
        {
            await PowerCmd.Apply<DisintegrationPower>(Owner.Creature, SnakeTheBite_SelectionCount, Owner.Creature, this);
        }
    }
}
