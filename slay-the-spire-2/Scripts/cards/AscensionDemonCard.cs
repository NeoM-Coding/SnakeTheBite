// 升魔 - 命运能力牌（已禁用）
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using BaseLib.Abstracts;
// using BaseLib.Utils;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Creatures;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Models.CardPools;
// using MegaCrit.Sts2.Core.Models.Powers;
//
// namespace SnakeTheBite.Scripts.Cards;
//
// [Pool(typeof(ColorlessCardPool))]
// public class AscensionDemonCard : SnakeTheBiteCardModel
// {
//     public AscensionDemonCard() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
//     {
//     }
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         var attackCard = Owner.Creature.CombatState.CreateCard<AscensionDemonAttackCard>(Owner);
//         var skillCard = Owner.Creature.CombatState.CreateCard<AscensionDemonSkillCard>(Owner);
//         await CardPileCmd.AddGeneratedCardToCombat(attackCard, PileType.Hand, addedByPlayer: true);
//         await CardPileCmd.AddGeneratedCardToCombat(skillCard, PileType.Hand, addedByPlayer: true);
//         await PowerCmd.Apply<DisintegrationPower>(Owner.Creature, 1m, Owner.Creature, this);
//     }
// }
