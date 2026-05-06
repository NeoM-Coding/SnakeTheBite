// 蛇蜕手牌上限补丁
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SnakeTheBite.Scripts.Relics;

namespace SnakeTheBite.Scripts.Patches;

[HarmonyPatch(typeof(CardPileCmd), "CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot")]
public class SnakeSkinHandSizePatch
{
    static bool Prefix(Player player, ref bool __result)
    {
        bool hasRelic = player?.Relics?.OfType<SnakeSkinRelic>().Any() == true;
        int maxHandSize = hasRelic ? 12 : CardPile.maxCardsInHand;

        if (PileType.Draw.GetPile(player).Cards.Count + PileType.Discard.GetPile(player).Cards.Count == 0)
        {
            ThinkCmd.Play(new LocString("combat_messages", "NO_DRAW"), player.Creature, 2.0);
            __result = false;
            return false;
        }
        if (PileType.Hand.GetPile(player).Cards.Count >= maxHandSize)
        {
            ThinkCmd.Play(new LocString("combat_messages", "HAND_FULL"), player.Creature, 2.0);
            __result = false;
            return false;
        }
        __result = true;
        return false;
    }
}
