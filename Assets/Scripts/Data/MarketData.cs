namespace Data
{
    public enum MarketType
    {
        Stock,
        Coin
    }

    public enum EventType
    {
        EarningsReport,   // 실적 발표
        InterestRate,     // 금리 발표
        Listing,          // 상장
        Delisting,        // 상장폐지
        Regulation,       // 규제
        News,             // 일반 뉴스
        Custom
    }

    [System.Serializable]
    public class AssetData
    {
        public string id;          // "AAPL", "BTC"
        public string name;        // "Apple", "Bitcoin"
        public MarketType marketType;
        public string symbol;      // 차트 표시용 심볼
    }

    [System.Serializable]
    public class MarketEventData
    {
        public string id;
        public string[] assetIds;     // 어떤 주식/코인에 연결되는지
        public EventType eventType;
        public string title;
        public string description;
        public string date;
        public float impactValue;  // 가격 영향도
    }

    [System.Serializable]
    public class CandleData
    {
        public string assetId;
        public string date;

        public float open;
        public float high;
        public float low;
        public float close;
        public float volume;
    }
}