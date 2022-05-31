using RoR2;

namespace ConfigurableBazaar
{
    internal class BazaarPlayer
    {
        public NetworkUser networkUser;
        public int lunarPurchases;

        public BazaarPlayer(NetworkUser networkUser)
        {
            this.networkUser = networkUser;
            lunarPurchases = 0;
        }
        internal void pickupLunar()
        {
            ++lunarPurchases;
        }
    }
}
