// 渎蛇 - 1费稀有技能，消耗，本回合给予敌人的中毒层数变为3倍，下回合自身获得21层中毒。升级后获得保留。
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MapleShadow.Scripts.Powers;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class SnakeBlasphemyCard : MapleShadowCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SnakeBlasphemyPower>(3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<SnakeBlasphemyPower>(),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
        ];

    public SnakeBlasphemyCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SnakeBlasphemyPower>(Owner.Creature, DynamicVars["SnakeBlasphemyPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
