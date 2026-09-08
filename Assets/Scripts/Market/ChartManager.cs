using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;

public class ChartManager : MonoBehaviour
{
    [Header("Chart data")]
    public TMP_InputField quantityText;
    public TMP_InputField priceText;
    [Header("Chart UI")]
    [SerializeField] private GameObject listPanel;
    [SerializeField] private GameObject chartPanel;
    [SerializeField] private Button sellBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Button closeBtn;

    public GameObject ListPanel => listPanel;
    public GameObject ChartPanel => chartPanel;

    public Button SellButton => sellBtn;
    public Button BuyButton => buyBtn;
    public Button SelectButton => selectBtn;
    public Button CloseButton => closeBtn;
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
