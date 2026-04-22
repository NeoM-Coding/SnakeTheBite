using System;
using SnakeTheBite.Scripts.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace SnakeTheBite.Scripts.Enchantments;

// 蛇液强化附魔
// 为蛇牌永久增加中毒数值，可重复叠加。
public class SnakeVenomBoostEnchantment : SnakeTheBiteEnchantmentModel
{
    // 允许同一张牌上叠加多次（Amount +1）
    public override bool IsStackable => true;

    // 附魔图标路径
    protected override string? CustomIconPath => $"res://SnakeTheBite/images/enchantments/{SnakeTheBiteModelHelper.ToSnakeCase(GetType().Name)}.png";

    // 只能附魔到带毒的蛇牌上
    public override bool CanEnchant(CardModel card)
    {
        if (!base.CanEnchant(card))
            return false;
        return SnakeTheBiteCardTags.IsSnakeCard(card)
            && (card.DynamicVars.ContainsKey("PoisonPower") || card.DynamicVars.ContainsKey("TruePoisonPower"));
    }

    // 每次重算时，把当前卡牌的对应数值设为【Canonical Base + Amount】
    // 这样无论是首次附魔、叠加、存档读档还是升级/降级，结果都始终正确。
    public override void RecalculateValues()
    {
        base.RecalculateValues();
        if (Card == null) return;

        if (Card.DynamicVars.ContainsKey("PoisonPower"))
        {
            decimal canonical = GetCanonicalValue("PoisonPower");
            Card.DynamicVars["PoisonPower"].BaseValue = canonical + Amount;
        }

        if (Card.DynamicVars.ContainsKey("TruePoisonPower"))
        {
            decimal canonical = GetCanonicalValue("TruePoisonPower");
            Card.DynamicVars["TruePoisonPower"].BaseValue = canonical + Amount;
        }

        if (Card.DynamicVars.ContainsKey("CalculationBase"))
        {
            decimal canonical = GetCanonicalValue("CalculationBase");
            Card.DynamicVars["CalculationBase"].BaseValue = canonical + Amount;
        }
    }

    // 获取当前卡牌在没有任何附魔时的 Canonical 基础值（已考虑升级等级）
    private decimal GetCanonicalValue(string dynamicVarName)
    {
        var canonical = ModelDb.GetById<CardModel>(Card.Id).ToMutable();
        for (int i = 0; i < Card.CurrentUpgradeLevel; i++)
        {
            canonical.UpgradeInternal();
            canonical.FinalizeUpgradeInternal();
        }
        return canonical.DynamicVars.TryGetValue(dynamicVarName, out var var)
            ? var.BaseValue
            : 0m;
    }
}
