using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

// ボタンの名前とプレハブを紐づけるためのデータクラス
[System.Serializable]
public class SpawnableItem
{
    [Tooltip("UI Builderで設定したボタンのName属性")]
    public string buttonName;
    
    [Tooltip("生成するプレハブ")]
    public GameObject prefab;
}

public class ConstructionManager : MonoBehaviour
{
    [Header("生成するアイテムのリスト")]
    [Tooltip("インスペクターの「+」ボタンを押して、ボタン名とプレハブを登録してください")]
    public List<SpawnableItem> itemsToSpawn;

    [Header("生成位置の基準")]
    public Vector3 defaultSpawnPosition = new Vector3(0, 0, 0);

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        var root = uiDocument.rootVisualElement;

        // リストに登録されたすべてのアイテムに対して自動でクリックイベントを登録
        foreach (var item in itemsToSpawn)
        {
            var button = root.Q<Button>(item.buttonName);
            if (button != null)
            {
                button.clicked += () => SpawnItem(item.prefab);
            }
        }
    }

    private void SpawnItem(GameObject prefab)
    {
        if (prefab == null) return;
        Instantiate(prefab, defaultSpawnPosition, Quaternion.identity);
    }
}
