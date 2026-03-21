using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    // メニュー枠の参照
    private VisualElement constructionMenu;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        var root = uiDocument.rootVisualElement;

        // 1. 各サブメニューのVisualElementを取得
        constructionMenu = root.Q<VisualElement>("ConstructionMenu");
        // animationMenu = root.Q<VisualElement>("AnimationMenu");

        // 2. メインメニューのボタンを取得
        var constructionButton = root.Q<Button>("Construction");
        var animationButton = root.Q<Button>("Animation");
        var saveLoadButton = root.Q<Button>("SaverChanger");
        var optionsButton = root.Q<Button>("Options");
        var viewsReplayButton = root.Q<Button>("ViewsReplay");

        // 3. 各ボタンのコールバックを設定
        if (constructionButton != null) constructionButton.clicked += OnConstructionButtonClicked;

        HideAllMenus();
    }

    private void HideAllMenus()
    {
        if (constructionMenu != null) constructionMenu.style.display = DisplayStyle.None;
    }

    private void OnConstructionButtonClicked()
    {
        if (constructionMenu == null) return;
        bool isVisible = constructionMenu.style.display == DisplayStyle.Flex;
        HideAllMenus();
        if (!isVisible)
        {
            constructionMenu.style.display = DisplayStyle.Flex;
        }
    }
}
