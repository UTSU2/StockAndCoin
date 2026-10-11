using UnityEngine;
using Data;
using System;

public class TradeMatchingEngine : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private OrderBookManager orderBookManager;
    [SerializeField] private RealtimeMarketUpdater marketUpdater;

    public event Action<LimitOrder, LimitOrder, float, int> OnTradeExecuted;

    public bool SubmitOrder(LimitOrder incomingOrder)
    {
        if (incomingOrder == null || incomingOrder.price <= 0f || incomingOrder.quantity <= 0)
            return false;

        if (orderBookManager == null || marketUpdater == null)
            return false;

        OrderBookData orderBook = orderBookManager.GetOrderBook(incomingOrder.assetId);

        if (orderBook == null)
            return false;

        var oppositeLevels = incomingOrder.isBuy
            ? orderBook.sellOrders : orderBook.buyOrders;

        foreach (OrderBookLevel level in oppositeLevels)
        {
            if (incomingOrder.quantity <= 0)
                break;

            bool canMatch = incomingOrder.isBuy
                ? level.price <= incomingOrder.price : level.price >= incomingOrder.price;

            if (!canMatch)
                break;

            foreach (LimitOrder restingOrder in level.orders)
            {
                if (incomingOrder.quantity <= 0)
                    break;

                if (restingOrder.quantity <= 0)
                    continue;

                int executedQuantity = Mathf.Min(incomingOrder.quantity, restingOrder.quantity);

                float executedPrice = restingOrder.price;

                incomingOrder.quantity -= executedQuantity;
                restingOrder.quantity -= executedQuantity;

                marketUpdater.UpdateTradePrice(incomingOrder.assetId, executedPrice, executedQuantity);

                OnTradeExecuted?.Invoke(
                    incomingOrder, restingOrder, executedPrice, executedQuantity
                );

                Debug.Log(
                    $"[TradeMatching] {incomingOrder.assetId} " +
                    $"{executedPrice}원 / {executedQuantity}주 체결"
                );
            }
        }

        orderBookManager.RemoveEmptyOrders(incomingOrder.assetId);

        if (incomingOrder.quantity > 0)
            return orderBookManager.AddOrder(incomingOrder);

        return true;
    }
}
