// 王之蛇咬能力 - 本场战斗每打出一张蛇牌，所有敌人获得中毒（不叠加）
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class KingSnakeBitePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override int DisplayAmount => (int)Amount;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (!SnakeTheBiteCardTags.IsSnakeCard(cardPlay.Card))
            return;

        var enemies = Owner.CombatState.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToList();
        if (enemies.Count == 0)
            return;

        Flash();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<PoisonPower>(enemy, Amount, Owner, null);
        }
    }

    // 本场战斗持续，无需在回合开始时移除
}
