using BepInEx.Configuration;
using System.Collections.Generic;
using System;

namespace ConfigurableBazaar
{
    internal class ModConfig
    {
        public static ConfigEntry<int> lunarItemLimit;
        public static ConfigEntry<int> newtPortal;
        public static ConfigEntry<bool> spawnScrapper;
        public static ConfigEntry<bool> spawnCauldron;
        public static ConfigEntry<bool> spawnCleansingPool;
        public static ConfigEntry<bool> greenToWhite;
        public static ConfigEntry<int> itemsToReforge;
        public static ConfigEntry<int> printerCount;
        public static ConfigEntry<float> tier1Chance;
        public static ConfigEntry<float> tier2Chance;
        public static ConfigEntry<float> tier3Chance;
        public static ConfigEntry<float> tierBossChance;

        public static void InitConfig(ConfigFile config)
        {
            lunarItemLimit = config.Bind("General", "Lunar Item Limit", 5, new ConfigDescription("**DOESNT WORK** Set how many lunar items each person can take from the bazaar"));
            newtPortal = config.Bind("General", "Newt Portal", 0, new ConfigDescription("Set which stages the blue portal should spawn (0: Vanilla) (12345: Every Stage)"));
            spawnScrapper = config.Bind("General", "Enable Scrapper", true, new ConfigDescription("Set if scrapper should spawn in the bazaar"));
            spawnCleansingPool = config.Bind("General", "Enable Cleansing Pool", false, new ConfigDescription("Set if cleansing pool should spawn in the bazaar"));
            spawnCauldron = config.Bind("Lunar Cauldron", "Enable Reforge Cauldron", true, new ConfigDescription("Set if cauldron should spawn in the bazaar"));
            greenToWhite = config.Bind("Lunar Cauldron", "Green To White", true, new ConfigDescription("Set if cauldron should reforge greens (instead of a red) to 3 whites"));
            itemsToReforge = config.Bind("Lunar Cauldron", "Items To Reforge", 2, new ConfigDescription("Set how many items should be reforged (Defaults - green: 2, red: 1)"));
            printerCount = config.Bind("Printers", "Printer Count", 1, new ConfigDescription("Set how many 3D Printers should spawn in the bazaar. Maximum is 4"));
            printerCount.Value = Math.Abs(printerCount.Value);
            if (printerCount.Value > 4)
                printerCount.Value = 4;
            tier1Chance = config.Bind("Printers", "Tier 1 Chance", 0.7f, new ConfigDescription("Set how likely it is for a bazaar 3D Printer to be tier 1"));
            tier1Chance.Value = Math.Abs(tier1Chance.Value);
            tier2Chance = config.Bind("Printers", "Tier 2 Chance", 0.2f, new ConfigDescription("Set how likely it is for a bazaar 3D Printer to be tier 2"));
            tier2Chance.Value = Math.Abs(tier2Chance.Value);
            tier3Chance = config.Bind("Printers", "Tier 3 Chance", 0.05f, new ConfigDescription("Set how likely it is for a bazaar 3D Printer to be tier 3"));
            tier3Chance.Value = Math.Abs(tier3Chance.Value);
            tierBossChance = config.Bind("Printers", "Boss Tier Chance", 0.05f, new ConfigDescription("Set how likely it is for a bazaar 3D Printer to be boss tier"));
            tierBossChance.Value = Math.Abs(tierBossChance.Value);
        }
    }
}
