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

            orderBook.buyOrders.Add(
                new OrderBookLevel(
                    buyPrice,
                    buyQuantity
                )
            );

            orderBook.sellOrders.Add(
                new OrderBookLevel(
                    sellPrice,
                    sellQuantity
                )
            );
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
        // 우선 초기 버전에서는 basePrice 사용
        // 이후 MarketDatabase의 마지막 CandleData.close로 변경 예정
        return asset.basePrice;
    }

    private float GetTickSize(float price)
    {
        // 임시 호가 단위
        return 10f;
    }
}
