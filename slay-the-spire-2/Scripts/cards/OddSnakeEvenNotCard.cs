// 奇蛇偶不蛇 - 保留，无法打出。给予随机敌人7层中毒。回合结束后，若手牌数为奇数，自动打出这张牌。
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class OddSnakeEvenNotCard : SnakeTheBiteCardModel
{
    private const int energyCost = -1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override bool HasTurnEndInHandEffect => true;

    protected override bool IsPlayable => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(7m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    public OddSnakeEvenNotCard() : base(energyCost, type, rarity, targetType) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var enemies = CombatState.HittableEnemies;
        if (enemies.Count > 0)
        {
            var target = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
            await PowerCmd.Apply<PoisonPower>(target, DynamicVars.Poison.IntValue, Owner.Creature, this);
        }
    }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        var hand = PileType.Hand.GetPile(Owner);
        if (hand.Cards.Count % 2 == 1)
        {
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(2m);
    }
}
