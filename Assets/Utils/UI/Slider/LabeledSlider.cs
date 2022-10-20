using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LabeledSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueTMP;
    [SerializeField, Range(1, 3)] private int numDecimalDigits = 1;

    private Slider slider;

    private void Awake()
    {
        var rect = valueTMP.GetComponent<RectTransform>();
        rect.sizeDelta = (30 + 10 * numDecimalDigits) * Vector2.right + rect.sizeDelta.y * Vector2.up;
        rect.anchoredPosition = (40 + 10 * numDecimalDigits) * Vector2.right + rect.anchoredPosition.y * Vector2.up;

        if (valueTMP)
        {
            var slider = GetComponent<Slider>();
            SetValue(slider.value);
        }
    }

    public void SetValue(float value)
    {
        string format = "0.";
        for (int i = 0; i < numDecimalDigits; i++)
        {
            format += "0";
        }
        if (valueTMP) valueTMP.text = value.ToString(format);
    }
}
