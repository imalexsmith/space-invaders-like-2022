using UnityEngine;
using UnityEngine.InputSystem;
using UltEvents;


public class AimCursor : MonoBehaviour
{
    // ====================================================
    public UltEvent<Vector3> OnPositionChanged = new();

    [Space]
    public float PointerMoveThreshold = 1.25f;

    [Space]
    public InputActionReference AimAction;

    private Vector2 _lastPos;


    // ====================================================
    protected void Awake()
    {
        AimAction.action.performed += AimInput;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    protected void Start()
    {
        _lastPos = Pointer.current.position.ReadValue();

        LevelManager.Instance.OnVictory += UnlockCursor;
        LevelManager.Instance.OnDefeat += UnlockCursor;
    }

    protected void OnDestroy()
    {
        AimAction.action.performed -= AimInput;

        if (LevelManager.IsReady)
        {
            LevelManager.Instance.OnVictory -= UnlockCursor;
            LevelManager.Instance.OnDefeat -= UnlockCursor;
        }
    }

    private void AimInput(InputAction.CallbackContext context)
    {
        var sPos = context.ReadValue<Vector2>();
        var d = (sPos - _lastPos).magnitude;

        if (d > PointerMoveThreshold)
        {
            var cam = Camera.main;
            var pos = cam.ScreenToWorldPoint(new Vector3(sPos.x, sPos.y, -cam.transform.position.z));
            transform.position = pos;
            OnPositionChanged.Invoke(pos);
        }

        _lastPos = sPos;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}