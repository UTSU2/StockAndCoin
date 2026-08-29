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
    public Button stockChartOpenBtn;
    public Button coinChartOpenBtn;
    public Button buyBtn;
    public Button sellBtn;
    public Button selectBtn;
    public Button closeBtn;
    [Header("Panel")]
    public GameObject chartPanel;
    public GameObject stockPanel;
    public GameObject stockListPanel;
    public GameObject stockChartPanel;
    public GameObject coinPanel;
    public GameObject coinListPanel;
    public GameObject coinChartPanel;
    [Header("Manager")]
    public TimeManager timeManager;
    public PlayerManager playerManager;
    public ChartController chartController; //current
    public ChartController stockChartController;
    public ChartController coinChartController;
    public ChartManager chartManager;   //current
    public ChartManager stockChartManager;
    public ChartManager coinChartManager;

    private void Start()
    {
        RefreshUI();

        OpenStockPanel(); //임시
        //buyBtn.onClick.AddListener(OnBuyButtonClicked);
        //sellBtn.onClick.AddListener(OnSellButtonClicked);
        //selectBtn.onClick.AddListener(OnSelectButtonClicked);
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
        cashText.text = $"{playerManager.Cash:N0} Soul";
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
        chartManager = coinPanel.activeSelf
            ? coinChartManager
            : stockChartManager;
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
        chartPanel.SetActive(true);
        chartController.LoadChart(chartController.currentAssetId);
    }
    private void OnCloseButtonClicked()
    {
        Debug.Log("닫기 버튼 클릭");
        if (chartPanel.activeSelf)
        {
            chartPanel.SetActive(false);
        }
    }

    public void OpenStockPanel()
    {
        stockPanel.SetActive(true);
        coinPanel.SetActive(false);

        chartController = stockChartController;
        chartManager = stockChartManager;
        chartPanel = stockChartPanel;
        buyBtn = stockChartManager.GetbuyButton();
        sellBtn = stockChartManager.GetsellButton();
        selectBtn = stockChartManager.GetselectButton();
        closeBtn = stockChartManager.GetcloseButton();

        selectBtn.onClick.AddListener(OnSelectButtonClicked);
        closeBtn.onClick.AddListener(OnCloseButtonClicked);
    }

    public void OpenCoinPanel()
    {
        stockPanel.SetActive(false);
        coinPanel.SetActive(true);

        chartController = coinChartController;
        chartManager = coinChartManager;
        chartPanel = coinChartPanel;
        buyBtn = coinChartManager.GetbuyButton();
        sellBtn = coinChartManager.GetsellButton();
        selectBtn = coinChartManager.GetselectButton();
        closeBtn = coinChartManager.GetcloseButton();

        selectBtn.onClick.AddListener(OnSelectButtonClicked);
        closeBtn.onClick.AddListener(OnCloseButtonClicked);
    }

    public void ClosePanel()
    {
        stockPanel.SetActive(false);
        coinPanel.SetActive(false);

        chartController = null;
        chartManager = null;
    }
}
