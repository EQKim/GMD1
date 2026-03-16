using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPulseEffect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseAmount = 0.08f;

    [Header("Colors")]
    [SerializeField] private bool changeColor = true;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private Vector3 baseScale;
    private bool isSelected;
    private TMPro.TMP_Text tmpText;

    private void Awake()
    {
        baseScale = transform.localScale;
        tmpText = GetComponent<TMPro.TMP_Text>();

        if (tmpText != null && changeColor)
            tmpText.color = normalColor;
    }

    private void Update()
    {
        if (isSelected)
        {
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
            transform.localScale = baseScale * pulse;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, Time.unscaledDeltaTime * 10f);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;

        if (tmpText != null && changeColor)
            tmpText.color = selectedColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        transform.localScale = baseScale;

        if (tmpText != null && changeColor)
            tmpText.color = normalColor;
    }
}