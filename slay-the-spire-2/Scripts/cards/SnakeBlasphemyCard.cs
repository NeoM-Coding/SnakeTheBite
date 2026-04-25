// 渎蛇 - 1费稀有技能，消耗，进入神蛇姿态，之后每回合开始时自身获得21层中毒。升级后获得保留。
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class SnakeBlasphemyCard : SnakeTheBiteCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SnakeBlasphemyPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromPower<SnakeBlasphemyPower>(), HoverTipFactory.FromPower<DivineSnakePower>()];

    public SnakeBlasphemyCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 退出其他蛇姿态
        if (Owner.Creature.HasPower<FuriousSnakePower>())
            await PowerCmd.Remove<FuriousSnakePower>(Owner.Creature);
        if (Owner.Creature.HasPower<ChargingSnakePower>())
            await PowerCmd.Remove<ChargingSnakePower>(Owner.Creature);

        // 进入神蛇姿态
        await PowerCmd.Apply<DivineSnakePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);

        // 给予渎蛇效果
        await PowerCmd.Apply<SnakeBlasphemyPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["SnakeBlasphemyPower"].BaseValue, Owner.Creature, this, false);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
