// 缺陷机器人 - 羁绊能力牌，0费，综合模拟缺陷机器人效果
using System.Collections.Generic;
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
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class DefectRobotCard : SnakeTheBiteCardModel
{
    public DefectRobotCard() : base(0, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 增加充能球栏位
        Owner.BaseOrbSlotCount += 3;

        // 最大生命+10
        Owner.Creature.SetMaxHpInternal(Owner.Creature.MaxHp + 10);
        await CreatureCmd.Heal(Owner.Creature, 10m);

        // 获得Defect初始牌组中的几张牌
        var combatState = Owner.Creature.CombatState;
        if (combatState != null)
        {
            var defectCards = new List<CardModel>
            {
                combatState.CreateCard<StrikeDefect>(Owner),
                combatState.CreateCard<StrikeDefect>(Owner),
                combatState.CreateCard<DefendDefect>(Owner),
                combatState.CreateCard<DefendDefect>(Owner),
                combatState.CreateCard<Zap>(Owner),
                combatState.CreateCard<Dualcast>(Owner),
            };
            foreach (var c in defectCards)
            {
                await CardPileCmd.AddGeneratedCardToCombat(c, PileType.Draw, addedByPlayer: true);
            }
        }

        // 施加综合Power
        await PowerCmd.Apply<DefectRobotPower>(Owner.Creature, 1m, Owner.Creature, this);
    }
}
