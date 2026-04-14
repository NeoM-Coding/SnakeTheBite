using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MapleShadow.Scripts.Cards;

// 全身蛇咬
[Pool(typeof(IroncladCardPool))]
public class FullBodySnakeBiteCard : MapleShadowCardModel
{
    // 基础耗能 - 1费(白卡)
    private const int energyCost = 1;
    // 卡牌类型 - 攻击牌
    private const CardType type = CardType.Attack;
    // 卡牌稀有度 - 普通
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型 - 任意敌人
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    public FullBodySnakeBiteCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑 - 给予敌人等于自身格挡值的中毒层数
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_bite");

        int poisonAmount = Owner.Creature.Block;
        if (poisonAmount > 0)
        {
            await PowerCmd.Apply<PoisonPower>(cardPlay.Target, poisonAmount, Owner.Creature, this);
        }
    }

    // 升级后的效果逻辑 - 费用减1（1 → 0）
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
