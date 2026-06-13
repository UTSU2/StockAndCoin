using UnityEngine;
using TMPro;

public class ChartManager : MonoBehaviour
{
    [Header("Chart data")]
    public TMP_Text quantityText;
    public TMP_Text priceText;

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
