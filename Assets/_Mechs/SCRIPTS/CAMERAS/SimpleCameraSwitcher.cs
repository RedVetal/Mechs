using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCameraSwitcher : MonoBehaviour
{
    [SerializeField] Camera[] cameras;
    [SerializeField] int startIndex = 0;
    [SerializeField] InputActionReference switchAction;

    int i;

    void OnEnable() => switchAction?.action.Enable();
    void OnDisable() => switchAction?.action.Disable();
    void Start() => SetActive(startIndex);

    void Update()
    {
        if (switchAction && switchAction.action.WasPressedThisFrame())
            SetActive((i + 1) % cameras.Length);
    }

    void SetActive(int index)
    {
        i = Mathf.Clamp(index, 0, cameras.Length - 1);

        // выключаем ВСЕ камеры в сцене
        foreach (var cam in FindObjectsOfType<Camera>(true))
        {
            bool on = false;
            cam.gameObject.SetActive(on);
            var al = cam.GetComponent<AudioListener>();
            if (al) al.enabled = on;
        }

        // включаем нужную
        var active = cameras[i];
        if (active)
        {
            active.gameObject.SetActive(true);
            var al = active.GetComponent<AudioListener>();
            if (al) al.enabled = true;
        }
    }
}
