using UnityEditor.Rendering;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    // Inspector で明示的に指定も可能（空なら子階層を自動探索）
    [SerializeField] private Collider targetCollider;

    private Renderer[] targetRenderers;

    private Material[][] cachedMaterials;
    
    private Color[][] originalColors;

    private void Awake()
    {
        if (targetCollider == null)
            targetCollider = GetComponentInChildren<Collider>();
        
        Debug.Log("targetColider: "+targetCollider);

        targetRenderers = GetComponentsInChildren<Renderer>();

        cachedMaterials = new Material[targetRenderers.Length][];
        originalColors = new Color[targetRenderers.Length][];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            cachedMaterials[i] = targetRenderers[i].materials;
            
            originalColors[i] = new Color[cachedMaterials[i].Length];

            for (int j = 0; j < cachedMaterials[i].Length; j++)
            {
                originalColors[i][j] = cachedMaterials[i][j].color;
            }
        }
    }

    public void OnSelected()
    {
        if (cachedMaterials == null) return;

        for (int i = 0; i < cachedMaterials.Length; i++)
        {
            for (int j = 0; j < cachedMaterials[i].Length; j++)
            {
                cachedMaterials[i][j].color = Color.red; 
            }
        }
    }

    public void OnDeselected()
    {
        if (cachedMaterials == null || originalColors == null) return;

        Debug.Log("選択解除されました");

        for (int i = 0; i < cachedMaterials.Length; i++)
        {
            for (int j = 0; j < cachedMaterials[i].Length; j++)
            {
                cachedMaterials[i][j].color = originalColors[i][j]; 
            }
        }
    }

    public void OnDropped()
    {
        Debug.Log(gameObject.name + " がドロップされました");
    }
}
