// 进阶之蛇 - 进阶9 临时力量损失（已禁用）
// using BaseLib.Abstracts;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Combat;
// using MegaCrit.Sts2.Core.Entities.Creatures;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Entities.Powers;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Models.Powers;
// using MegaCrit.Sts2.Core.ValueProps;
//
// namespace SnakeTheBite.Scripts.Powers;
//
// public class AdvancedSnakeTempStrengthLossPower : SnakeTheBitePowerModel
// {
//     public override PowerType Type => PowerType.Debuff;
//     public override PowerStackType StackType => PowerStackType.Counter;
//     public override int DisplayAmount => (int)Amount;
//
//     protected override string SmartDescriptionLocKey => base.Id.Entry + ".description";
//
//     public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
//     {
//         if (dealer != Owner)
//             return 0m;
//         return -Amount;
//     }
//
//     public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
//     {
//         if (side != Owner.Side)
//             return;
//         await PowerCmd.Remove(this);
//     }
// }
