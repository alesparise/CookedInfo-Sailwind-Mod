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

            [HarmonyPostfix]
            [HarmonyPatch("UpdateLookText")]
            public static void UpdateLookText_Patch(
                ShipItemFood ___food,
                CookableFood ___cookable,
                float ___dried,
                DryingRackCol ___dryingCol)
            {
                if (___cookable.GetCurrentCookTrigger()) 
                {
                    if (!Plugin.configColoredText.Value)
                        ___food.description = $"\n{BuildDescription(___food)}";

                    if (___food.amount > 0f && ___food.amount < 1f)
                        ___food.description = $"\n<color={yellow}>{BuildDescription(___food)}</color>";

                    if (___food.amount >= 1f && ___food.amount < 1.5f)
                        ___food.description = $"\n<color={green}>{BuildDescription(___food)}</color>";

                    if (___food.amount > 1.5f)
                        ___food.description = $"\n<color={red}>{BuildDescription(___food)}</color>";
                }

                if (___dryingCol)
                {
                    if (!Plugin.configColoredText.Value)                    
                        ___food.description = $"\n{BuildDescription(___food, ___dried)}";
                    
                    if (___dried > 0f && ___dried < 1f)                    
                        ___food.description = $"\n<color={yellow}>{BuildDescription(___food, ___dried)}</color>";
                    
                    if (___dried >= 0.99f)                    
                        ___food.description = $"\n<color={green}>{BuildDescription(___food, ___dried)}</color>";                    
                }
            }

            private static string BuildDescription(ShipItem food)
            {                
                if (food.amount >= 1.25f && food.amount < 1.5f)
                    return $"overcooked {(food.description.Contains("cooked") ? "" : food.description)} {CookedPercent(food.amount)}{CookingBar(food.amount)}";                

                return $"{food.description} {CookedPercent(food.amount)}{CookingBar(food.amount)}";
            }

            private static string BuildDescription(ShipItem food, float dried)
            {
                return $"{food.description} {CookedPercent(dried)}{CookingBar(dried)}";
            }

            private static string CookedPercent(float amount)
            {
                if (!Plugin.configCookingPercent.Value) return "";

                return $"\n<b>{Mathf.RoundToInt(amount * 100)}%</b>";
            }

            private static string CookingBar(float amount)
            {   //possibly useful ASCII characters: ████░░░░░░ ████▒▒▒▒ ■□□□□ ▄▄▄... ▀▀    ═══───
                if (!Plugin.configCookingBar.Value) return "";
                
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
