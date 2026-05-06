// 升魔技能 - 升魔生成的临时技能牌（已禁用）
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
// public class AscensionDemonSkillCard : SnakeTheBiteCardModel
// {
//     public AscensionDemonSkillCard() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self, shouldShowInCardLibrary: false)
//     {
//     }
//
//     protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(0m, ValueProp.Move)];
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
//     }
// }
