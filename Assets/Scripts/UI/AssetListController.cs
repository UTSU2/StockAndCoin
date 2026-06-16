using UnityEngine;
using Data;

public class AssetListController : MonoBehaviour
{
    public MarketDatabase marketDatabase;
    public MarketType marketType;
    public AssetListItem itemPrefab;
    public Transform itemContainer;
    public ChartController chartController;
    private void Start()
    {
        CreateAssetList();
    }

    public void CreateAssetList()
    {
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (AssetData asset in marketDatabase.assets)
        {
            if (asset.marketType == marketType)
            {
                AssetListItem item = Instantiate(itemPrefab, itemContainer);
                item.Initialize(asset, chartController);
            }
        }
    }
}