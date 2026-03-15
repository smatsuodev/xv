using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public enum ViewMode
    {
        FirstPerson,
        ThirdPerson
    }

    [Header("View Settings")]
    public ViewMode currentMode = ViewMode.FirstPerson;

    [Header("Movement Settings")]
    [Tooltip("地上での移動速度")]
    public float groundMoveSpeed = 5f;
    [Tooltip("宇宙空間での移動速度")]
    public float spaceMoveSpeed = 15f;
    [Header("Rotation Settings")]
    [Tooltip("マウス感度")]
    public float lookSensitivity = 0.1f;
    public float maxPitch = 85f;
    public float minPitch = -85f;

    [Header("First Person Settings")]
    [Tooltip("地上から目の高さまでのオフセット")]
    public float eyeHeight = 1.5f;

    [Header("Third Person Settings")]
    [Tooltip("ターゲットからのカメラのオフセット位置")]
    public Vector3 thirdPersonOffset = new Vector3(0f, 2f, -5f);

    private float pitch = 0f;
    private float yaw = 0f;

    // プレイヤーの仮想的な中心位置
    private Vector3 targetPosition;

    void Start()
    {
        // 開始時はカーソルを表示（回転はクリック中のみ）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        targetPosition = transform.position;
        if (currentMode == ViewMode.FirstPerson)
        {
            // 一人称視点の場合、開始時のY座標から目の高さを引いた位置を足元とする
            targetPosition.y -= eyeHeight;
        }

        // 現在のカメラの回転を取得
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 視点切り替え (Vキー)
        if (keyboard.vKey.wasPressedThisFrame)
        {
            SwitchViewMode();
        }

        HandleRotation();
        HandleMovement(keyboard);
        ApplyTransform();
    }

    private void SwitchViewMode()
    {
        if (currentMode == ViewMode.FirstPerson)
        {
            currentMode = ViewMode.ThirdPerson;
        }
        else
        {
            currentMode = ViewMode.FirstPerson;
        }
    }

    private void HandleRotation()
    {
        // Debug.Log();

        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        // {
        //     return; 
        // }

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // クリック中（左ボタンまたは右ボタン）のみ回転を許可
        // if (mouse.leftButton.isPressed || mouse.rightButton.isPressed)
        if (mouse.rightButton.isPressed)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Vector2 mouseDelta = mouse.delta.ReadValue();

            float mouseX = mouseDelta.x * lookSensitivity;
            float mouseY = mouseDelta.y * lookSensitivity;

            yaw += mouseX;
            pitch -= mouseY;

            // ピッチ（上下の回転）を制限
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void HandleMovement(Keyboard keyboard)
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveZ += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveZ -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX -= 1f;

        Vector3 moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        if (currentMode == ViewMode.FirstPerson)
        {
            // 【一人称視点】地上の移動 (Y軸の回転のみを考慮し、水平に移動)
            Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;

            Vector3 moveDirection = forward * moveInput.z + right * moveInput.x;
            targetPosition += moveDirection * (groundMoveSpeed * Time.deltaTime);
        }
        else if (currentMode == ViewMode.ThirdPerson)
        {
            // 【三人称視点】宇宙空間の移動 (カメラの向いている全方向を考慮)
            Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 forward = cameraRotation * Vector3.forward;
            Vector3 right = cameraRotation * Vector3.right;
            Vector3 up = cameraRotation * Vector3.up;

            // Q/Eキーで上昇・下降機能 (Eで上昇、Qで下降)
            float moveY = 0f;
            if (keyboard.eKey.isPressed) moveY = 1f;
            if (keyboard.qKey.isPressed) moveY = -1f;

            Vector3 moveDirection = forward * moveInput.z + right * moveInput.x + up * moveY;
            targetPosition += moveDirection * (spaceMoveSpeed * Time.deltaTime);
        }
    }

    private void ApplyTransform()
    {
        // 回転の適用
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = targetRotation;

        // 位置の適用
        if (currentMode == ViewMode.FirstPerson)
        {
            // 一人称：ターゲット位置（足元）＋目の高さ
            transform.position = targetPosition + new Vector3(0f, eyeHeight, 0f);
        }
        else if (currentMode == ViewMode.ThirdPerson)
        {
            // 三人称：ターゲット位置からオフセット分だけ後ろに下がる
            transform.position = targetPosition + targetRotation * thirdPersonOffset;
        }
    }
}
