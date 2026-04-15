// 渎蛇能力 - 本回合给予敌人的中毒层数变为Amount倍，下回合自身获得7×Amount层中毒
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Powers;

public class SnakeBlasphemyPower : MapleShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => (int)Amount;
    protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        Flash();
        await PowerCmd.Apply<PoisonPower>(Owner, 7m * Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}

// Harmony 补丁：本回合内玩家给予敌人的中毒层数乘以 SnakeBlasphemyPower 的 Amount
[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyPowerAmountGiven))]
public static class SnakeBlasphemyModifyPowerAmountGivenPatch
{
    static decimal Postfix(decimal __result, PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (power is PoisonPower && giver?.GetPower<SnakeBlasphemyPower>() is { } blasphemy && target != null && target.IsEnemy)
        {
            return __result * blasphemy.Amount;
        }
        return __result;
    }
}
