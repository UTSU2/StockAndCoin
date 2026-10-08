using UnityEngine;
using Data;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    private PlayerData playerData;
    public float Cash => playerData.cash;
    public PlayerInfoRank InfoRank => playerData.infoRank;

    private void Awake()
    {
        playerData = new PlayerData(); //저장 시스템 만들고 수정 예정
    }

    public bool CanBuy(float totalPrice)
    {
        return totalPrice > 0f && playerData.cash >= totalPrice;
    }

    public bool CanSell(string assetId, int quantity)
    {
        if (quantity <= 0)
            return false;

        return GetHoldingQuantity(assetId) >= quantity;
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
}
