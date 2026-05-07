using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class MarketDatabase
    {
        public List<AssetData> assets = new();
        public List<MarketEventData> events = new();
        public List<CandleData> candles = new();

        public List<CandleData> GetCandlesByAsset(string assetId)
        {
            return candles
                .Where(c => c.assetId == assetId)
                .ToList();
        }

        public List<MarketEventData> GetEventsByAsset(string assetId)
        {
            return events
                .Where(e => e.assetIds != null && e.assetIds.Contains(assetId))
                .ToList();
        }
    }
}