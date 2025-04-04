using HarmonyLib;

namespace CookedInfo
{
    internal class Patches
    {
        [HarmonyPatch(typeof(FoodState))]
        public class FoodStatePatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("UpdateLookText")]
            public static void UpdateLookText_Patch(
                ShipItemFood ___food,
                CookableFood ___cookable,
                float ___dried,
                DryingRackCol ___dryingCol,
                float ___spoiled)
            {
                // if in stove or smoker, food is cooked at 1f and burns at 1.5f
                if (___cookable.GetCurrentCookTrigger())
                {
                    DescriptionBuilder.BurnableDescription(ref ___food.description, ___food.amount);
                }

                // if on drying rack, food dries at 0.99f
                if (___dryingCol)
                {
                    var amount = ___dried / 0.99f;
                    DescriptionBuilder.NonBurnableDescription(ref ___food.description, amount);
                }

                // freshness, food spoils at 0.9f
                var spoiled = ___spoiled / 0.9f;
                DescriptionBuilder.Freshness(ref ___food.description, spoiled);
            }
        }

        [HarmonyPatch(typeof(ShipItemSoup))]
        public class ShipItemSoupPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("UpdateLookText")]
            public static void UpdateLookText_Patch(
                ShipItemSoup __instance,
                CookableFoodSoup ___cookable,
                float ___currentWater,
                float ___currentEnergy,
                float ___currentUncookedEnergy,
                float ___currentSpoiled)
            {
                // soup doesn't have a fixed cook point, use a ratio of current energy / current energy + uncooked energy
                var amount = ___currentEnergy / (___currentEnergy + ___currentUncookedEnergy);

                // soup spoils at currentSpoiled / (currentEnergy + currentUncookedEnergy) > 0.9f
                var spoiled = ___currentSpoiled / (___currentEnergy + ___currentUncookedEnergy) / 0.9f;

                // soup doesn't have good descriptions yet so making our own
                if (amount > 0f && amount < 1f)
                    __instance.description = $"uncooked soup";
                if (amount >= 1f)
                    __instance.description = $"cooked soup";
                if (spoiled >= 1f)
                    __instance.description = $"rotten soup";
                if (___currentEnergy + ___currentUncookedEnergy == 0f)
                    __instance.description = ___currentWater > 0f ? $"{__instance.name} of water" : __instance.name;

                // if in stove or smoker
                if (___cookable.GetCurrentCookTrigger())
                {
                    DescriptionBuilder.NonBurnableDescription(ref __instance.description, amount);
                }

                // freshness
                DescriptionBuilder.Freshness(ref __instance.description, spoiled);
            }
        }
    }
}
