using BepInEx;
using R2API.Utils;
using System.Collections.Generic;
using RoR2;
using RoR2.Skills;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ConfigurableBazaar
{
  [BepInPlugin("com.zorp.ConfigurableBazaar", "ConfigurableBazaar", "1.0.0")]

  public class ConfigurableBazaar : BaseUnityPlugin
  {
    private Bazaar bazaar;
    private readonly System.Random r = new();
    private int rerolls = -1;
    // private SeerStationController thirdSeerStationController;
    private Dictionary<int, Printer> printerDict = new();
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
      bazaar = new Bazaar();
      On.RoR2.Run.Start += OnRunStart;
      On.RoR2.BazaarController.Awake += StartBazaar;
      On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += NewtPortalSpawner;
      On.RoR2.BazaarController.SetUpSeerStations += SetUpSeerStations;
      On.RoR2.CostHologramContent.FixedUpdate += CostHologramContent_FixedUpdate;
      if (ModConfig.lunarRollAmount.Value != -1)
        On.RoR2.PurchaseInteraction.SetAvailable += SetAvailable;
      On.RoR2.PurchaseInteraction.OnInteractionBegin += OnInteractionBegin;
      On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += CanBeAffordedByInteractor;
      On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer += GenerateNewPickupServer;
      if (ModConfig.lunarRollPrice.Value != -1)
        On.RoR2.PurchaseInteraction.ScaleCost += ScaleCost;
    }

    private void OnRunStart(On.RoR2.Run.orig_Start orig, Run self)
    {
        AdjustNewt();
        orig(self);
    }

    private void AdjustNewt()
    {
        GameObject Newt = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/ShopkeeperBody");
        SkillLocator SklLocate = Newt.GetComponent<SkillLocator>();
        SkillFamily Ult = SklLocate.special.skillFamily;
        Ult.variants[0].skillDef = new SkillDef();
    }

    private void StartBazaar(On.RoR2.BazaarController.orig_Awake orig, BazaarController self)
    {
      if (!NetworkServer.active)
        return;
      rerolls = -1;
      bazaar.ResetBazaarPlayers();
      SpawnScrapper();
      SpawnCauldrons();
      SpawnCleansingPool();
      SpawnPrinters();
      // SpawnThirdSeer();
      //self.seerStations[2] = thirdSeerStationController;
      StartCoroutine(BroadcastShopSettings());
      if (ModConfig.lunarRollPrice.Value != -1)
        StartCoroutine(ChangeShopPrices());
      orig(self);
    }

    private IEnumerator<WaitForSeconds> BroadcastShopSettings()
    {
        yield return new WaitForSeconds(2f);
        string redCauldronText = ModConfig.spawnR2WCauldron.Value ? $"| Red to white cauldron takes {ModConfig.itemsToReforgeR.Value} red(s) for 3 whites" : "";
        string greenCauldronText = ModConfig.spawnG2WCauldron.Value ? $"| Green to white cauldron takes {ModConfig.itemsToReforgeG.Value} green(s) for 3 whites" : "";
        string rerollText = ModConfig.lunarRollAmount.Value != -1 ? $"| The shop can be rerolled {ModConfig.lunarRollAmount.Value} time(s)" : "";
        Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
        {
            baseToken = $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.green)}>Each player can get {ModConfig.lunarItemLimit.Value} lunar(s) {rerollText} {greenCauldronText} {redCauldronText}</color>"
        });
    }

    private IEnumerator<WaitForSeconds> ChangeShopPrices()
    {
        yield return new WaitForSeconds(2f);
        List<PurchaseInteraction> purchaseInteractions = InstanceTracker.GetInstancesList<PurchaseInteraction>();
        foreach (PurchaseInteraction purchaseInteraction in purchaseInteractions)
        {
            if (purchaseInteraction.name.StartsWith("LunarShop") && ModConfig.lunarRollPrice.Value >= 0)
                purchaseInteraction.Networkcost = ModConfig.lunarItemPrice.Value;
            else if (purchaseInteraction.name.StartsWith("LunarRecycler") && ModConfig.lunarRollPrice.Value >= 0)
                purchaseInteraction.Networkcost = ModConfig.lunarRollPrice.Value;
        }
    }


    public void ScaleCost(On.RoR2.PurchaseInteraction.orig_ScaleCost orig, PurchaseInteraction self, float scalar)
    {
        if (self.name.StartsWith("LunarRecycler"))
            scalar = ModConfig.lunarRollPrice.Value;
        orig(self, scalar);
    }

    private static void SetUpSeerStations(On.RoR2.BazaarController.orig_SetUpSeerStations orig, BazaarController self)
    {
        orig(self);
        foreach (SeerStationController seerStationController in self.seerStations)
            seerStationController.GetComponent<PurchaseInteraction>().Networkcost = ModConfig.lunarSeerPrice.Value;
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


    /** private void SpawnThirdSeer()
    {
        if (ModConfig.spawnExtraSeer.Value)
        {
            // SceneDef nextStageScene = Run.instance.nextStageScene;
            Vector3 position = new(-132.6f, -25f, -6.7f);
            Vector3 rotation = new(0.0f, 132f, 0.0f);
            GameObject seerStation = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SeerStation"), position, Quaternion.identity);
            thirdSeerStationController = seerStation.GetComponent<SeerStationController>();
            seerStation.transform.eulerAngles = rotation;
            NetworkServer.Spawn(seerStation);
        }
    } **/

    private void SpawnCauldrons()
    {
        if (ModConfig.spawnG2WCauldron.Value)
        {
            GameObject g2wCauldron = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/LunarCauldron, RedToWhite Variant"), new Vector3(-132f, -25.7f, -29.7f), Quaternion.identity);
            g2wCauldron.GetComponent<PurchaseInteraction>().costType = CostTypeIndex.GreenItem;
            g2wCauldron.GetComponent<PurchaseInteraction>().Networkcost = ModConfig.itemsToReforgeG.Value;
            g2wCauldron.transform.eulerAngles = new Vector3(0.0f, 66f, 0.0f);
            NetworkServer.Spawn(g2wCauldron);
        } 
        if (ModConfig.spawnR2WCauldron.Value)
        {
            GameObject r2wCauldron = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/LunarCauldron, RedToWhite Variant"), new Vector3(-123.5f, -23.6f, -4.8f), Quaternion.identity);
            r2wCauldron.GetComponent<PurchaseInteraction>().Networkcost = ModConfig.itemsToReforgeR.Value;
            r2wCauldron.transform.eulerAngles = new Vector3(0.0f, 80f, 0.0f);
            NetworkServer.Spawn(r2wCauldron);
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
    private bool CheckIfInteractorHasItem(Interactor interactor, ItemDef itemDef) => interactor.GetComponent<CharacterBody>().inventory.GetItemCount(itemDef) > 0;
    private bool IsRedCauldron(PurchaseInteraction interaction) => interaction.costType == CostTypeIndex.GreenItem && interaction.cost == 5;
    
    private bool CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator)
    {   
        if (ModConfig.redScrapCauldron.Value && IsRedCauldron(self) && CheckIfInteractorHasItem(activator, RoR2Content.Items.ScrapRed))
            return true;
        return orig(self, activator);
    }
    private void OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
    {
        NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
        BazaarPlayer bazaarPlayer = bazaar.GetBazaarPlayer(networkUser);
        // red scrap logic
        if (IsRedCauldron(self) && CheckIfInteractorHasItem(activator, RoR2Content.Items.ScrapRed) && ModConfig.redScrapCauldron.Value)
        {
            self.costType = CostTypeIndex.RedItem;
            self.cost = 1;
            orig(self, activator);
            self.costType = CostTypeIndex.GreenItem;
            self.cost = 5;
        // lunar shop logic
        } else if (self.name.StartsWith("LunarShop"))
        {
            if (bazaarPlayer.lunarPurchases < ModConfig.lunarItemLimit.Value)
            {
                orig(self, activator);
                bazaarPlayer.lunarPurchases++;
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
                {
                    baseToken = $"{networkUser.userName} you can buy {ModConfig.lunarItemLimit.Value - bazaarPlayer.lunarPurchases} more lunar(s)"
                });
            }
            else
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
                {
                    baseToken = $"{networkUser.userName} you've reached your lunar limit"
                });
                return;
            }
        } else
            orig(self, activator);
    }

    private void CostHologramContent_FixedUpdate(On.RoR2.CostHologramContent.orig_FixedUpdate orig, CostHologramContent self)
    {
        if (self.displayValue == 5 && self.costType == CostTypeIndex.GreenItem)
        {
            CostHologramContent.sharedStringBuilder.Clear();
            self.targetTextMesh.color = Color.white;
            self.targetTextMesh.SetText($"<nobr><color=#{ColorUtility.ToHtmlStringRGBA(Color.red)}>1 Scrap(s)</color></nobr><br>OR<br><nobr><color=#{ColorUtility.ToHtmlStringRGBA(Color.green)}>5 Item(s)</color></nobr>");
        }
        else
            orig(self);
    }

    [Server]
    private void SetAvailable(On.RoR2.PurchaseInteraction.orig_SetAvailable orig, PurchaseInteraction self, bool newAvailable)
    {
        if (self.name.StartsWith("LunarRecycler") && ModConfig.lunarRollAmount.Value >= rerolls)
        {
            orig(self, newAvailable);
            rerolls++;
        }
        else if (self.name.StartsWith("LunarRecycler") && ModConfig.lunarRollAmount.Value <= rerolls)
            orig(self, false);
        else
            orig(self, newAvailable);
    }

    [Server]
    private void GenerateNewPickupServer(On.RoR2.ShopTerminalBehavior.orig_GenerateNewPickupServer orig, ShopTerminalBehavior self)
    {
        if (ModConfig.respawnLunars.Value && self.name.StartsWith("LunarShop"))
            self.NetworkhasBeenPurchased = false;
        orig(self);
        if (ModConfig.respawnLunars.Value && self.name.StartsWith("LunarShop"))
            self.GetComponent<PurchaseInteraction>().SetAvailable(true);
    }
  }
}
