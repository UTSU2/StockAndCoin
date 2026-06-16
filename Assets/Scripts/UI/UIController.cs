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
    [Header("Panel")]
    public GameObject stockPanel;
    public GameObject coinPanel;
    [Header("Manager")]
    public TimeManager timeManager;
    public PlayerManager playerManager;
    public ChartController chartController; //current
    public ChartController stockChartController;
    public ChartController coinChartController;
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
    public void CheckCurrentController()
    {
        chartController = coinPanel.activeSelf
            ? coinChartController
            : stockChartController;
    }
    private void OnBuyButtonClicked()
    {
        Debug.Log("매수 버튼 클릭");
        CheckCurrentController();
        bool success = playerManager.BuyAsset(chartController.currentAssetId, chartManager.GetPrice(), chartManager.GetQuantity());
        if (success)
            RefreshUI();
    }

    private void OnSellButtonClicked()
    {
        Debug.Log("매도 버튼 클릭");
        CheckCurrentController();
        bool success = playerManager.SellAsset(chartController.currentAssetId, chartManager.GetPrice(), chartManager.GetQuantity());
        if (success)
            RefreshUI();
    }

    private void OnSelectButtonClicked()
    {
        Debug.Log("선택 버튼 클릭");
    }

    public void OpenStockPanel()
    {
        stockPanel.SetActive(true);
        coinPanel.SetActive(false);

        chartController = stockChartController;
    }

    public void OpenCoinPanel()
    {
        stockPanel.SetActive(false);
        coinPanel.SetActive(true);

        chartController = coinChartController;
    }

    public void ClosePanel()
    {
        stockPanel.SetActive(false);
        coinPanel.SetActive(false);

        chartController = null;
    }
}
