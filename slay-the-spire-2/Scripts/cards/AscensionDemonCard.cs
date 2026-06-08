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
using SnakeTheBite.Scripts.Keywords;
using SnakeTheBite.Scripts.Powers;

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

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SnakeKeywords.Fated, CardKeyword.Eternal];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    // 悬浮提示：显示升魔能力效果、升魔·破与升魔·御
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AscensionDemonPower>(),
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
        if (SnakeTheBite_HasAddedTokens || Owner == null)
            return;

        // Pile 已设置但不是 Deck 时直接返回（不设置标志，因为之后可能移到 Deck）
        if (Pile != null && Pile.Type != PileType.Deck)
            return;

        // Pile 尚未设置（如 RunState.CreateCard 过程中），等待 AfterCardChangedPiles 再触发
        if (Pile == null)
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
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<AscensionDemonPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);
    }

    // 每场战斗开始时获得等于选择次数的瓦解层数
    public override async Task BeforeCombatStart()
    {
        // 防止战斗中的克隆卡牌重复触发（PopulateCombatState 会克隆 Deck 中的卡牌）
        if (CombatState != null)
            return;

        if (SnakeTheBite_SelectionCount > 0)
        {
            await PowerCmd.Apply<DisintegrationPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, SnakeTheBite_SelectionCount, Owner.Creature, this, false);
        }
    }
}
