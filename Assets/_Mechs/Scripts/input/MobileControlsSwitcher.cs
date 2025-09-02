using UnityEngine;
using UnityEngine.InputSystem;

// Показывает мобильные кнопки только там, где они нужны.
// В Editor можно принудительно показать (для Device Simulator).
[DefaultExecutionOrder(-1000)]
public class MobileControlsSwitcher : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Parent GameObject that contains all On-Screen controls (sticks + fire button).")]
    [SerializeField] private GameObject mobileControlsRoot;

    [Header("Options")]
    [Tooltip("Show mobile controls while running in the Editor (useful for Device Simulator).")]
    [SerializeField] private bool forceShowInEditor = true;

    [Tooltip("Auto-switch when input devices are added/removed at runtime.")]
    [SerializeField] private bool autoSwitchAtRuntime = true;

    [Tooltip("Optional debug toggle hotkey (Editor/PC).")]
    [SerializeField] private Key toggleKey = Key.F1;

    private void Awake()
    {
        ApplyVisibility(CalcShouldShow());
        if (autoSwitchAtRuntime)
            InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        if (autoSwitchAtRuntime)
            InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Update()
    {
        // Ручной тумблер (удобно в Editor)
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            SetActive(!IsActive());
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
            ApplyVisibility(CalcShouldShow());
    }

    // Решаем, показывать ли кнопки
    private bool CalcShouldShow()
    {
#if UNITY_EDITOR
        if (forceShowInEditor) return true; // для Device Simulator
#endif
        // На реальном Android/iOS всегда показываем
        if (Application.isMobilePlatform) return true;

        // Если подключён экран тача (редко в PC), тоже можно показать
        return Touchscreen.current != null;
    }

    private void ApplyVisibility(bool show) => SetActive(show);

    private void SetActive(bool value)
    {
        if (mobileControlsRoot && mobileControlsRoot.activeSelf != value)
            mobileControlsRoot.SetActive(value);
    }

    private bool IsActive() => mobileControlsRoot && mobileControlsRoot.activeSelf;
}
