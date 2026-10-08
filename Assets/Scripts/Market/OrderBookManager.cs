using System.Collections.Generic;
using UnityEngine;
using Data;

public class OrderBookManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private MarketDatabase database;

    [Header("Order Book Settings")]
    [SerializeField] private int orderBookLevelCount = 10;
    [SerializeField] private int minQuantity = 10;
    [SerializeField] private int maxQuantity = 100;

    private readonly Dictionary<string, OrderBookData> orderBooks = new();

    private void Start()
    {
        InitializeOrderBooks();
    }

    private void InitializeOrderBooks()
    {
        orderBooks.Clear();

        foreach (AssetData asset in database.assets)
        {
            float currentPrice = GetCurrentPrice(asset);

            OrderBookData orderBook =
                CreateOrderBook(asset.id, currentPrice);

            orderBooks.Add(asset.id, orderBook);

            Debug.Log(
                $"{asset.id} 호가 생성 완료 " +
                $"/ 매수 {orderBook.buyOrders.Count}개 " +
                $"/ 매도 {orderBook.sellOrders.Count}개"
            );
        }
    }

    private OrderBookData CreateOrderBook(
        string assetId,
        float currentPrice)
    {
        OrderBookData orderBook = new OrderBookData(assetId);

        float tickSize = GetTickSize(currentPrice);

        for (int i = 1; i <= orderBookLevelCount; i++)
        {
            float buyPrice =
                currentPrice - tickSize * i;

            float sellPrice =
                currentPrice + tickSize * i;

            int buyQuantity =
                Random.Range(minQuantity, maxQuantity + 1);

            int sellQuantity =
                Random.Range(minQuantity, maxQuantity + 1);

            OrderBookLevel buyLevel =
                new OrderBookLevel(buyPrice);

            buyLevel.orders.Add(
                new LimitOrder(
                    assetId,
                    buyPrice,
                    buyQuantity,
                    true,
                    false
                )
            );

            orderBook.buyOrders.Add(buyLevel);

            OrderBookLevel sellLevel =
                new OrderBookLevel(sellPrice);

            sellLevel.orders.Add(
                new LimitOrder(
                    assetId,
                    sellPrice,
                    sellQuantity,
                    false,
                    false
                )
            );

            orderBook.sellOrders.Add(sellLevel);
        }

        return orderBook;
    }

    public OrderBookData GetOrderBook(string assetId)
    {
        if (orderBooks.TryGetValue(
            assetId,
            out OrderBookData orderBook))
        {
            return orderBook;
        }

        return null;
    }

    private float GetCurrentPrice(AssetData asset)
    {
        List<CandleData> candles =
            database.GetCandlesByAsset(asset.id);

        if (candles == null || candles.Count == 0)
            return asset.basePrice;

        CandleData latestCandle =
            candles[candles.Count - 1];

        return latestCandle.close;
    }

    private float GetTickSize(float price)
    {
        // 임시 호가 단위
        return 10f;
    }

}
