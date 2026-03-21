using UnityEditor.Rendering;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    // Inspector で明示的に指定も可能（空なら子階層を自動探索）
    [SerializeField] private Collider targetCollider;

    private Color originalColor;

    private void Awake()
    {
        if (targetCollider == null)
            targetCollider = GetComponentInChildren<Collider>();
        
        Debug.Log("targetColider: "+targetCollider);
    }

    public void OnSelected()
    {
    }

    public void OnDeselected()
    {
    }

    public void OnDropped()
    {
        // ドロップ時の処理（必要に応じてカスタマイズ）
    }
}