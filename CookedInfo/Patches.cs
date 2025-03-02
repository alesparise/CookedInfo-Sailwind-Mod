using HarmonyLib;
using System.Text;
using UnityEngine;

namespace CookedInfo
{
    internal class Patches
    {
        [HarmonyPatch(typeof(FoodState))]
        public class FoodStatePatches
        {
            static readonly string red = "#4D0000";
            static readonly string green = "#003300";
            static readonly string yellow = "#CC7F00";
            static readonly string light_blue = "#2861fc";
            static readonly string med_blue = "#1C3270";
            static readonly string dark_blue = "#071336";

            [HarmonyPostfix]
            [HarmonyPatch("UpdateLookText")]
            public static void UpdateLookText_Patch(
                ShipItemFood ___food,
                CookableFood ___cookable,
                float ___dried,
                DryingRackCol ___dryingCol,
                float ___spoiled)
            {
                if (___cookable.GetCurrentCookTrigger())
                {
                    if (___food.amount > 0f && ___food.amount < 1f)
                        ___food.description = $"{BuildDescription(yellow, ___food.description, ___food.amount)}";

                    if (___food.amount >= 1f && ___food.amount < 1.25f)
                        ___food.description = $"{BuildDescription(green, ___food.description, ___food.amount)}";

                    if (___food.amount >= 1.25f)                    
                        ___food.description = $"{BuildDescription(red, $"overcooked {___food.description.Replace("cooked ", "")}", ___food.amount)}";                    
                }

                if (___dryingCol)
                {
                    if (___dried > 0f && ___dried < 0.99f)
                        ___food.description = $"{BuildDescription(yellow, ___food.description, ___dried)}";

                    if (___dried >= 0.99f)
                        ___food.description = $"{BuildDescription(green, ___food.description, ___dried)}";
                }

                if (!Plugin.configFreshnessBar.Value) return;
                if (___spoiled < 0.30f)
                    ___food.description = $"{FreshnessBar(light_blue, ___food, ___spoiled)}";
                if (___spoiled >= 0.30f && ___spoiled < 0.6f)
                    ___food.description = $"{FreshnessBar(med_blue, ___food, ___spoiled)}";
                if (___spoiled >= 0.60f && ___spoiled < 0.9f)
                    ___food.description = $"{FreshnessBar(dark_blue, ___food, ___spoiled)}";
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

            private static string FreshnessBar(string color, ShipItemFood food, float amount)
            {
                if (!Plugin.configColoredText.Value) 
                    return $"{food.description}{ProgressBar(0.9f - amount)}"; ;

                return $"{food.description}<color={color}>{ProgressBar(0.9f - amount)}</color>";
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
}
