// 蛇咬不咬 - 2费无色技能，保留。给予12层中毒，50%概率给予随机敌人，50%概率给予自己或队友。
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

[Pool(typeof(ColorlessCardPool))]
public class SnakeBiteOrNotCard : SnakeTheBiteCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PoisonPower>(12m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    public SnakeBiteOrNotCard() : base(energyCost, type, rarity, targetType) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int poisonAmount = DynamicVars.Poison.IntValue;
        var rng = Owner.RunState.Rng.CombatTargets;

        // 50% 概率给予随机敌人，50% 概率给予自己或队友
        bool giveToEnemy = rng.NextBool();

        if (giveToEnemy)
        {
            var enemies = CombatState.HittableEnemies;
            if (enemies.Count > 0)
            {
                var target = rng.NextItem(enemies);
                await PowerCmd.Apply<PoisonPower>(target, poisonAmount, Owner.Creature, this);
            }
        }
        else
        {
            var players = CombatState.Players;
            if (players.Count > 0)
            {
                var targetPlayer = rng.NextItem(players);
                await PowerCmd.Apply<PoisonPower>(targetPlayer.Creature, poisonAmount, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Poison.UpgradeValueBy(3m);
    }
}
