using UnityEngine;
using Data;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class MarketSimulator : MonoBehaviour
{
    [Header("Database")]
    public MarketDatabase database;

    [Header("Chart")]
    public ChartController chartController;

    [Header("Date")]
    public GameDate currentDate;
    [SerializeField] private EventManager eventManager;

    public void NextDay()
    {
        if (database == null)
        {
            Debug.LogWarning("MarketDatabase가 연결되지 않았습니다.");
            return;
        }

        List<MarketEventData> todayEvents = eventManager.CheckRandomEvents();

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
                    GetNextDateString()
                );

            chartData.candles.Add(next);
        }

        AddOneDay();

        if (chartController != null)
        {
            chartController.LoadChart(chartController.currentAssetId);
        }
    }

    private CandleChartData GetCandleChart(string assetId)
    {
        return database.candleCharts
            .FirstOrDefault(c => c.assetId == assetId);
    }

    private List<MarketEventData> GetEventsByAssetAndDate(string assetId, GameDate date)
    {
        return database.events
            .Where(e =>
                e.date != null &&
                e.date.year == date.year &&
                e.date.month == date.month &&
                e.date.day == date.day &&
                e.impacts != null &&
                e.impacts.Any(i => i.assetId == assetId)
            )
            .ToList();
    }

    private string GetNextDateString()
    {
        System.DateTime current = new System.DateTime(
            currentDate.year,
            currentDate.month,
            currentDate.day
        );

        return current.AddDays(1).ToString("yyyy-MM-dd");
    }

    private void AddOneDay()
    {
        System.DateTime current = new System.DateTime(
            currentDate.year,
            currentDate.month,
            currentDate.day
        );

        current = current.AddDays(1);

        currentDate.year = current.Year;
        currentDate.month = current.Month;
        currentDate.day = current.Day;
    }
}
