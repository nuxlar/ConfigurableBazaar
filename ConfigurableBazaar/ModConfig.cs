using BepInEx.Configuration;
using System;

namespace ConfigurableBazaar
{
    internal class ModConfig
    {
        public static ConfigEntry<int> newtPortal;

        public static ConfigEntry<int> lunarItemLimit;
        public static ConfigEntry<int> lunarItemPrice;
        public static ConfigEntry<int> lunarSeerPrice;
        public static ConfigEntry<int> lunarRollPrice;
        public static ConfigEntry<int> lunarRollAmount;
        public static ConfigEntry<bool> respawnLunars;

        public static ConfigEntry<bool> spawnScrapper;
        public static ConfigEntry<bool> spawnR2WCauldron;
        public static ConfigEntry<bool> spawnG2WCauldron;
        public static ConfigEntry<bool> spawnCleansingPool;
        public static ConfigEntry<bool> redScrapCauldron;
        public static ConfigEntry<int> itemsToReforgeG;
        public static ConfigEntry<int> itemsToReforgeR;

        public static ConfigEntry<int> printerCount;
        public static ConfigEntry<float> tier1Chance;
        public static ConfigEntry<float> tier2Chance;
        public static ConfigEntry<float> tier3Chance;
        public static ConfigEntry<float> tierBossChance;

        public static void InitConfig(ConfigFile config)
        {
            newtPortal = config.Bind("General", "Newt Portal", 0, new ConfigDescription("Set which stages the blue portal should spawn (0: Vanilla) (12345: Every Stage)"));
            spawnScrapper = config.Bind("General", "Enable Scrapper", true, new ConfigDescription("Set if scrapper should spawn in the bazaar"));
            spawnCleansingPool = config.Bind("General", "Enable Cleansing Pool", false, new ConfigDescription("Set if cleansing pool should spawn in the bazaar"));

            lunarItemLimit = config.Bind("Lunar Shop", "Lunar Item Limit", 5, new ConfigDescription("Set how many lunar items each person can take from the bazaar"));
            lunarItemPrice = config.Bind("Lunar Shop", "Lunar Item Price", 2, new ConfigDescription("Set how much lunar items cost"));
            lunarSeerPrice = config.Bind("Lunar Shop", "Lunar Seer Price", 2, new ConfigDescription("Set how much the lunar seers cost"));
            lunarRollPrice = config.Bind("Lunar Shop", "Lunar Roll Price", -1, new ConfigDescription("Set how much the rolls should cost (-1: Vanilla)"));
            lunarRollAmount = config.Bind("Lunar Shop", "Lunar Roll Amount", -1, new ConfigDescription("Set how many times you can roll the lunar shop (-1: Vanilla)"));
            respawnLunars = config.Bind("Lunar Shop", "Respawn Lunars", false, new ConfigDescription("Set if lunar items should respawn after every roll"));

            redScrapCauldron = config.Bind("Lunar Cauldrons", "Enable Red Scrap Cauldron", true, new ConfigDescription("Set if red cauldrons should take red scrap"));
            spawnR2WCauldron = config.Bind("Lunar Cauldrons", "Enable Red to White Cauldron", false, new ConfigDescription("Set if Red to White Cauldron should spawn in the bazaar"));
            spawnG2WCauldron = config.Bind("Lunar Cauldrons", "Enable Green to White Cauldron", true, new ConfigDescription("Set if Green to White Cauldron should spawn in the bazaar"));
            itemsToReforgeG = config.Bind("Lunar Cauldrons", "Items To Reforge for Red Cauldron", 2, new ConfigDescription("Set how many items should be reforged for the Green to White cauldron"));
            itemsToReforgeR = config.Bind("Lunar Cauldrons", "Items To Reforge for Green Cauldron", 1, new ConfigDescription("Set how many items should be reforged for the Red to White cauldron"));

            printerCount = config.Bind("Printers", "Printer Count", 1, new ConfigDescription("Set how many 3D Printers should spawn in the bazaar. Maximum is 4"));
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
