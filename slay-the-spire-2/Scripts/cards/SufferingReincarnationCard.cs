// 苦痛轮回 - 命运技能牌，1费，给予各牌堆2张灵魂，给予随机敌人20层灾厄
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class SufferingReincarnationCard : SnakeTheBiteCardModel
{
    public SufferingReincarnationCard() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        // 手牌2张灵魂
        var handSouls = Soul.Create(Owner, 2, combatState);
        await CardPileCmd.AddGeneratedCardsToCombat(handSouls, PileType.Hand, Owner, CardPilePosition.Random);

        // 抽牌堆2张灵魂
        var drawSouls = Soul.Create(Owner, 2, combatState);
        await CardPileCmd.AddGeneratedCardsToCombat(drawSouls, PileType.Draw, Owner, CardPilePosition.Random);

        // 弃牌堆2张灵魂
        var discardSouls = Soul.Create(Owner, 2, combatState);
        await CardPileCmd.AddGeneratedCardsToCombat(discardSouls, PileType.Discard, Owner, CardPilePosition.Random);

        // 给予随机敌人20层灾厄
        var enemies = combatState.GetOpponentsOf(Owner.Creature).Where(c => c.IsAlive).ToList();
        if (enemies.Count > 0)
        {
            var target = enemies[Owner.RunState.Rng.CombatTargets.NextInt(enemies.Count)];
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), target, 20m, Owner.Creature, this, false);
        }
    }
}
