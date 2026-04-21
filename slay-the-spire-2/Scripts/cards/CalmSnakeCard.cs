// 平静之蛇 - 1费稀有技能。若已在蓄蛇，从抽牌堆选2(3)张蛇牌加入手牌；否则退出怒蛇（若有）并进入蓄蛇。
using BaseLib.Abstracts;
using BaseLib.Utils;
using MapleShadow.Scripts.Powers;
using MapleShadow.Scripts.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MapleShadow.Scripts.Cards;

[Pool(typeof(IroncladCardPool))]
public class CalmSnakeCard : MapleShadowCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("SnakeSelectCount", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ChargingSnakePower>()];

    public CalmSnakeCard() : base(energyCost, type, rarity, targetType) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 若已在蓄蛇，从抽牌堆选择蛇牌加入手牌
        if (Owner.Creature.HasPower<ChargingSnakePower>())
        {
            var drawPile = PileType.Draw.GetPile(Owner);
            var snakeCards = drawPile.Cards
                .Where(c => MapleShadowCardTags.IsSnakeCard(c))
                .ToList();

            if (snakeCards.Count > 0)
            {
                int selectCount = DynamicVars["SnakeSelectCount"].IntValue;
                var prefs = new CardSelectorPrefs(
                    new LocString("card_selection", "TO_DRAW"),
                    Math.Min(selectCount, snakeCards.Count)
                );
                var selected = await CardSelectCmd.FromSimpleGrid(ctx, snakeCards, Owner, prefs);
                foreach (var card in selected)
                {
                    await CardPileCmd.Add(card, PileType.Hand);
                }
            }
        }
        else
        {
            // 退出怒蛇（若有）
            if (Owner.Creature.HasPower<FuriousSnakePower>())
            {
                await PowerCmd.Remove<FuriousSnakePower>(Owner.Creature);
            }

            // 进入蓄蛇
            await PowerCmd.Apply<ChargingSnakePower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SnakeSelectCount"].UpgradeValueBy(1m);
    }
}
