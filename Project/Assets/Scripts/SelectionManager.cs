using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private float dragHeight; // ドラッグ中の浮かせる高さ

    private Selectable selectedObject;
    private float dragDepth;
    private Vector3 offset;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update() {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        
        // --- クリックで選択 ---
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("start raycast");
            Ray ray = mainCamera.ScreenPointToRay(mouse.position.value);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayer))
            {
                var selectable = hit.collider.GetComponent<Selectable>();
                Debug.Log("Ray hit object: " + hit.collider.name);
                if (selectable != null)
                {
                    Select(selectable);
                    // ドラッグ用の奥行きとオフセットを記録
                    dragDepth = mainCamera.WorldToScreenPoint(selectable.transform.position).z;
                    Vector3 worldMouse = GetMouseWorldPosition();
                    offset = selectable.transform.position - worldMouse;
                }
            }
            else
            {
                Debug.Log("Ray missed");
                Deselect();
            }
        }

        // --- ドラッグで移動 ---
        if (mouse.leftButton.isPressed && selectedObject != null)
        {
            Vector3 targetPos = GetMouseWorldPosition() + offset;
            targetPos.y += dragHeight;
            selectedObject.transform.position = Vector3.Lerp(
                selectedObject.transform.position, targetPos, 20f * Time.deltaTime
            );
        }

        // --- ドロップ ---
        if (mouse.leftButton.wasReleasedThisFrame && selectedObject != null)
        {
            selectedObject.OnDropped();
        }

        if (keyboard != null && (keyboard.deleteKey.wasPressedThisFrame || keyboard.backspaceKey.wasPressedThisFrame))
        {
            if (selectedObject != null)
            {
                Destroy(selectedObject.gameObject);
                selectedObject = null;
                Debug.Log("選択中のオブジェクトを削除しました");
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mouse = Mouse.current;
        Vector3 screenPos = mouse.position.value;
        screenPos.z = dragDepth;
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    private void Select(Selectable obj)
    {
        if (selectedObject != null)
            selectedObject.OnDeselected();

        selectedObject = obj;
        selectedObject.OnSelected();
    }

    private void Deselect()
    {
        if (selectedObject != null)
        {
            selectedObject.OnDeselected();
            selectedObject = null;
        }
    }
}