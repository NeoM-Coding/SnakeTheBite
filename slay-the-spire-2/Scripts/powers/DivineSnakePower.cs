// 神蛇能力 - 蛇之姿态。施加的中毒与真实中毒变为三倍。
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Powers;

public class DivineSnakePower : SnakeTheBitePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

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
