using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;

public class ChartManager : MonoBehaviour
{
    [Header("Chart data")]
    public TMP_Text quantityText;
    public TMP_Text priceText;
    [Header("Chart UI")]
    [SerializeField] private GameObject listPanel;
    [SerializeField] private GameObject chartPanel;
    [SerializeField] private Button sellBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Button closeBtn;

    public GameObject GetlistPanel()
    {
        return listPanel;
    }
    public GameObject GetchartPanel()
    {
        return chartPanel;
    }
    public Button GetsellButton()
    {
        return sellBtn;
    }
    public Button GetbuyButton()
    {
        return buyBtn;
    }
    public Button GetselectButton()
    {
        return selectBtn;
    }
    public Button GetcloseButton()
    {
        return closeBtn;
    }
    public int GetQuantity()
    {
        if (int.TryParse(quantityText.text, out int quantity))
            return quantity;

        Debug.LogWarning("수량 변환 실패");
        return 0;
    }
    public float GetPrice()
    {
        if (float.TryParse(priceText.text, out float price))
            return price;

        Debug.LogWarning("가격 변환 실패");
        return 0;
    }
}
