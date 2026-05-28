using UnityEngine;
using Data;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [SerializeField] private MarketDatabase database;

    private readonly List<MarketEventData> activeEvents = new();

    public void CheckEvents(GameDate currentDate)
    {
        foreach (MarketEventData marketEvent in database.events)
        {
            if (marketEvent.date == null)
                continue;

            if (IsSameDate(marketEvent.date, currentDate))
            {
                TriggerEvent(marketEvent);
            }
        }
    }

    private void TriggerEvent(MarketEventData marketEvent)
    {
        if (activeEvents.Contains(marketEvent))
            return;

        activeEvents.Add(marketEvent);

        Debug.Log($"이벤트 발생: {marketEvent.id}");

        ApplyEventImpact(marketEvent);
    }

    private void ApplyEventImpact(MarketEventData marketEvent)
    {
        if (marketEvent.impacts == null)
            return;

        foreach (var impact in marketEvent.impacts)
        {
            AssetData asset = database.assets.Find(a => a.id == impact.assetId);

            if (asset == null)
                continue;

            Debug.Log($"{asset.name} 영향 적용: {impact.assetId}");
        }
    }

    private bool IsSameDate(GameDate a, GameDate b)
    {
        return a.year == b.year
            && a.month == b.month
            && a.day == b.day;
    }
}
