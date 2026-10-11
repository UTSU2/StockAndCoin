using UnityEngine;
using Data;
using System.Linq;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private PlayerData playerData;
    private float reservedCash = 0f;
    private readonly Dictionary<string, int> reservedHoldings = new();
    public float Cash => playerData.cash;
    public float AvailableCash => playerData.cash - reservedCash;
    public PlayerInfoRank InfoRank => playerData.infoRank;

    private void Awake()
    {
        playerData = new PlayerData(); //저장 시스템 만들고 수정 예정
    }

    public bool CanBuy(float totalPrice)
    {
        return totalPrice > 0f && AvailableCash >= totalPrice;
    }

    public bool CanSell(string assetId, int quantity)
    {
        if (quantity <= 0)
            return false;

        return GetAvailableHoldingQuantity(assetId) >= quantity;
    }
    public void ApplyBuy(
        string assetId,
        int quantity,
        float totalPrice)
    {
        if (quantity <= 0)
            return;

        PlayerHolding holding = playerData.holdings
            .FirstOrDefault(h => h.assetId == assetId);

        playerData.cash -= totalPrice;

        float averagePrice =
            totalPrice / quantity;

        if (holding == null)
        {
            playerData.holdings.Add(
                new PlayerHolding
                {
                    assetId = assetId,
                    quantity = quantity,
                    averagePrice = averagePrice
                }
            );

            return;
        }

        float currentTotalPrice =
            holding.averagePrice * holding.quantity;

        float newTotalPrice =
            currentTotalPrice + totalPrice;

        holding.quantity += quantity;

        holding.averagePrice =
            newTotalPrice / holding.quantity;
    }

    public void ApplySell(
        string assetId,
        int quantity,
        float totalPrice)
    {
        if (quantity <= 0)
            return;

        PlayerHolding holding = playerData.holdings
            .FirstOrDefault(h => h.assetId == assetId);

        if (holding == null)
            return;

        holding.quantity -= quantity;
        playerData.cash += totalPrice;

        if (holding.quantity <= 0)
            playerData.holdings.Remove(holding);
    }

    public int GetHoldingQuantity(string assetId)
    {
        PlayerHolding holding = playerData.holdings
            .FirstOrDefault(h => h.assetId == assetId);

        return holding?.quantity ?? 0;
    }

    public bool HasAsset(string assetId)
    {
        return playerData.holdings.Exists(
            holding =>
                holding.assetId == assetId && holding.quantity > 0
        );
    }

    public bool IsFavorite(string assetId)
    {
        return playerData.favoriteAssetIds.Exists(
            favorite => favorite == assetId
        );
    }
    public int GetAvailableHoldingQuantity(string assetId)
    {
        int holdingQuantity = GetHoldingQuantity(assetId);

        reservedHoldings.TryGetValue(
            assetId, out int reservedQuantity
        );

        return holdingQuantity - reservedQuantity;
    }

    public bool ReserveBuy(float limitPrice, int quantity)
    {
        if (limitPrice <= 0f || quantity <= 0)
            return false;

        float totalPrice = limitPrice * quantity;

        if (!CanBuy(totalPrice))
            return false;

        reservedCash += totalPrice;

        return true;
    }

    public bool ReserveSell(string assetId, int quantity)
    {
        if (!CanSell(assetId, quantity))
            return false;

        if (!reservedHoldings.ContainsKey(assetId))
            reservedHoldings[assetId] = 0;

        reservedHoldings[assetId] += quantity;

        return true;
    }

    public void SettleBuy(string assetId, float limitPrice, int quantity, float executedPrice)
    {
        if (quantity <= 0 || limitPrice <= 0f || executedPrice <= 0f || executedPrice > limitPrice)
            return;

        float reservedAmount = limitPrice * quantity;
        float actualAmount = executedPrice * quantity;

        if (reservedCash < reservedAmount)
            return;

        reservedCash -= reservedAmount;

        ApplyBuy(assetId, quantity, actualAmount);
    }

    public void SettleSell(string assetId, int quantity, float executedPrice)
    {
        if (quantity <= 0)
            return;

        if (!reservedHoldings.TryGetValue(assetId, out int reservedQuantity))
            return;

        if (reservedQuantity < quantity)
            return;

        reservedHoldings[assetId] -= quantity;

        if (reservedHoldings[assetId] == 0)
            reservedHoldings.Remove(assetId);

        float totalPrice = executedPrice * quantity;

        ApplySell(assetId, quantity, totalPrice);
    }

    public void ReleaseBuy(float limitPrice, int quantity)
    {
        if (limitPrice <= 0f || quantity <= 0)
            return;

        float releaseAmount = limitPrice * quantity;

        if (reservedCash < releaseAmount)
            return;

        reservedCash -= releaseAmount;

        reservedCash = Mathf.Max(0f, reservedCash);
    }

    public void ReleaseSell(string assetId, int quantity)
    {
        if (quantity <= 0)
            return;

        if (!reservedHoldings.TryGetValue(assetId, out int reservedQuantity))
            return;

        if (reservedQuantity < quantity)
            return;

        int remainingQuantity = reservedQuantity - quantity;

        if (remainingQuantity == 0)
            reservedHoldings.Remove(assetId);
        else
            reservedHoldings[assetId] = remainingQuantity;
    }
}
