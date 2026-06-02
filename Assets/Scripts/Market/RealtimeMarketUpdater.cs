using UnityEngine;
using Data;

public class RealtimeMarketUpdater : MonoBehaviour
{
    [Header("Database")]
    public MarketDatabase database;

    [Header("Chart")]
    public ChartController chartController;

    [Header("Realtime Setting")]
    public bool isMarketOpen = true;
    public float updateInterval = 1f;
    public float volumePerTickMin = 50f;
    public float volumePerTickMax = 300f;

    private float timer;

    void Update()
    {
        if (!isMarketOpen)
            return;

        if (database == null)
            return;

        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;

            UpdateRealtimePrices();
            RefreshChart();
        }
    }

    private void UpdateRealtimePrices()
    {
        Debug.Log("실시간 가격 변동 실행");

        foreach (AssetData asset in database.assets)
        {
            Debug.Log($"변동 대상: {asset.id}");
        }
        foreach (AssetData asset in database.assets)
        {
            CandleChartData chart = database.candleCharts
                .Find(c => c.assetId == asset.id);

            if (chart == null || chart.candles == null || chart.candles.Count == 0)
                continue;

            CandleData currentCandle = chart.candles[chart.candles.Count - 1];

            float moveRate = Random.Range(-asset.currentMoveRange, asset.currentMoveRange);
            float newClose = currentCandle.close * (1f + moveRate);

            currentCandle.close = newClose;

            if (newClose > currentCandle.high)
                currentCandle.high = newClose;

            if (newClose < currentCandle.low)
                currentCandle.low = newClose;

            currentCandle.volume += Random.Range(volumePerTickMin, volumePerTickMax);
        }
    }

    private void RefreshChart()
    {
        if (chartController == null)
            return;

        chartController.LoadChart(chartController.currentAssetId);
    }

    public void OpenMarket()
    {
        isMarketOpen = true;
    }

    public void CloseMarket()
    {
        isMarketOpen = false;
    }
}
