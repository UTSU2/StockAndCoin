using UnityEngine;
using Data;
using System.Collections.Generic;
using System.Linq;

public class EventManager : MonoBehaviour
{
    [SerializeField] private MarketDatabase database;

    private readonly List<MarketEventData> activeEvents = new();

    public List<MarketEventData> CheckRandomEvents()
    {
        UpdateCanAriseStates();

        List<MarketEventData> occurredEvents = new();

        List<MarketEventData> sortedEvents = database.events
            .Where(e => !e.isArise && e.canArise)
            .OrderByDescending(e => e.probability)
            .ToList();

        int occurCount = Random.Range(0, 11);
        foreach (MarketEventData marketEvent in sortedEvents)
        {
            if (occurredEvents.Count >= occurCount)
                break;

            float randomValue = Random.Range(0f, 100f);

            if (randomValue <= marketEvent.probability)
            {
                TriggerEvent(marketEvent);
                occurredEvents.Add(marketEvent);
            }
        }

        return occurredEvents;
    }
    private void UpdateCanAriseStates()
    {
        foreach (MarketEventData marketEvent in database.events)
        {
            if (marketEvent.isArise)
            {
                marketEvent.canArise = false;
                continue;
            }

            marketEvent.canArise = CanEventArise(marketEvent);
        }
    }
    private bool CanEventArise(MarketEventData marketEvent)
    {
        if (marketEvent.prerequisiteEventIds.Count == 0)
            return true;

        foreach (string prerequisiteId in marketEvent.prerequisiteEventIds)
        {
            MarketEventData prerequisiteEvent =
                database.events.FirstOrDefault(e => e.id == prerequisiteId);

            if (prerequisiteEvent == null)
                return false;

            if (!prerequisiteEvent.isArise)
                return false;
        }

        return true;
    }

    private void TriggerEvent(MarketEventData marketEvent)
    {
        if (marketEvent.isArise)
            return;

        marketEvent.isArise = true;
        activeEvents.Add(marketEvent);

        Debug.Log($"이벤트 발생: {marketEvent.title}");

        ApplyAssetImpact(marketEvent);
        ApplyEventToEventImpact(marketEvent);
    }

    private void ApplyAssetImpact(MarketEventData marketEvent)
    {
        if (marketEvent.impacts == null)
            return;

        foreach (EventImpactData impact in marketEvent.impacts)
        {
            AssetData asset = database.assets.Find(a => a.id == impact.assetId);

            if (asset == null)
                continue;

            Debug.Log($"{asset.name} 자산 영향 적용");
        }
    }

    private void ApplyEventToEventImpact(MarketEventData marketEvent)
    {
        if (marketEvent.eventImpacts == null)
            return;

        foreach (EventToEventImpactData eventImpact in marketEvent.eventImpacts)
        {
            MarketEventData targetEvent =
                database.events.Find(e => e.id == eventImpact.eventId);

            if (targetEvent == null)
                continue;

            if (targetEvent.isArise)
                continue;

            targetEvent.probability += eventImpact.probabilityChange;
            targetEvent.probability = Mathf.Clamp(targetEvent.probability, 0f, 100f);

            Debug.Log(
                $"{targetEvent.title} 발생 확률 변경: {targetEvent.probability}%"
            );
        }
    }
}
