using BepInEx;
using R2API.Utils;
using System.Collections.Generic;
using RoR2;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ConfigurableBazaar
{
  [BepInPlugin("com.zorp.ConfigurableBazaar", "ConfigurableBazaar", "0.6.5")]
  [NetworkCompatibility]

  public class ConfigurableBazaar : BaseUnityPlugin
  {

    // private Bazaar bazaar;
    private readonly System.Random r = new System.Random();
    private Dictionary<int, Printer> printerDict = new Dictionary<int, Printer>();
    private string[] printers = new string[4]
    {
          "iscDuplicator",
          "iscDuplicatorLarge",
          "iscDuplicatorMilitary",
          "iscDuplicatorWild"
    };

    private Dictionary<int, string[]> stages = new Dictionary<int, string[]>
        {
            {1, new string[5] { "blackbeach", "blackbeach2", "golemplains", "golemplains2", "snowyforest"} },
            {2, new string[3] { "goolake", "foggyswamp", "ancientloft" } },
            {3, new string[3] { "frozenwall", "wispgraveyard", "sulfurpools" } },
            {4, new string[3] { "dampcavesimple", "shipgraveyard", "rootjungle" } },
            {5, new string[1] { "skymeadow" } },
        };
    int[] stageNums;
    public void Awake()
    {
      ModConfig.InitConfig(Config);
      stageNums = SanitizeNewtPortalConfig();
      // bazaar = new Bazaar();
      On.RoR2.BazaarController.Awake += StartBazaar;
      On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += NewtPortalSpawner;
      On.EntityStates.NewtMonster.KickFromShop.OnEnter += (orig, self) => { };
      // On.RoR2.SceneExitController.Begin += new On.RoR2.SceneExitController.hook_Begin(SceneExitController_Begin);
      // On.RoR2.Run.AdvanceStage += new On.RoR2.Run.hook_AdvanceStage(Run_AdvanceStage);
        }

    private void StartBazaar(On.RoR2.BazaarController.orig_Awake orig, RoR2.BazaarController self)
    {
      if (!NetworkServer.active)
        return;
      SpawnScrapper();
      SpawnCauldron();
      SpawnCleansingPool();
      SpawnPrinters();
      // bazaar.ResetBazaarPlayers();
      orig.Invoke(self);
    }

    private void FillPrinterInfo()
    {
      printerDict.Add(0, new Printer(new Vector3(-133.3f, -25.7f, -17.9f), new Vector3(0.0f, 72.6f, 0.0f)));
      printerDict.Add(1, new Printer(new Vector3(-71.3f, -24.7f, -29.2f), new Vector3(0.0f, 291f, 0.0f)));
      printerDict.Add(2, new Printer(new Vector3(-143.8f, -24.7f, -23.6f), new Vector3(0.0f, 61f, 0.0f)));
      printerDict.Add(3, new Printer(new Vector3(-110.7f, -26.7f, -46.4f), new Vector3(0.0f, 32.2f, 0.0f)));
    }

    private void SpawnPrinters()
    {
      printerDict.Clear();
      FillPrinterInfo();
      for (int key = 0; key < ModConfig.printerCount.Value; ++key)
        SpawnInstance(printerDict[key].position, printerDict[key].rotation, GetRandomPrinter());
    }

    private void SpawnScrapper()
    {
      if (ModConfig.spawnScrapper.Value)
        SpawnInstance(new Vector3(-82.1f, -23.7f, -5.2f), new Vector3(0.0f, 72.6f, 0.0f), "iscScrapper");
    }

    private void SpawnCleansingPool()
    {
      if (ModConfig.spawnCleansingPool.Value)
        SpawnInstance(new Vector3(-65.7f, -23.5f, -18.9f), new Vector3(0.0f, 72.6f, 0.0f), "iscShrineCleanse");
    }

    private void SpawnInstance(Vector3 position, Vector3 rotation, string instanceType)
    {
      DirectorPlacementRule placementRule = new DirectorPlacementRule();
      placementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
      SpawnCard spawnCard = LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/" + instanceType);
      GameObject spawnedInstance = spawnCard.DoSpawn(position, Quaternion.identity, new DirectorSpawnRequest(spawnCard, placementRule, Run.instance.runRNG)).spawnedInstance;
      spawnedInstance.transform.eulerAngles = rotation;
      NetworkServer.Spawn(spawnedInstance);
    }
    private string GetRandomPrinter()
    {
      double tierChances = (double)ModConfig.tier1Chance.Value + (double)ModConfig.tier2Chance.Value + (double)ModConfig.tier3Chance.Value + (double)ModConfig.tierBossChance.Value;
      double tier = r.NextDouble() * tierChances;
      if (tier <= (double)ModConfig.tier1Chance.Value)
        return printers[0];
      if (tier <= (double)ModConfig.tier1Chance.Value + (double)ModConfig.tier2Chance.Value)
        return printers[1];
      return tier <= (double)ModConfig.tier1Chance.Value + (double)ModConfig.tier2Chance.Value + (double)ModConfig.tier3Chance.Value ? printers[2] : printers[3];
    }

    private void SpawnCauldron()
    {
      if (ModConfig.spawnCauldron.Value)
      {
        GameObject gameObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/LunarCauldron, RedToWhite Variant");
        if (ModConfig.greenToWhite.Value)
            gameObject.GetComponent<PurchaseInteraction>().costType = CostTypeIndex.GreenItem;
        gameObject.GetComponent<PurchaseInteraction>().Networkcost = ModConfig.itemsToReforge.Value;
        GameObject cauldron = Instantiate(gameObject, new Vector3(-132f, -25.7f, -29.7f), Quaternion.identity);
        cauldron.transform.eulerAngles = new Vector3(0.0f, 66f, 0.0f);
        NetworkServer.Spawn(cauldron);
      }
    }

    private int[] SanitizeNewtPortalConfig()
    {
      int[] numbers = new int[5] { 0, 0, 0, 0, 0 };
      int value = ModConfig.newtPortal.Value;

      if (value > 12345 || value <= 0)
        return new int[0];

      for (; value > 0; value /= 10)
      {
        int digit = value % 10;
        numbers[digit - 1] = digit;
      }

      return numbers;
    }

    private async void NewtPortalSpawner(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnAllEligiblePortals orig, TeleporterInteraction self)
    {
        if (stageNums.Length > 0)
        {
            int[] nums = new int[5] { 1, 2, 3, 4, 5 };
            await Task.Run(() =>
            {
                foreach (int num in nums)
                {
                    foreach (string stage in stages[num])
                    {
                        if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName(stage))
                        {
                            if (stageNums[num - 1] == 0)
                                self.shouldAttemptToSpawnShopPortal = false;
                            else
                                self.shouldAttemptToSpawnShopPortal = true;
                            return;
                        }
                    }
                }
            });
        }
      orig(self);
    }

        /**
        private bool CheckIfInteractorHasItem(Interactor interactor, ItemDef itemDef) => CheckIfCharacterMasterHasItem(interactor.GetComponent<CharacterBody>(), itemDef);
        private bool CheckIfCharacterMasterHasItem(CharacterBody characterBody, ItemDef itemDef) => characterBody.inventory.GetItemCount(itemDef) > 0;

        private bool PurchaseInteraction_CanBeAffordedByInteractor(
           On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor methodReference,
           PurchaseInteraction classReference,
           Interactor interactor)
        {
            return classReference.costType == CostTypeIndex.GreenItem && classReference.cost == 5 && this.CheckIfInteractorHasItem(interactor, RoR2Content.Items.ScrapRed) || methodReference.Invoke(classReference, interactor);
        }

         private void SceneExitController_Begin(
            On.RoR2.SceneExitController.orig_Begin orig,
            SceneExitController self)
            {
            if (NetworkServer.active && (bool) self.destinationScene && self.destinationScene.baseSceneName.Contains("bazaar") && !SceneInfo.instance.sceneDef.baseSceneName.Contains("bazaar"))
                bazaar.ResetBazaarPlayers();
                orig.Invoke(self);
            }

            private void Run_AdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
            {
            if (!SceneExitController.isRunning && nextScene.baseSceneName == "bazaar")
                bazaar.ResetBazaarPlayers();
                orig.Invoke(self, nextScene);
            }
        **/
    }
}
