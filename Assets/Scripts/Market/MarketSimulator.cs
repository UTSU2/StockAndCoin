using UnityEngine;
using Data;
using System.Collections.Generic;
using System.Linq;

public class MarketSimulator : MonoBehaviour
{
    [Header("Database")]
    public MarketDatabase database;

    [Header("Chart")]
    public ChartController chartController;

    [Header("Date")]
    public TimeManager timeManager;
    [SerializeField] private EventManager eventManager;

    private void OnEnable()
    {
        if (timeManager != null)
            timeManager.OnDayChanged += NextDay;
    }
    private void OnDisable()
    {
        if (timeManager != null)
            timeManager.OnDayChanged -= NextDay;
    }
    public void NextDay()
    {
        if (database == null)
        {
            Debug.LogWarning("MarketDatabase가 연결되지 않았습니다.");
            return;
        }
        if (eventManager == null)
        {
            Debug.LogWarning("EventManager가 연결되지 않았습니다.");
            return;
        }

        List<MarketEventData> todayEvents = eventManager.CheckRandomEvents();
        ApplyEventVolatility(todayEvents);

        foreach (AssetData asset in database.assets)
        {
            CandleChartData chartData = GetCandleChart(asset.id);

            if (chartData == null)
            {
                Debug.LogWarning($"{asset.id}의 CandleChartData가 없습니다.");
                continue;
            }

            CandleData prev = chartData.candles.LastOrDefault();

            if (prev == null)
            {
                Debug.LogWarning($"{asset.id}의 이전 캔들이 없습니다.");
                continue;
            }

            List<MarketEventData> assetEvents = todayEvents
                .Where(e =>
                    e.impacts != null &&
                    e.impacts.Any(i => i.assetId == asset.id)
                )
                .ToList();

            CandleData next =
                CandleGenerator.CreateStartCandle(
                    asset.id,
                    prev,
                    assetEvents,
                    GetCurrentDateString()
                );

            chartData.candles.Add(next);
        }

        if (chartController != null)
        {
            chartController.LoadChart(chartController.currentAssetId);
        }
    }
    private void ApplyEventVolatility(List<MarketEventData> todayEvents)
    {
        foreach (AssetData asset in database.assets)
        {
            float totalVolatilityImpact = 0f;
            foreach (MarketEventData marketEvent in todayEvents)
            {
                if (marketEvent.impacts == null)
                    continue;

                foreach (EventImpactData impact in marketEvent.impacts)
                {
                    if (impact.assetId == asset.id)
                    {
                        totalVolatilityImpact += Mathf.Abs(impact.volatilityImpact);
                    }
                }
            }
            if (totalVolatilityImpact > 0f)
            {
                asset.currentMoveRange = asset.baseMoveRange + totalVolatilityImpact;
            }
        }
    }

    private CandleChartData GetCandleChart(string assetId)
    {
        return database.candleCharts
            .FirstOrDefault(c => c.assetId == assetId);
    }

    private string GetCurrentDateString()
    {
        if (timeManager == null)
            return "";

        return $"{timeManager.year:D4}-{timeManager.month:D2}-{timeManager.day:D2}";
    }

}
