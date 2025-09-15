// Assets/_Mechs/SCRIPTS/CAMERAS/MechFollowCamera.cs
using UnityEngine;

namespace Mechs.CameraRig
{
    [DefaultExecutionOrder(10000)]
    public class MechFollowCamera : MonoBehaviour
    {
        public enum Mode { TopDown, ThirdPerson, FirstPerson }

        [Header("Target")]
        [SerializeField] Transform followTarget;     // обычно T_Torso
        [SerializeField] Transform mountOverride;    // сокет для вида из кабины (CabinEye). Опционально.

        [Header("Mode")]
        [SerializeField] Mode mode = Mode.TopDown;

        [Header("Smoothing")]
        [SerializeField] float posSmooth = 0.12f;    // 0 = без сглаживания
        [SerializeField] float rotSmooth = 0.12f;

        [Header("Extra world offset")]
        [SerializeField] Vector3 worldOffset = Vector3.zero;

        // Top-down params (вращаемся вместе с торсом)
        [Header("TopDown")]
        [SerializeField] float tdHeight = 12f;
        [SerializeField] float tdBack = 10f;
        [SerializeField] float tdSide = 0f;
        [SerializeField] float tdPitch = 45f;
        [SerializeField] float tdYawOffset = 0f;

        // TPS params (за спиной торса)
        [Header("ThirdPerson")]
        [SerializeField] float tpsDistance = 8f;
        [SerializeField] float tpsSide = 0.5f;    // >0 вправо от торса
        [SerializeField] float tpsUp = 2f;
        [SerializeField] float tpsPitch = 10f;
        [SerializeField] float tpsYawOffset = 0f;

        // FP params (из кабины)
        [Header("FirstPerson")]
        [SerializeField] Vector3 fpLocalOffset = Vector3.zero; // если нет mountOverride
        [SerializeField] float fpYawOffset = 0f;
        [SerializeField] float fpPitchOffset = 0f;

        Vector3 _vel;

        void OnEnable() => Snap();

        void LateUpdate()
        {
            if (!followTarget) return;

            Compute(out var desiredPos, out var desiredRot);

            // сглаживание позиции
            if (posSmooth > 0f)
                transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _vel, posSmooth);
            else
                transform.position = desiredPos;

            // сглаживание поворота
            if (rotSmooth > 0f)
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-rotSmooth * 60f * Time.deltaTime));
            else
                transform.rotation = desiredRot;
        }

        void Compute(out Vector3 pos, out Quaternion rot)
        {
            switch (mode)
            {
                case Mode.TopDown:
                    {
                        float yaw = followTarget.eulerAngles.y + tdYawOffset;
                        var yawRot = Quaternion.Euler(0f, yaw, 0f);
                        pos = followTarget.position
                             + Vector3.up * tdHeight
                             + yawRot * (Vector3.back * tdBack + Vector3.right * tdSide)
                             + worldOffset;
                        rot = Quaternion.Euler(tdPitch, yaw, 0f);
                        break;
                    }
                case Mode.ThirdPerson:
                    {
                        float yaw = followTarget.eulerAngles.y + tpsYawOffset;
                        var yawRot = Quaternion.Euler(0f, yaw, 0f);
                        pos = followTarget.position
                             + yawRot * (Vector3.back * tpsDistance + Vector3.right * tpsSide)
                             + Vector3.up * tpsUp
                             + worldOffset;
                        rot = Quaternion.Euler(tpsPitch, yaw, 0f);
                        break;
                    }
                case Mode.FirstPerson:
                default:
                    {
                        if (mountOverride)
                        {
                            pos = mountOverride.position + worldOffset;
                            rot = mountOverride.rotation * Quaternion.Euler(fpPitchOffset, fpYawOffset, 0f);
                        }
                        else
                        {
                            float yaw = followTarget.eulerAngles.y + fpYawOffset;
                            pos = followTarget.TransformPoint(fpLocalOffset) + worldOffset;
                            rot = Quaternion.Euler(fpPitchOffset, yaw, 0f);
                        }
                        break;
                    }
            }
        }

        [ContextMenu("Snap")]
        public void Snap()
        {
            if (!followTarget) return;
            Compute(out var p, out var r);
            transform.SetPositionAndRotation(p, r);
            _vel = Vector3.zero;
        }

        // на всякий, удобно видеть, куда смотрим
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
            Gizmos.DrawWireSphere(transform.position, 0.15f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1.5f);
        }
    }
}
