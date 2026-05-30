// 要蛇喽！能力 - 2回合后给予所有敌人中毒（第3回合抽牌结束后触发）
using System;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class SnakeRainPower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    // Amount=3 对应 2 回合延迟：第2回合递减到2，第3回合递减到1，然后触发
    // 角标显示 Amount-1，让玩家看到 2→1
    public override int DisplayAmount => Math.Max(1, (int)Amount - 1);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PoisonPower>(15m)];

    public void SetPoisonAmount(decimal amount)
    {
        AssertMutable();
        base.DynamicVars.Poison.BaseValue = amount;
    }

    // 在回合开始时递减
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
            return;
        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
        }
    }

    // 在抽牌结束后触发（AfterSideTurnStart 在 SetupPlayerTurn/抽牌之后）
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
            return;
        if (Amount > 1)
            return;

        Flash();
        var enemies = combatState.HittableEnemies;
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<PoisonPower>(new ThrowingPlayerChoiceContext(), enemy, DynamicVars.Poison.BaseValue, Owner, null, false);
        }
        await PowerCmd.Remove(this);
    }
}
