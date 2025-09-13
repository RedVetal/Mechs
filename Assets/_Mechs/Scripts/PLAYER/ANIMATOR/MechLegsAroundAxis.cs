// Assets/_Mechs/Scripts/MechLegsAroundAxis_BindOnce.cs
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MechLegsAroundAxis : MonoBehaviour
{
    [Header("Что вращаем (твои обёртки бедра)")]
    public Transform leftThigh;    // L_Thigh
    public Transform rightThigh;   // R_Thigh

    [Header("Оси (пустышки внутри соответствующего бедра)")]
    public Transform leftHipAxis;  // L_HipAxis (child of L_Thigh)
    public Transform rightHipAxis; // R_HipAxis (child of R_Thigh)
    // Ось берётся по X (right) пустышки.

    public enum Drive { InputAction, RigidbodySpeed, Manual }
    public Drive drive = Drive.InputAction;

    [Header("Шаг")]
    public float stepFrequency = 2f;      // Гц на полном ходу
    public float thighAmplitudeDeg = 16f; // размах в градусах

#if ENABLE_INPUT_SYSTEM
    [Header("Input System (если InputAction)")]
    public InputActionReference moveAction;
    public float inputDeadzone = 0.15f;
#endif

    [Header("Rigidbody (если RigidbodySpeed)")]
    public Rigidbody sourceRb;
    public float speedForFullStride = 3.5f;

    [Header("Manual (если Manual)")]
    [Range(0, 1)] public float manualDrive01 = 0.6f;

    // ---- Закреплённые при старте данные (в СИСТЕМЕ РОДИТЕЛЯ БЕДРА) ----
    Transform lParent, rParent;
    Vector3 lBasePos, rBasePos;         // localPosition бедра при старте
    Quaternion lBaseRot, rBaseRot;        // localRotation бедра при старте
    Vector3 lPivotP, rPivotP;           // pivot в space родителя
    Vector3 lAxisP, rAxisP;            // ось в space родителя (нормализована)

    float phase;

    void Awake()
    {
        if (drive == Drive.RigidbodySpeed && !sourceRb)
            sourceRb = GetComponentInParent<Rigidbody>();
        Rebind(); // фиксируем всё один раз
    }

    public void Rebind()
    {
        // левая
        if (leftThigh)
        {
            lParent = leftThigh.parent;
            lBasePos = leftThigh.localPosition;
            lBaseRot = leftThigh.localRotation;

            if (leftHipAxis && lParent)
            {
                lPivotP = lParent.InverseTransformPoint(leftHipAxis.position);
                lAxisP = lParent.InverseTransformDirection(leftHipAxis.right).normalized;
            }
        }
        // правая
        if (rightThigh)
        {
            rParent = rightThigh.parent;
            rBasePos = rightThigh.localPosition;
            rBaseRot = rightThigh.localRotation;

            if (rightHipAxis && rParent)
            {
                rPivotP = rParent.InverseTransformPoint(rightHipAxis.position);
                rAxisP = rParent.InverseTransformDirection(rightHipAxis.right).normalized;
            }
        }
    }

    void Update()
    {
        float drive01 = 0f;
        switch (drive)
        {
#if ENABLE_INPUT_SYSTEM
            case Drive.InputAction:
                if (moveAction && moveAction.action != null)
                {
                    float m = moveAction.action.ReadValue<Vector2>().magnitude;
                    drive01 = (m > inputDeadzone) ? Mathf.InverseLerp(inputDeadzone, 1f, m) : 0f;
                }
                break;
#endif
            case Drive.RigidbodySpeed:
                if (sourceRb)
                    drive01 = Mathf.Clamp01(sourceRb.linearVelocity.magnitude / Mathf.Max(0.01f, speedForFullStride));
                break;
            case Drive.Manual:
                drive01 = manualDrive01;
                break;
        }

        float freq = stepFrequency * (drive01 > 0f ? Mathf.Lerp(0.25f, 1f, drive01) : 0f);
        phase += freq * Time.deltaTime * Mathf.PI * 2f;
        float A = thighAmplitudeDeg * drive01;

        ApplyLeg(leftThigh, lParent, lBasePos, lBaseRot, lPivotP, lAxisP, Mathf.Sin(phase) * A);
        ApplyLeg(rightThigh, rParent, rBasePos, rBaseRot, rPivotP, rAxisP, Mathf.Sin(phase + Mathf.PI) * A);
    }

    // Восстанавливаем позу из базы + поворот вокруг закреплённой оси/точки (в пространстве родителя)
    void ApplyLeg(Transform thigh, Transform parent, Vector3 basePos, Quaternion baseRot,
                  Vector3 pivotP, Vector3 axisP, float angleDeg)
    {
        if (!thigh || !parent || axisP.sqrMagnitude < 1e-6f) return;

        Quaternion q = Quaternion.AngleAxis(angleDeg, axisP);

        Vector3 v = basePos - pivotP;      // все в локале родителя
        thigh.localPosition = pivotP + q * v;
        thigh.localRotation = q * baseRot;
    }

#if UNITY_EDITOR
    [ContextMenu("Rebind (capture axes now)")]
    void RebindMenu() { Rebind(); UnityEditor.EditorUtility.SetDirty(this); }
#endif
}
