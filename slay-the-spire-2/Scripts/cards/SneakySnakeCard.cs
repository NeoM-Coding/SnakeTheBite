using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

// 偷偷的蛇
[Pool(typeof(IroncladCardPool))]
public class SneakySnakeCard : SnakeTheBiteCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(3, ValueProp.Move), new PowerVar<SneakySnakePower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<SneakySnakePower>()];

    public SneakySnakeCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var power = await PowerCmd.Apply<SneakySnakePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["SneakySnakePower"].BaseValue, Owner.Creature, this, false);
        if (power != null)
        {
            decimal cardMultiplier = IsUpgraded ? 0.50m : 0.75m;
            if (power.DynamicVars["DamageReduction"].BaseValue > cardMultiplier)
            {
                power.DynamicVars["DamageReduction"].BaseValue = cardMultiplier;
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
