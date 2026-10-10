
using System.Collections.Generic;
using UnityEngine;
using Data;

public class OrderBookManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private MarketDatabase database;

    private readonly Dictionary<string, OrderBookData> orderBooks = new();

    private void Awake()
    {
        InitializeOrderBooks();
    }

    private void InitializeOrderBooks()
    {
        orderBooks.Clear();

        if (database == null)
        {
            Debug.LogError(
                "[OrderBookManager] MarketDatabase가 연결되지 않았습니다."
            );
            return;
        }

        foreach (AssetData asset in database.assets)
        {
            OrderBookData orderBook =
                new OrderBookData(asset.id);

            orderBooks.Add(asset.id, orderBook);
        }
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

    public bool AddOrder(LimitOrder order)
    {
        if (order == null ||
            order.price <= 0f ||
            order.quantity <= 0)
            return false;

        OrderBookData orderBook =
            GetOrderBook(order.assetId);

        if (orderBook == null)
            return false;

        List<OrderBookLevel> levels =
            order.isBuy
                ? orderBook.buyOrders
                : orderBook.sellOrders;

        OrderBookLevel level =
            levels.Find(l =>
                Mathf.Approximately(l.price, order.price));

        if (level == null)
        {
            level = new OrderBookLevel(order.price);
            levels.Add(level);
        }

        level.orders.Add(order);

        SortOrderBook(levels, order.isBuy);

        return true;
    }

    private void SortOrderBook(
        List<OrderBookLevel> levels,
        bool isBuy)
    {
        if (isBuy)
        {
            levels.Sort(
                (a, b) => b.price.CompareTo(a.price)
            );
        }
        else
        {
            levels.Sort(
                (a, b) => a.price.CompareTo(b.price)
            );
        }
    }

    public bool RemoveOrder(string assetId, string orderId)
    {
        OrderBookData orderBook =
            GetOrderBook(assetId);

        if (orderBook == null)
            return false;

        bool removed = false;

        removed |= RemoveOrderFromLevels(
            orderBook.buyOrders,
            orderId
        );

        removed |= RemoveOrderFromLevels(
            orderBook.sellOrders,
            orderId
        );

        return removed;
    }

    private bool RemoveOrderFromLevels(
        List<OrderBookLevel> levels,
        string orderId)
    {
        bool removed = false;

        foreach (OrderBookLevel level in levels)
        {
            int count = level.orders.RemoveAll(
                order => order.orderId == orderId
            );

            if (count > 0)
                removed = true;
        }

        levels.RemoveAll(
            level => level.orders.Count == 0
        );

        return removed;
    }

    public void RemoveEmptyOrders(string assetId)
    {
        OrderBookData orderBook =
            GetOrderBook(assetId);

        if (orderBook == null)
            return;

        RemoveEmptyLevels(orderBook.buyOrders);
        RemoveEmptyLevels(orderBook.sellOrders);
    }

    private void RemoveEmptyLevels(
        List<OrderBookLevel> levels)
    {
        foreach (OrderBookLevel level in levels)
        {
            level.orders.RemoveAll(
                order => order.quantity <= 0
            );
        }

        levels.RemoveAll(
            level => level.orders.Count == 0
        );
    }
}
