// 进阶之蛇 - 事件遗物
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeTheBite.Scripts.Powers;
using SnakeTheBite.Scripts.Potions;
using SnakeTheBite.Scripts.Cards;

namespace SnakeTheBite.Scripts.Relics;

[Pool(typeof(EventRelicPool))]
public class AdvancedSnakeRelic : SnakeTheBiteRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override bool IsAllowedInShops => false;
    public override bool HasUponPickupEffect => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("EffectsDescription", "根据进阶等级不断增加新的效果。")
    ];

    // 根据当前进阶等级更新遗物描述中的效果文本
    public void UpdateDescription()
    {
        int asc = Owner?.RunState.AscensionLevel ?? 0;
        string text = BuildEffectsDescription(asc);
        ((StringVar)DynamicVars["EffectsDescription"]).StringValue = text;
    }

    public override async Task AfterObtained()
    {
        UpdateDescription();
        int asc = Owner.RunState.AscensionLevel;

        // 进阶4：拾起时获得1瓶药水（进阶十增强为2瓶）
        if (asc >= 4)
        {
            int potionCount = GetValue(1, 2, asc >= 10);
            var snakePotions = ModelDb.AllPotions
                .Where(p => p is SnakeTheBitePotionModel)
                .ToList();
            for (int i = 0; i < potionCount && snakePotions.Count > 0; i++)
            {
                var potion = Owner.RunState.Rng.Niche.NextItem(snakePotions)!.ToMutable();
                await PotionCmd.TryToProcure(potion, Owner);
            }
        }

        // 进阶7：拾起时随机升级1张卡牌（进阶十增强为2张）
        if (asc >= 7)
        {
            int upgradeCount = GetValue(1, 2, asc >= 10);
            var upgradable = Owner.Deck.Cards.Where(c => c.IsUpgradable).ToList();
            foreach (var card in upgradable.StableShuffle(Owner.RunState.Rng.Niche).Take(upgradeCount))
            {
                CardCmd.Upgrade(card);
            }
        }

        // 进阶10：将一张进阶之福加入牌组
        if (asc >= 10)
        {
            var blessing = ModelDb.Card<AscendersBlessingCard>().ToMutable();
            blessing.Owner = Owner;
            Owner.Deck.AddInternal(blessing);
        }
    }

    public override async Task BeforeCombatStart()
    {
        UpdateDescription();
        int asc = Owner.RunState.AscensionLevel;
        if (asc < 1) return;
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        // 进阶1：精英战斗获得1层蛇之精准（进阶十增强为2层）
        if (asc >= 1 && combatState.Encounter.RoomType == RoomType.Elite)
        {
            int precision = GetValue(1, 2, asc >= 10);
            await PowerCmd.Apply<SnakePrecisionPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, precision, Owner.Creature, null, false);
        }

        // 进阶5：战斗开始将进阶之灾放入手牌
        if (asc >= 5)
        {
            var ascendersBane = Owner.PlayerCombatState?.AllCards.FirstOrDefault(c => c is AscendersBane);
            if (ascendersBane != null)
            {
                await CardPileCmd.Add(ascendersBane, PileType.Hand, CardPilePosition.Top, this);
            }
        }

        // 进阶8：战斗开始对所有敌人造成7点伤害（进阶十增强为10点）
        if (asc >= 8)
        {
            int damage = GetValue(7, 10, asc >= 10);
            var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count > 0)
            {
                await CreatureCmd.Damage(
                    new BlockingPlayerChoiceContext(), enemies, damage,
                    ValueProp.Unpowered | ValueProp.SkipHurtAnim,
                    Owner.Creature, null);
            }
        }

        // 进阶9：战斗开始使所有敌人第一回合失去1点力量（进阶十增强为2点）
        if (asc >= 9)
        {
            int strengthLoss = GetValue(1, 2, asc >= 10);
            foreach (var enemy in combatState.Enemies.Where(e => e.IsAlive))
            {
                await PowerCmd.Apply<DarkShacklesPower>(new ThrowingPlayerChoiceContext(), enemy, strengthLoss, Owner.Creature, null, false);
            }
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        int asc = Owner.RunState.AscensionLevel;
        if (asc < 2) return;

        // 进阶2：先古之民额外回复7%生命（进阶十增强为10%）
        if (asc >= 2 && room is EventRoom eventRoom && eventRoom.CanonicalEvent is AncientEventModel)
        {
            int percent = GetValue(7, 10, asc >= 10);
            decimal healAmount = Owner.Creature.MaxHp * percent / 100m;
            if (healAmount > 0)
            {
                Flash();
                await CreatureCmd.Heal(Owner.Creature, healAmount);
            }
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        int asc = Owner.RunState.AscensionLevel;
        if (asc < 3) return;

        // 进阶3：战斗后额外获得7金币（进阶十增强为10）
        int gold = GetValue(7, 10, asc >= 10);
        if (gold > 0)
        {
            Flash();
            await PlayerCmd.GainGold(gold, Owner);
        }
    }

    public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal cost)
    {
        int asc = Owner.RunState.AscensionLevel;
        if (asc < 6) return cost;
        if (player != Owner) return cost;
        if (entry is not MerchantCardRemovalEntry) return cost;

        // 进阶6：商人移除价格下降7%（进阶十增强为10%）
        int discount = GetValue(7, 10, asc >= 10);
        return cost * (100 - discount) / 100m;
    }

    internal static string BuildEffectsDescription(int ascension)
    {
        if (ascension <= 0)
            return "当前进阶等级未激活任何效果。";

        var sb = new StringBuilder();
        bool boosted = ascension >= 10;

        sb.AppendLine($"[gold]进阶1[/gold]：精英战斗获得[blue]{GetValue(1, 2, boosted)}[/blue]层[gold]蛇之精准[/gold]。");

        if (ascension >= 2)
            sb.AppendLine($"[gold]进阶2[/gold]：先古之民额外回复[blue]{GetValue(7, 10, boosted)}%[/blue]生命。");

        if (ascension >= 3)
            sb.AppendLine($"[gold]进阶3[/gold]：战斗后额外获得[blue]{GetValue(7, 10, boosted)}[/blue]金币。");

        if (ascension >= 4)
            sb.AppendLine($"[gold]进阶4[/gold]：拾起时获得[blue]{GetValue(1, 2, boosted)}[/blue]瓶[gold]蛇药[/gold]。");

        if (ascension >= 5)
            sb.AppendLine($"[gold]进阶5[/gold]：战斗开始将[gold]牌组[/gold]中的[gold]进阶之灾[/gold]放入手牌。");

        if (ascension >= 6)
            sb.AppendLine($"[gold]进阶6[/gold]：商人移除价格下降[blue]{GetValue(7, 10, boosted)}%[/blue]。");

        if (ascension >= 7)
            sb.AppendLine($"[gold]进阶7[/gold]：拾起时随机升级[blue]{GetValue(1, 2, boosted)}[/blue]张卡牌。");

        if (ascension >= 8)
            sb.AppendLine($"[gold]进阶8[/gold]：战斗开始对所有敌人造成[blue]{GetValue(7, 10, boosted)}[/blue]点伤害。");

        if (ascension >= 9)
            sb.AppendLine($"[gold]进阶9[/gold]：战斗开始使所有敌人第一回合失去[blue]{GetValue(1, 2, boosted)}[/blue]点[gold]力量[/gold]。");

        if (boosted)
            sb.AppendLine($"[gold]进阶10[/gold]：以上效果获得增强。");

        return sb.ToString().TrimEnd('\n');
    }

    private static int GetValue(int baseValue, int boostedValue, bool boosted) => boosted ? boostedValue : baseValue;
}
