using UnityEngine;
using Data;

public class RealtimeMarketUpdater : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private MarketDatabase database;

    [Header("Chart")]
    [SerializeField] private ChartController chartController;

    [Header("Realtime Setting")]
    public TimeManager timeManager;
    public float updateInterval = 1f;
    public float volumePerTickMin = 50f;
    public float volumePerTickMax = 300f;

    private float timer;

    public void UpdateTradePrice(
        string assetId,
        float tradePrice,
        int quantity)
    {
        if (database == null)
            return;

        if (tradePrice <= 0f || quantity <= 0)
            return;

        CandleChartData chart = database.candleCharts
            .Find(c => c.assetId == assetId);

        if (chart == null ||
            chart.candles == null ||
            chart.candles.Count == 0)
            return;

        CandleData candle =
            chart.candles[chart.candles.Count - 1];

        candle.close = tradePrice;

        if (tradePrice > candle.high)
            candle.high = tradePrice;

        if (tradePrice < candle.low)
            candle.low = tradePrice;

        candle.volume += quantity;

        RefreshChart(assetId);
    }

    private void RefreshChart(string assetId)
    {
        if (chartController == null)
            return;

        if (chartController.currentAssetId != assetId)
            return;

        chartController.LoadChart(assetId);
    }
}
