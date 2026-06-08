// 游魂 - 命运技能牌，获得"记录"，下回合每打出1张牌回复2点生命值
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
using SnakeTheBite.Scripts.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class WanderingSpiritCard : SnakeTheBiteCardModel
{
    public WanderingSpiritCard() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RecordPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, Owner.Creature.CurrentHp, Owner.Creature, this, false);
    }
}
