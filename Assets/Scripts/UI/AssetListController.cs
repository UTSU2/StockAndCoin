using UnityEngine;
using Data;

public enum AssetListType
{
    All,
    Holding,
    Favorite
}
public class AssetListController : MonoBehaviour
{
    public MarketDatabase marketDatabase;
    public MarketType marketType;
    public AssetListItem itemPrefab;
    public Transform itemContainer;
    public ChartController chartController;

    [SerializeField] private PlayerManager playerManager;
    private AssetListType currentListType = AssetListType.All;
    private void Start()
    {
        Debug.Log($"[{name}] AssetListController Start 실행");
        CreateAssetList();
    }

    public void CreateAssetList()
    {
        if (marketDatabase == null)
        {
            Debug.LogError("MarketDatabase가 연결되지 않았습니다.");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("AssetListItem 프리팹이 연결되지 않았습니다.");
            return;
        }

        if (itemContainer == null)
        {
            Debug.LogError("ItemContainer가 연결되지 않았습니다.");
            return;
        }

        Debug.Log($"전체 자산 수: {marketDatabase.assets.Count}");
        Debug.Log($"현재 목록 타입: {marketType}");

        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (AssetData asset in marketDatabase.assets)
        {
            Debug.Log(
                $"자산 확인: {asset.id}, 타입: {asset.marketType}"
            );

            if (asset.marketType != marketType)
                continue;

            bool shouldShow = false;
            switch (currentListType)
            {
                case AssetListType.All:
                    shouldShow = true;
                    break;
                case AssetListType.Holding:
                    shouldShow = IsHolding(asset.id);
                    break;
                case AssetListType.Favorite:
                    shouldShow = playerManager.IsFavorite(asset.id);
                    break;
            }

            if (!shouldShow) continue;

            AssetListItem item = Instantiate(itemPrefab, itemContainer);
            item.Initialize(asset, chartController);
        }
    }

    public void ShowAllAssets()
    {
        currentListType = AssetListType.All;
        CreateAssetList();
    }
    public void ShowHoldingAssets()
    {
        currentListType = AssetListType.Holding;
        CreateAssetList();
    }
    public void ShowFavoriteAssets()
    {
        currentListType = AssetListType.Favorite;
        CreateAssetList();
    }

    private bool IsHolding(string assetId)
    {
        return playerManager.HasAsset(assetId);
    }
}