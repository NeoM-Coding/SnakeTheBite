// 蛇之血清 - 罕见药水，本回合手牌费用正常，获得本回合与下回合各1点能量，可给队友
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeTheBite.Scripts.Potions;

[Pool(typeof(SharedPotionPool))]
public class SnakeSerumPotion : SnakeTheBitePotionModel
{
    // 稀有度 - 罕见（蓝）
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    // 使用方式 - 仅战斗中使用
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型 - 任意玩家（可以给队友）
    public override TargetType TargetType => TargetType.AnyPlayer;

    // 动态变量 - 1点能量
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];

    // 使用时的效果逻辑
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);

        // 1. 本回合手牌费用正常化：清除临时费用修改并设为标准费用
        var hand = PileType.Hand.GetPile(target.Player!);
        foreach (var card in hand.Cards)
        {
            if (card.EnergyCost.CostsX)
                continue;
            // 清除本回合与打出的临时修改
            card.EnergyCost.EndOfTurnCleanup();
            card.EnergyCost.AfterCardPlayedCleanup();
            // 强制设为本回合的标准费用
            card.EnergyCost.SetThisTurn(card.EnergyCost.Canonical);
        }

        // 2. 本回合获得 1 点能量
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, target.Player!);

        // 3. 下回合获得 1 点能量
        await PowerCmd.Apply<EnergyNextTurnPower>(new ThrowingPlayerChoiceContext(), target, DynamicVars.Energy.BaseValue, target, null, false);
    }
}
