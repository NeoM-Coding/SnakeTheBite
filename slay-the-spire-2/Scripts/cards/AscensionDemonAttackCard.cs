// 升魔攻击 - 升魔生成的临时攻击牌（已禁用）
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using BaseLib.Abstracts;
// using BaseLib.Utils;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Creatures;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using MegaCrit.Sts2.Core.ValueProps;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Models.CardPools;
//
// namespace SnakeTheBite.Scripts.Cards;
//
// [Pool(typeof(TokenCardPool))]
// public class AscensionDemonAttackCard : SnakeTheBiteCardModel
// {
//     public AscensionDemonAttackCard() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, shouldShowInCardLibrary: false)
//     {
//     }
//
//     protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0m, ValueProp.Move)];
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
//         await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
//             .Execute(choiceContext);
//     }
// }
