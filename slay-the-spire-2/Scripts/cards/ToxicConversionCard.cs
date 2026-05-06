// 毒性转化 - 1费红卡技能，将自身中毒双倍给予所有敌人（升级后包含真实中毒）
using BaseLib.Abstracts;
using BaseLib.Utils;
using SnakeTheBite.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class ToxicConversionCard : SnakeTheBiteCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<TruePoisonPower>()];

    public ToxicConversionCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState?.GetOpponentsOf(Owner.Creature).Where(c => c.IsAlive).ToList() ?? [];
        if (enemies.Count == 0)
            return;

        int poisonAmount = Owner.Creature.GetPowerAmount<PoisonPower>();
        if (poisonAmount > 0)
        {
            foreach (var enemy in enemies)
            {
                await PowerCmd.Apply<PoisonPower>(enemy, poisonAmount * 2, Owner.Creature, this);
            }
        }

        if (IsUpgraded)
        {
            int truePoisonAmount = Owner.Creature.GetPowerAmount<TruePoisonPower>();
            if (truePoisonAmount > 0)
            {
                foreach (var enemy in enemies)
                {
                    await PowerCmd.Apply<TruePoisonPower>(enemy, truePoisonAmount * 2, Owner.Creature, this);
                }
            }
        }
    }
}
