using UnityEngine;
using Data;
using System.Collections.Generic;
using System.Linq;

public class MarketSimulator : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private MarketDatabase database;

    [Header("Chart")]
    [SerializeField] private ChartController chartController;

    [Header("Time")]
    [SerializeField] private TimeManager timeManager;

    [Header("Event")]
    [SerializeField] private EventManager eventManager;

    public event System.Action<List<MarketEventData>> OnMarketEventsOccurred;

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
        if (database == null || timeManager == null)
        {
            Debug.LogWarning(
                "[MarketSimulator] Database 또는 TimeManager가 없습니다."
            );
            return;
        }

        string currentDate = timeManager.GetDateText();

        foreach (AssetData asset in database.assets)
        {
            if (!asset.isListed || !asset.isAvailable)
                continue;

            CandleChartData chartData = GetCandleChart(asset.id);

            if (chartData == null || chartData.candles == null)
                continue;

            CandleData prev = chartData.candles.LastOrDefault();

            if (prev == null)
                continue;

            if (prev.date == currentDate)
                continue;

            CandleData next =
                CandleGenerator.CreateStartCandle(
                    prev,
                    currentDate
                );

            chartData.candles.Add(next);
        }

        if (eventManager != null)
        {
            List<MarketEventData> todayEvents =
                eventManager.CheckRandomEvents();

            OnMarketEventsOccurred?.Invoke(todayEvents);
        }

        RefreshChart();
    }

    private CandleChartData GetCandleChart(string assetId)
    {
        return database.candleCharts
            .FirstOrDefault(c => c.assetId == assetId);
    }

    private void RefreshChart()
    {
        if (chartController == null)
            return;

        chartController.LoadChart(
            chartController.currentAssetId
        );
    }
}
