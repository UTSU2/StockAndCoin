using UnityEngine;
using Data;
using System.Collections.Generic;


public class CandleGenerator
{
    public static CandleData CreateStartCandle(
        string assetId,
        CandleData prev,
        List<MarketEventData> events,
        string nextDate)
    {
        float eventRate = 0f;

        foreach (MarketEventData marketEvent in events)
        {
            if (marketEvent.impacts == null)
                continue;

            foreach (EventImpactData impact in marketEvent.impacts)
            {
                if (impact.assetId == assetId)
                    eventRate += impact.impactValue;
            }
        }

        float startPrice = prev.close * (1f + eventRate);

        return new CandleData
        {
            date = nextDate,
            open = startPrice,
            high = startPrice,
            low = startPrice,
            close = startPrice,
            volume = 0f
        };
    }
}
