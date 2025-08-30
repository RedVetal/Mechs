////В Project выбери свой файл *.inputactions (например, MechInput.inputactions).
////Смотри Inspector (импортёр Input Actions). Поставь галку Generate C# Class.
////В поле C# Class Name задай понятное имя, напр.: MechInput. (Лучше не «Player», чтобы не путать с именем action map.)
////Нажми Apply (кнопка вверху инспектора). Подожди компиляцию.

//using UnityEngine;
//using UnityEngine.InputSystem;  // пространство имен нового Input System

//public class MechController : MonoBehaviour
//{
//    public Transform turretTransform;    // ссылка на объект башни танка (Transform)
//    public GameObject shellPrefab;       // префаб снаряда для выстрела
//    public Transform firePoint;          // точка, откуда вылетает снаряд (на стволе орудия)

//    // Скорости движения и поворота
//    public float moveSpeed = 5f;
//    public float turnSpeed = 90f;        // градусов в секунду для поворота корпуса
//    public float turretTurnSpeed = 120f; // градусов в секунду для поворота башни

//    // Экземпляр класса ввода (сгенерированного или Asset)
//    private PlayerInputActions controls; // допустим, у нас сгенерирован класс PlayerInputActions

//    private InputAction moveAction;
//    private InputAction aimAction;
//    private InputAction fireAction;

//    void Awake()
//    {
//        // Инициализируем Input Actions
//        controls = new PlayerInputActions(); // если вы назвали иначе, используйте ваш класс или InputActionAsset
//        moveAction = controls.Player.Move;
//        aimAction = controls.Player.Aim;
//        fireAction = controls.Player.Fire;
//    }

//    void OnEnable()
//    {
//        // Включаем действия
//        controls.Player.Enable();
//    }

//    void OnDisable()
//    {
//        controls.Player.Disable();
//    }

//    void Update()
//    {
//        // 1. Чтение ввода движения
//        Vector2 moveInput = moveAction.ReadValue<Vector2>();
//        float forward = moveInput.y;    // вперед/назад (W/S)
//        float turn = moveInput.x;       // поворот влево/вправо (A/D)

//        // Перемещение корпуса танка
//        // Поворот: вращаем вокруг вертикальной оси
//        transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f);
//        // Движение: едем вперед/назад по локальной оси Z
//        Vector3 moveVector = transform.forward * (forward * moveSpeed * Time.deltaTime);
//        transform.position += moveVector;

//        // 2. Чтение ввода прицеливания
//        Vector2 aimInput = aimAction.ReadValue<Vector2>();
//        float aimHorizontal = aimInput.x;
//        // Поворачиваем башню вокруг вертикальной оси (локальной относительно корпуса)
//        if (turretTransform != null)
//        {
//            turretTransform.Rotate(0f, aimHorizontal * turretTurnSpeed * Time.deltaTime, 0f);
//        }

//        // 3. Выстрел при нажатии кнопки Fire
//        if (fireAction.WasPerformedThisFrame()) // или fireAction.triggered
//        {
//            Shoot();
//        }
//    }

//    void Shoot()
//    {
//        if (shellPrefab != null && firePoint != null)
//        {
//            // Создаем снаряд и придаем ему импульс вперед от ствола
//            GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
//            // Если у снаряда есть Rigidbody, придать ему силу:
//            Rigidbody rb = shell.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                rb.velocity = firePoint.forward * 20f; // например, летит вперед с скоростью 20
//            }
//        }
//    }
//}
