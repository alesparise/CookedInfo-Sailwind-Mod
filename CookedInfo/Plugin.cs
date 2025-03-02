using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace CookedInfo
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "pr0skynesis.cookedinfo";
        public const string PLUGIN_NAME = "Cooked Info";
        public const string PLUGIN_VERSION = "1.2.0";

        internal static Plugin instance;
        internal static ManualLogSource logger;

        internal static ConfigEntry<bool> configColoredText;
        internal static ConfigEntry<bool> configCookingPercent;
        internal static ConfigEntry<bool> configCookingBar;
        internal static ConfigEntry<bool> configFreshnessBar;

        private void Awake()
        {
            instance = this;
            logger = Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);

            configColoredText = Config.Bind("Colored Text", "showColoredText", true, "Enables the colored text for raw, cooked and burnt food, change to false to disable it.");
            configCookingPercent = Config.Bind("Cooking Percentage", "showCookingPercent", true, "Enables the cooking percentage when cooking food, change to false to disable it.");
            configCookingBar = Config.Bind("Loading Bar", "showCookingBar", true, "Enables the cooking loading bar when cooking food, change to false to disable it.");
            configFreshnessBar = Config.Bind("Freshness Bar", "showFreshnessBar", true, "Enables the freshness loading bar, change to false to disable it.");
        }
    }    
}
