using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPulseEffect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseAmount = 0.08f;

    private Vector3 baseScale;
    private bool isSelected;

    private void Awake()
    {
        baseScale = transform.localScale;
        ResetVisuals();
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
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                baseScale,
                Time.unscaledDeltaTime * 10f
            );
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResetVisuals();
    }

    private void OnDisable()
    {
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        isSelected = false;
        transform.localScale = baseScale;
    }
}