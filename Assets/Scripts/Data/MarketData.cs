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
    public enum AssetStateAction
    {
        None,
        List,
        Delist,
        Reveal
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
        public float baseMoveRange; // 기본 변동성
        public float currentMoveRange; //현재 변동성
        public bool isListed;  // 거래 가능 여부
        public bool isAvailable; // 시장 등장 여부
    }

    [System.Serializable]
    public class OrderBookLevel
    {
        public float price;
        public int quantity;

        public OrderBookLevel(float price, int quantity)
        {
            this.price = price;
            this.quantity = quantity;
        }
    }

    [System.Serializable]
    public class OrderBookData
    {
        public string assetId;

        public List<OrderBookLevel> buyOrders = new();
        public List<OrderBookLevel> sellOrders = new();

        public OrderBookData(string assetId)
        {
            this.assetId = assetId;
        }
    }

    [System.Serializable]
    public class EventImpactData
    {
        public string assetId;
        public float impactValue; //시가 영향
        public float volatilityImpact; // 변동성 영향
        public AssetStateAction stateAction; //상태 변화
    }
    [System.Serializable]
    public class EventToEventImpactData
    {
        public string eventId;
        public float probabilityChange;
    }

    [System.Serializable]
    public class MarketEventData
    {
        public string id;
        public List<EventImpactData> impacts = new();
        public List<EventToEventImpactData> eventImpacts = new();
        public List<string> prerequisiteEventIds = new();
        public EventType eventType;
        public string title;
        public string description;
        public GameDate date;  // 삭제 예정
        public bool isArise;
        public bool canArise;
        public float probability; // 사건이 일어날 확률
    }

    [System.Serializable]
    public class CandleData
    {
        public string date;
        public float open;
        public float high;
        public float low;
        public float close;
        public float volume;
    }

    [System.Serializable]
    public class CandleChartData
    {
        public string assetId;
        public List<CandleData> candles = new();
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