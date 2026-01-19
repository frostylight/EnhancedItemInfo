using Duckov.UI;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core.ItemInfo;

internal class SidePanel(string name = "SidePanel") {
    readonly string name = name;
    GameObject Panel {
        get {
            if (field == null || !field) {
                field = CreatePanel(name);
            }
            return field;
        }
    }
    public bool Inited { get; private set; } = false;
    public Transform LayoutParent => Panel.transform;

    private static GameObject CreatePanel(string name) {
        GameObject panel = new(name);
        panel.SetActive(false);

        var ui = ItemHoveringUI.Instance;
        if (ui == null || !ui) {
            Logger.Error($"Null ItemHoveringUI Instance");
            return panel;
        }
        var layout = ui.LayoutParent;
        ListComponents(layout.gameObject);
        var content = layout.parent;
        ListComponents(content.gameObject);

        panel.AddComponent<CanvasRenderer>();

        var image = panel.AddComponent<Image>();
        var oriImage = layout.GetComponent<Image>();
        image.color = oriImage.color;
        image.sprite = oriImage.sprite;
        image.type = oriImage.type;

        var vlg = panel.GetOrAddComponent<VerticalLayoutGroup>();
        var oriVlg = layout.GetComponent<VerticalLayoutGroup>();
        vlg.padding = oriVlg.padding;
        vlg.spacing = oriVlg.spacing;
        vlg.childAlignment = oriVlg.childAlignment;
        vlg.childForceExpandWidth = oriVlg.childForceExpandWidth;
        vlg.childForceExpandHeight = oriVlg.childForceExpandHeight;

        var column = content.Find("Colomn2");
        if (column != null) {
            ListComponents(column.gameObject);
            Logger.Debug($"Colomn2!");
            panel.transform.SetParent(column, false);
            // 插到侧边栏上面
            panel.transform.SetAsFirstSibling();
        }
        else {
            panel.transform.SetParent(content, false);
            // 插到原来侧边栏左侧
            panel.transform.SetSiblingIndex(layout.GetSiblingIndex() + 1);
        }
        panel.SetActive(true);
        return panel;
    }
    public void Show() {
        Panel?.SetActive(true);
    }
    public void Hide() {
        Panel?.SetActive(false);
    }
    public void SetActive(bool active) {
        Panel?.SetActive(active);
    }
    public static void ListComponents(GameObject gameObject, int indent = 0) {
#if DEBUG
        StringBuilder stringBuilder = new();
        for (int j = 0; j < indent; j++) { stringBuilder.Append("\t"); }
        Logger.Debug($"{stringBuilder}Component of {gameObject.name}");
        stringBuilder.Append("\t");
        int count = gameObject.GetComponentCount();
        for (int i = 0; i < count; i++) {
            var com = gameObject.GetComponentAtIndex(i);
            Logger.Debug($"{stringBuilder} {i} -> {com.GetType()}");
        }
#endif
    }
}
