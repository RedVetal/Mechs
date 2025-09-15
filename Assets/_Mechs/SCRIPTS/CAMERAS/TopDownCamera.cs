using UnityEngine;

namespace Mechs.CameraRig
{
    // ќбновл€емс€ ѕќ«∆≈ всего остального
    [DefaultExecutionOrder(10000)]
    public class TopDownCamera : MonoBehaviour
    {
        [SerializeField] Transform target;   // MechRoot
        [Header("Framing")]
        [SerializeField] float height = 12f; // высота над землей
        [SerializeField] float back = 10f;   // насколько южнее цели (смотрим на север = +Z)
        [SerializeField] float pitchDeg = 45f; // наклон вниз
        [Tooltip("—двинуть кадр в мировых координатах (X = восток, Y = север)")]
        [SerializeField] Vector2 planarOffset = Vector2.zero;

        [Header("Smoothing")]
        [SerializeField] float followSmooth = 0.12f; // 0.08Ц0.2 норм
        Vector3 _vel;

        void OnEnable()
        {
            _vel = Vector3.zero;
            Snap();
        }

        void LateUpdate()
        {
            if (!target) return;

            var desired = DesiredPosition();
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _vel, followSmooth);
            transform.rotation = Quaternion.Euler(pitchDeg, 0f, 0f); // север всегда вверх
        }

        Vector3 DesiredPosition()
        {
            var pos = target.position;
            pos += Vector3.up * height;      // вверх
            pos += -Vector3.forward * back;  // южнее цели (смотрим на север)
            pos += new Vector3(planarOffset.x, 0f, planarOffset.y); // тонка€ подстройка кадра
            return pos;
        }

        public void Snap()
        {
            if (!target) return;
            transform.position = DesiredPosition();
            transform.rotation = Quaternion.Euler(pitchDeg, 0f, 0f);
        }

#if UNITY_EDITOR
        // удобна€ кнопка в инспекторе
        [ContextMenu("Snap Now")]
        void EditorSnap() => Snap();
#endif
    }
}
