using HarmonyLib;
using System.Text;
using UnityEngine;

namespace CookedInfo
{
    internal class Patches
    {
        static readonly string red = "#4D0000";
        static readonly string green = "#003300";
        static readonly string yellow = "#CC7F00";
        static readonly string light_blue = "#2861FC";
        static readonly string med_blue = "#1C3270";
        static readonly string dark_blue = "#071336";

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
                    if (___food.amount > 0f && ___food.amount < 1f)
                        ___food.description = $"{BuildDescription(yellow, ___food.description, ___food.amount)}";

                    if (___food.amount >= 1f && ___food.amount < 1.25f)
                        ___food.description = $"{BuildDescription(green, ___food.description, ___food.amount)}";

                    if (___food.amount >= 1.25f)
                        ___food.description = $"{BuildDescription(red, $"overcooked {___food.description.Replace("cooked ", "")}", ___food.amount)}";
                }

                // if on drying rack, food dries at 0.99f
                if (___dryingCol)
                {
                    if (___dried > 0f && ___dried < 0.99f)
                        ___food.description = $"{BuildDescription(yellow, ___food.description, ___dried)}";

                    if (___dried >= 0.99f)
                        ___food.description = $"{BuildDescription(green, ___food.description, ___dried)}";
                }

                // freshness, food spoils at 0.9f
                var spoiled = ___spoiled / 0.9f;
                if (!Plugin.configFreshnessBar.Value) return;
                if (spoiled < 0.33f)
                    ___food.description = $"{FreshnessBar(light_blue, ___food.description, spoiled)}";
                if (spoiled >= 0.33f && spoiled < 0.66f)
                    ___food.description = $"{FreshnessBar(med_blue, ___food.description, spoiled)}";
                if (spoiled >= 0.66f)
                    ___food.description = $"{FreshnessBar(dark_blue, ___food.description, spoiled)}";
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
                float ___currentEnergy,
                float ___currentUncookedEnergy,
                float ___currentSpoiled)
            {
                // soup doesn't have a fixed cook point, use a ratio of current engergy / current energy + uncooked energy
                var amount = ___currentEnergy / (___currentEnergy + ___currentUncookedEnergy);

                // soup spoils at currentSpoiled / (currentEnergy + currentUncookedEnergy) > 0.9f
                var spoiled = (___currentSpoiled / (___currentEnergy + ___currentUncookedEnergy)) / 0.9f;

                // soup doesn't have good descriptions yet so making our own
                if (amount > 0f && amount < 1f)
                    __instance.description = $"uncooked soup";
                if (amount >= 1f)
                    __instance.description = $"cooked soup";
                if (spoiled >= 1f)
                    __instance.description = $"rotten soup";

                // if in stove or smoker
                if (___cookable.GetCurrentCookTrigger())
                {
                    if (amount > 0f && amount < 1f)
                        __instance.description = $"{BuildDescription(yellow, __instance.description, amount)}";
                    if (amount >= 1f)
                        __instance.description = $"{BuildDescription(green, __instance.description, amount)}";
                }

                // freshness                
                if (!Plugin.configFreshnessBar.Value) return;
                if (spoiled < 0.33f)
                    __instance.description = $"{FreshnessBar(light_blue, __instance.description, spoiled)}";
                if (spoiled >= 0.33f && spoiled < 0.66f)
                    __instance.description = $"{FreshnessBar(med_blue, __instance.description, spoiled)}";
                if (spoiled >= 0.66f)
                    __instance.description = $"{FreshnessBar(dark_blue, __instance.description, spoiled)}";
            }
        }

        private static string BuildDescription(string color, string desc, float amount)
        {
            if (!Plugin.configColoredText.Value)
                return $"{desc}{CookedPercent(amount)}{CookingBar(amount)}";

            return $"{desc}<color={color}>{CookedPercent(amount)}{CookingBar(amount)}</color>";
        }

        private static string CookedPercent(float amount)
        {
            if (!Plugin.configCookingPercent.Value) return "";

            return $"\n<b>{Mathf.RoundToInt(amount * 100)}%</b>";
        }

        private static string CookingBar(float amount)
        {
            if (!Plugin.configCookingBar.Value) return "";

            return $"{ProgressBar(amount)}";
        }

        private static string FreshnessBar(string color, string foodDescr, float amount)
        {
            if (!Plugin.configColoredText.Value)
                return $"{foodDescr}{ProgressBar(0.9f - amount)}"; ;

            return $"{foodDescr}<color={color}>{ProgressBar(0.9f - amount)}</color>";
        }

        private static string ProgressBar(float amount)
        {   //possibly useful ASCII characters: ████░░░░░░ ████▒▒▒▒ ■□□□□ ▄▄▄... ▀▀    ═══───
            int barLength = 300; // Total length of the loading bar
            int filledLength = (int)(amount * barLength); // Calculate the number of filled characters

            if (filledLength <= 0 || filledLength >= barLength) return ""; //empty string if raw or fully cooked already.

            StringBuilder bar = new StringBuilder();
            for (int i = 0; i < barLength; i++)
            {
                if (i < filledLength)
                {
                    bar.Append("█"); // Filled character
                }
                else
                {
                    bar.Append("░"); // Empty character
                }
            }

            return $"\n<size=3%>{bar}</size>";
        }
    }
}
