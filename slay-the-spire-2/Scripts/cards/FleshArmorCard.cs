// 血肉加身 - 命运能力牌，0费，奥斯提造成伤害时玩家与奥斯提获得等量临时格挡
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
public class FleshArmorCard : SnakeTheBiteCardModel
{
    public FleshArmorCard() : base(0, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FleshArmorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
