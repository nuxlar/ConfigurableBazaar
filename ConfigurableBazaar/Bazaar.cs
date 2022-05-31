using System.Collections.Generic;
using RoR2;

namespace ConfigurableBazaar
{
    internal class Bazaar
    {
        private List<BazaarPlayer> bazaarPlayers = new List<BazaarPlayer>();
        private void CreateBazaarPlayers()
        {
            for (int index = 0; index < PlayerCharacterMasterController.instances.Count; ++index)
            {
                PlayerCharacterMasterController instance = PlayerCharacterMasterController.instances[index];
                BazaarPlayer bazaarPlayer;
                bazaarPlayer = new BazaarPlayer(instance.networkUser);
                bazaarPlayers.Add(bazaarPlayer);
            }
        }

        public BazaarPlayer GetBazaarPlayer(NetworkUser networkUser)
        {
            for (int index = 0; index < bazaarPlayers.Count; ++index)
            {
                if (bazaarPlayers[index].networkUser == networkUser)
                    return bazaarPlayers[index];
            }
            return null;
        }

        public void ResetBazaarPlayers()
        {
            bazaarPlayers.Clear();
            CreateBazaarPlayers();
        }
        public bool PlayerHasPurchasesLeft(BazaarPlayer bazaarPlayer) => ModConfig.lunarItemLimit.Value <= -1 || bazaarPlayer.lunarPurchases < ModConfig.lunarItemLimit.Value;
        public List<BazaarPlayer> GetBazaarPlayers() => bazaarPlayers;
    }
}
