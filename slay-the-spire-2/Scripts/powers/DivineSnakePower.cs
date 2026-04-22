// 神蛇能力 - 蛇之姿态。本回合内，施加的中毒与真实中毒变为三倍。自己回合开始时移除。
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Powers;

public class DivineSnakePower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    // 自己回合开始时移除该姿态
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// Harmony 补丁：神蛇姿态下，玩家给予敌人的中毒与真实中毒层数变为三倍
[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyPowerAmountGiven))]
public static class DivineSnakeModifyPowerAmountGivenPatch
{
    static decimal Postfix(decimal __result, PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if ((power is PoisonPower or TruePoisonPower) && giver?.GetPower<DivineSnakePower>() is not null && target != null && target.IsEnemy)
        {
            return __result * 3m;
        }
        return __result;
    }
}
