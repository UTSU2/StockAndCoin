using System.Collections.Generic;

namespace Data
{
    [System.Serializable]
    public class PlayerData
    {
        public float cash = 1000000f;
        public PlayerInfoRank infoRank = PlayerInfoRank.Beginner;
        public List<PlayerHolding> holdings = new();
        public List<PurchasedInfo> purchasedInfos = new();
    }

    [System.Serializable]
    public class PlayerHolding
    {
        public string assetId;
        public int quantity;
        public float averagePrice;
    }
}
