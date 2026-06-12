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
    public bool BuyAsset(string assetId, float price, int quantity)
    {
        float totalPrice = price * quantity;

        if (quantity <= 0) //일단 현 상태 유지, 후에 quantity가 0이면 버튼 눌리지 않게 수정 예정
        {
            Debug.LogWarning("구매 수량은 1 이상이어야 합니다");
            return false;
        }
        if (playerData.cash < totalPrice) //후에 자산 부족하면 버튼 눌리지 않게 수정 예정
        {
            Debug.LogWarning("현금이 부족합니다");
            return false;
        }

        PlayerHolding holding = playerData.holdings
            .FirstOrDefault(h => h.assetId == assetId);

        playerData.cash -= totalPrice;

        if (holding == null)
        {
            playerData.holdings.Add(new PlayerHolding
            {
                assetId = assetId,
                quantity = quantity,
                averagePrice = price
            });
        }
        else
        {
            float currentTotalPrice = holding.averagePrice * holding.quantity;
            float newTotalPrice = currentTotalPrice + totalPrice;

            holding.quantity += quantity;
            holding.averagePrice = newTotalPrice / holding.quantity;
        }

        return true;
    }

    public bool SellAsset(string assetId, float price, int quantity)
    {
        if (quantity <= 0) //일단 현 상태 유지, 후에 quantity가 0이면 버튼 눌리지 않게 수정 예정
        {
            Debug.LogWarning("판매 수량은 1 이상이어야 합니다");
            return false;
        }

        PlayerHolding holding = playerData.holdings
            .FirstOrDefault(h => h.assetId == assetId);

        if (holding == null || holding.quantity < quantity)
        {
            Debug.LogWarning("보유 수량 부족");
            return false;
        }

        holding.quantity -= quantity;
        playerData.cash += price * quantity;

        if (holding.quantity <= 0)
        {
            playerData.holdings.Remove(holding);
        }

        return true;
    }

    public int GetHoldingQuantity(string assetId)
    {
        PlayerHolding holding =
            playerData.holdings.FirstOrDefault(h => h.assetId == assetId);

        return holding?.quantity ?? 0;
    }
}
