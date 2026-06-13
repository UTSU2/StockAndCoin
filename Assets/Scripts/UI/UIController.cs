using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text dateText;
    public TMP_Text cashText;
    public TMP_Text rankText;
    [Header("Button")]
    public Button stockPanelOpenBtn;
    public Button coinPanelOpenBtn;
    public Button buyBtn;
    public Button sellBtn;
    public Button selectBtn;
    [Header("Manager")]
    public TimeManager timeManager;
    public PlayerManager playerManager;
    public ChartController chartController;
    public ChartManager chartManager;

    private void Start()
    {
        RefreshUI();

        buyBtn.onClick.AddListener(OnBuyButtonClicked);
        sellBtn.onClick.AddListener(OnSellButtonClicked);
        selectBtn.onClick.AddListener(OnSelectButtonClicked);
    }
    public void RefreshUI()
    {
        UpdateDate();
        UpdateCash();
        UpdateRank();
    }

    public void UpdateDate()
    {
        dateText.text = $"{timeManager.year}.{timeManager.month:D2}.{timeManager.day:D2}";
    }
    public void UpdateCash()
    {
        cashText.text = $"{playerManager.Cash:N0}원";
    }
    public void UpdateRank()
    {
        rankText.text = playerManager.InfoRank.ToString();
    }
    private void OnBuyButtonClicked()
    {
        Debug.Log("매수 버튼 클릭");
        playerManager.BuyAsset(chartController.currentAssetId, chartManager.GetPrice(), chartManager.GetQuantity());

    }

    private void OnSellButtonClicked()
    {
        Debug.Log("매도 버튼 클릭");
        playerManager.SellAsset(chartController.currentAssetId, chartManager.GetPrice(), chartManager.GetQuantity());
    }

    private void OnSelectButtonClicked()
    {
        Debug.Log("선택 버튼 클릭");
    }
}
