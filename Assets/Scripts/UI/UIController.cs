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
    public Button BuyBtn;
    public Button SellBtn;
    public Button SelectBtn;
    [Header("Manager")]
    public TimeManager timeManager;
    public PlayerManager playerManager;

    private void Start()
    {
        RefreshUI();
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
}
