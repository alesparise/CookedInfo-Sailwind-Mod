using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CookedInfo
{
    internal class Patches
    {
        [HarmonyPatch(typeof(FoodState))]
        public class CookableFoodPatches
        {
            //hex color values
            static readonly string red = "#4D0000";
            static readonly string green = "#003300";
            static readonly string yellow = "#CC7F00";
            //list of the foods that don't display a name by default
            static readonly string[] noNameFoods = { "sausage", "dates", "bread", "banana", "bun", "crab cake", "cheese", "goat cheese", "lamb", "rice cake", "forest mushroom", "field mushroom", "cave mushroom", "orange" };

            [HarmonyPostfix] //patch happens after the original
            [HarmonyPatch("UpdateLookText")]
            public static void UpdateLookText_Patch(ShipItemFood ___food, float __dried, float __smoked, float __salted, float __spoiled)
            {   //There are five possible situations depending on the value of __item.amount
                if (noNameFoods.Contains(___food.name) && ___food.amount == 0f && Plugin.configNameAllFood.Value)
                {   //if the item is a noNameFoods, then use the name as description (e.g. bread, orange, dates)
                    //the check for amount == 0f is so that we don't get stuff like "Raw Orange"!
                    ___food.description = ___food.name;
                }
                if (Plugin.configColoredText.Value && ___food.description != "") //second check is needed for cases where nameAllFood is false
                {   //things happens with colored text.

                    if (___food.amount == 0f && !noNameFoods.Contains(___food.name))
                    {   //yellow, but only to OG named foods (eg fishes), so we don't get stuff like "Raw orange"!
                        ___food.description = $"\n<color={yellow}>{BuildDescription(___food)}</color>";
                    }
                    if (___food.amount > 0f && ___food.amount < 1f)
                    {   //yellow
                        ___food.description = $"\n<color={yellow}>{BuildDescription(___food)}</color>";
                    }
                    if (___food.amount >= 1f && ___food.amount < 1.5f)
                    {   //green
                        ___food.description = $"\n<color={green}>{BuildDescription(___food)}</color>";
                    }
                    if (___food.amount > 1.5f)
                    {   //red.
                        ___food.description = $"\n<color={red}>{BuildDescription(___food)}</color>";
                    }
                }
                else if (Plugin.configColoredText.Value == false && ___food.description != "")
                {   //things happens without colored text.
                    ___food.description = $"\n{BuildDescription(___food)}";
                }
            }

            private static string BuildDescription(ShipItem food)
            {   //this gets the description all together.
                return $"{food.description} {CookedPercent(food.amount)}{CookingBar(food.amount)}";
                //return $"{CookedStatus(food)}{CookedPercent(food.amount)}{CookingBar(food.amount)}";
            }

            //private static string CookedStatus(ShipItem food)
            //{   //returns the food status (0 = raw, 1 = undercooked, 2 = cooked, 3 = overcooked, 4 = burnt)
            //    if (food.amount <= 0f)
            //    {   //raw
            //        return $"Raw {food.name}";
            //    }
            //    else if (food.amount > 0f && food.amount < 1f)
            //    {   //undercooked
            //        return $"Undercooked {food.name}";
            //    }
            //    else if (food.amount >= 1f && food.amount < 1.5f)
            //    {   //cooked
            //        return $"Cooked {food.name}";
            //    }
            //    else if (food.amount >= 1.5f && food.amount < 1.75f)
            //    {   //overcooked
            //        return $"Overcooked {food.name}";
            //    }
            //    else
            //    {   //burnt (over 1.75f)
            //        return $"Burnt {food.name}";
            //    }
            //}

            private static string CookedPercent(float amount)
            {   //returns the % string
                if (!Plugin.configCookingPercent.Value)
                {   //empty string if we don't want the %
                    return "";
                }

                //var maxValue = [food.amount, ]

                int percent = Mathf.RoundToInt(amount * 100);

                return $"\n<b>{percent}%</b>";
            }
            private static string CookingBar(float amount)
            {   //possibly useful ASCII characters: ████░░░░░░ ████▒▒▒▒ ■□□□□ ▄▄▄... ▀▀    ═══───
                //returns a the loading bar
                if (!Plugin.configCookingBar.Value)
                {   //empty string if we don't want the bar
                    return "";
                }
                int barLength = 300; // Total length of the loading bar
                int filledLength = (int)(amount * barLength); // Calculate the number of filled characters

                if (filledLength <= 0 || filledLength >= barLength)
                {   //empty string if raw or fully cooked already.
                    return "";
                }
                else
                {
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
}
