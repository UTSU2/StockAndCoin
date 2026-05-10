using System.Collections.Generic;

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
    public enum InfoGrade
    {
        Low,
        Normal,
        High,
        Premium,
        Insider
    }

    public enum PlayerInfoRank
    {
        Beginner,
        Normal,
        Expert,
        VIP,
        Legend
    }

    [System.Serializable]
    public class GameDate
    {
        public int year;
        public int month;
        public int day;
    }

    [System.Serializable]
    public class AssetData
    {
        public string id;          // "AAPL", "BTC"
        public string name;        // "Apple", "Bitcoin"
        public MarketType marketType;
        public string symbol;      // 차트 표시용 심볼
        public float basePrice;    // 초기 기준 가격
    }

    [System.Serializable]
    public class EventImpactData
    {
        public string assetId;
        public float impactValue;
    }

    [System.Serializable]
    public class MarketEventData
    {
        public string id;
        public List<EventImpactData> impacts = new();
        public EventType eventType;
        public string title;
        public string description;
        public GameDate date;
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

    [System.Serializable]
    public class InformantOffer
    {
        public string offerId;

        // 내부적으로만 사용, UI에는 표시하지 않음
        public string eventId;

        public InfoGrade infoGrade;
        public int price;

        public bool purchased;
    }
    [System.Serializable]
    public class PurchasedInfo
    {
        public string eventId;

        public InfoGrade infoGrade;
        public PlayerInfoRank playerRank;

        public string revealedText;

        public float accuracy;
    }
}