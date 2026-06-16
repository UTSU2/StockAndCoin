using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;

public class AssetListItem : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text symbolText;
    public Button selectButton;

    private AssetData assetData;
    private ChartController chartController;

    public void Initialize(AssetData asset, ChartController chart)
    {
        assetData = asset;
        chartController = chart;

        nameText.text = asset.name;
        symbolText.text = asset.symbol;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        chartController.SelectAsset(assetData.id);
    }
}