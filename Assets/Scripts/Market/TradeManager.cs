using UnityEngine;
using Data;
using System.Linq;

public class TradeManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private OrderBookManager orderBookManager;
    [SerializeField] private PlayerManager playerManager;

    public bool LimitBuy(
        string assetId,
        float limitPrice,
        int quantity)
    {
        if (quantity <= 0)
        {
            Debug.LogWarning("구매 수량은 1 이상이어야 합니다.");
            return false;
        }

        if (limitPrice <= 0f)
        {
            Debug.LogWarning("구매 가격은 0보다 커야 합니다.");
            return false;
        }

        OrderBookData orderBook =
            orderBookManager.GetOrderBook(assetId);

        if (orderBook == null)
        {
            Debug.LogWarning("호가 정보를 찾을 수 없습니다.");
            return false;
        }

        float maximumPrice =
            limitPrice * quantity;

        if (!playerManager.CanBuy(maximumPrice))
        {
            Debug.LogWarning("현금이 부족합니다.");
            return false;
        }

        int remainingQuantity = quantity;
        int executedQuantity = 0;
        float totalPrice = 0f;

        foreach (OrderBookLevel level in orderBook.sellOrders)
        {
            if (remainingQuantity <= 0)
                break;

            if (level.price > limitPrice)
                break;

            foreach (LimitOrder order in level.orders)
            {
                if (remainingQuantity <= 0)
                    break;

                if (order.quantity <= 0)
                    continue;

                int tradeQuantity =
                    Mathf.Min(
                        order.quantity,
                        remainingQuantity
                    );

                order.quantity -= tradeQuantity;
                remainingQuantity -= tradeQuantity;

                executedQuantity += tradeQuantity;

                totalPrice +=
                    level.price * tradeQuantity;
            }
        }

        if (executedQuantity > 0)
        {
            playerManager.ApplyBuy(
                assetId,
                executedQuantity,
                totalPrice
            );
        }

        RemoveEmptyOrders(orderBook);

        if (remainingQuantity > 0)
        {
            AddPlayerOrder(
                orderBook,
                assetId,
                limitPrice,
                remainingQuantity,
                true
            );
        }

        Debug.Log(
            $"{assetId} 매수 주문 / " +
            $"주문 {quantity}주 / " +
            $"체결 {executedQuantity}주 / " +
            $"대기 {remainingQuantity}주"
        );

        return true;
    }

    public bool LimitSell(
        string assetId,
        float limitPrice,
        int quantity)
    {
        if (quantity <= 0)
        {
            Debug.LogWarning("판매 수량은 1 이상이어야 합니다.");
            return false;
        }

        if (limitPrice <= 0f)
        {
            Debug.LogWarning("판매 가격은 0보다 커야 합니다.");
            return false;
        }

        if (!playerManager.CanSell(assetId, quantity))
        {
            Debug.LogWarning("보유 수량이 부족합니다.");
            return false;
        }

        OrderBookData orderBook =
            orderBookManager.GetOrderBook(assetId);

        if (orderBook == null)
        {
            Debug.LogWarning("호가 정보를 찾을 수 없습니다.");
            return false;
        }

        int remainingQuantity = quantity;
        int executedQuantity = 0;
        float totalPrice = 0f;

        foreach (OrderBookLevel level in orderBook.buyOrders)
        {
            if (remainingQuantity <= 0)
                break;

            if (level.price < limitPrice)
                break;

            foreach (LimitOrder order in level.orders)
            {
                if (remainingQuantity <= 0)
                    break;

                if (order.quantity <= 0)
                    continue;

                int tradeQuantity =
                    Mathf.Min(
                        order.quantity,
                        remainingQuantity
                    );

                order.quantity -= tradeQuantity;
                remainingQuantity -= tradeQuantity;

                executedQuantity += tradeQuantity;

                totalPrice +=
                    level.price * tradeQuantity;
            }
        }

        if (executedQuantity > 0)
        {
            playerManager.ApplySell(
                assetId,
                executedQuantity,
                totalPrice
            );
        }

        RemoveEmptyOrders(orderBook);

        if (remainingQuantity > 0)
        {
            AddPlayerOrder(
                orderBook,
                assetId,
                limitPrice,
                remainingQuantity,
                false
            );
        }

        Debug.Log(
            $"{assetId} 매도 주문 / " +
            $"주문 {quantity}주 / " +
            $"체결 {executedQuantity}주 / " +
            $"대기 {remainingQuantity}주"
        );

        return true;
    }

    private void AddPlayerOrder(
        OrderBookData orderBook,
        string assetId,
        float price,
        int quantity,
        bool isBuy)
    {
        var levels =
            isBuy
                ? orderBook.buyOrders
                : orderBook.sellOrders;

        OrderBookLevel level =
            levels.FirstOrDefault(
                l => Mathf.Approximately(l.price, price)
            );

        if (level == null)
        {
            level = new OrderBookLevel(price);
            levels.Add(level);

            if (isBuy)
            {
                levels.Sort(
                    (a, b) =>
                        b.price.CompareTo(a.price)
                );
            }
            else
            {
                levels.Sort(
                    (a, b) =>
                        a.price.CompareTo(b.price)
                );
            }
        }

        level.orders.Add(
            new LimitOrder(
                assetId,
                price,
                quantity,
                isBuy,
                true
            )
        );
    }

    private void RemoveEmptyOrders(
        OrderBookData orderBook)
    {
        foreach (OrderBookLevel level in orderBook.buyOrders)
        {
            level.orders.RemoveAll(
                order => order.quantity <= 0
            );
        }

        foreach (OrderBookLevel level in orderBook.sellOrders)
        {
            level.orders.RemoveAll(
                order => order.quantity <= 0
            );
        }

        orderBook.buyOrders.RemoveAll(
            level => level.orders.Count == 0
        );

        orderBook.sellOrders.RemoveAll(
            level => level.orders.Count == 0
        );
    }
}