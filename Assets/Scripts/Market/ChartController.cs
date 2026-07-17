using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using Data;

public class ChartController : MonoBehaviour
{
    [Header("Database")]
    public MarketDatabase marketDatabase;
    public string currentAssetId = "BTC"; //  chart 가 열릴 때 마다 해당 id로 조회

    [Header("Candle UI")]
    public GameObject bullishCandlePrefab; //high
    public GameObject bearishCandlePrefab; //low
    public Transform candleContainer;

    public float candleWidth = 12f;
    public float candleGap = 4f;
    public float chartHeight = 300f;

    void Start()
    {
        //SelectAsset(currentAssetId);
    }
    public void SelectAsset(string assetId)
    {
        if (string.IsNullOrEmpty(assetId))
        {
            Debug.LogWarning("선택된 ID가 없습니다");
            return;
        }
        currentAssetId = assetId;
        LoadChart(assetId);
    }
    public void LoadChart(string assetId)
    {
        if (marketDatabase == null)
        {
            Debug.LogWarning("MarketDataBase가 연동되지 않았습니다");
            return;
        }

        List<CandleData> candles = marketDatabase.GetCandlesByAsset(assetId);
        if (candles == null || candles.Count == 0)
        {
            Debug.LogWarning($"{assetId}에 해당하는 캔들 데이터가 존재하지 않습니다");
            return;
        }

        DrawChart(candles);
    }
    public void RefreshChart()
    {
        if (string.IsNullOrEmpty(currentAssetId))
            return;

        LoadChart(currentAssetId);
    }

    public void DrawChart(List<CandleData> data)
    {
        ClearChart();

        foreach (Transform child in candleContainer)
            Destroy(child.gameObject);

        float max = data.Max(c => c.high);
        float min = data.Min(c => c.low);

        for (int i = 0; i < data.Count; i++)
        {
            var candle = data[i];

            bool isBull = candle.close >= candle.open;

            GameObject prefab = isBull
                ? bullishCandlePrefab
                : bearishCandlePrefab;

            GameObject obj = Instantiate(prefab, candleContainer);
            RectTransform rect = obj.GetComponent<RectTransform>();

            float x = i * (candleWidth + candleGap);
            rect.anchoredPosition = new Vector2(x, 0);

            float openY = Normalize(candle.open, min, max);
            float closeY = Normalize(candle.close, min, max);
            float highY = Normalize(candle.high, min, max);
            float lowY = Normalize(candle.low, min, max);

            float bodyTop = Mathf.Max(openY, closeY);
            float bodyBot = Mathf.Min(openY, closeY);

            //body
            var body = obj.transform.Find("Body").GetComponent<RectTransform>();
            body.anchoredPosition = new Vector2(0, bodyBot);
            body.sizeDelta = new Vector2(candleWidth, bodyTop - bodyBot);

            //wick
            var wick = obj.transform.Find("Wick").GetComponent<RectTransform>();
            wick.anchoredPosition = new Vector2(0, lowY);
            wick.sizeDelta = new Vector2(2f, highY - lowY);
        }
    }
    private void ClearChart()
    {
        foreach (Transform child in candleContainer)
            Destroy(child.gameObject);
    }

    float Normalize(float price, float min, float max)
    {
        if (Mathf.Approximately(max, min))
            return chartHeight * 0.5f;
        return (price - min) / (max - min) * chartHeight;
    }
}
