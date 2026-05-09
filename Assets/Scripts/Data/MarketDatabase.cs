using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Data/Market Database")]
    public class MarketDatabase : ScriptableObject
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
                .Where(e => e.impacts != null && e.impacts.Any(i => i.assetId == assetId))
                .ToList();
        }
        public List<AssetData> GetAssetsByEvent(string eventid)
        {
            MarketEventData marketEvent
                = events.FirstOrDefault(e => e.id == eventid);

            if (marketEvent == null || marketEvent.impacts == null)
                return new List<AssetData>();

            return assets
                .Where(a => marketEvent.impacts.Any(i => i.assetId == a.id))
                .ToList();
        }
    }
}