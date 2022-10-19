using UnityEngine;


public class AimCursorView : MonoBehaviour
{
    // ====================================================
    private RectTransform _rectTransform;
    private Canvas _parentCanvas;
    private AimCursor _aimCursor;


    // ====================================================
    protected void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
        _aimCursor = FindObjectOfType<AimCursor>();

        _aimCursor.OnPositionChanged += CursorPositionChanged;
    }

    protected void OnDestroy()
    {
        if (_aimCursor)
            _aimCursor.OnPositionChanged -= CursorPositionChanged;
    }

    private void CursorPositionChanged(Vector3 pos)
    {
        _rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(pos) / _parentCanvas.scaleFactor;
    }
}