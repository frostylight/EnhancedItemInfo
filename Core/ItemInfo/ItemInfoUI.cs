using Duckov.UI;
using Duckov.Utilities;
using EnhancedItemInfo.Utils;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

namespace EnhancedItemInfo.Core.ItemInfo;

internal abstract class ItemInfoUI<T> where T : ItemInfoUI<T> {
    TextMeshProUGUI? _text = null;
    protected TextMeshProUGUI Text {
        get {
            if (_text == null) {
                _text = UnityEngine.Object.Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                _text.gameObject.SetActive(false);
                _text.transform.localScale = Vector3.one;
                _text.fontSize = 18f;
            }
            return _text;
        }
    }

    protected bool Instantiated => _text != null && _text;
    protected virtual bool Enable => true;
    protected virtual bool ColoredInfo => ItemInfoManager.EnableColoredInfo;
    protected T Self => (T)this;
    protected ItemInfoUI() { }

    protected abstract bool Setup(Item item);
    protected abstract bool Setup(ItemMetaData itemMetaData);
    public void SetupAndShow(ItemHoveringUI uiInstance, Item item) {
        if (!Enable) {
            return;
        }
        if (Setup(item)) {
            if (ColoredInfo) {
                SetColor(item.GetLevelColor().WithAlpha(1f));
            }
            SetParent(uiInstance.LayoutParent);
            Show();
        }
    }
    public void SetupAndShow(ItemHoveringUI uiInstance, ItemMetaData itemMetaData) {
        if (!Enable) {
            return;
        }
        if (Setup(itemMetaData)) {
            if (ColoredInfo) {
                SetColor(itemMetaData.GetLevelColor().WithAlpha(1f));
            }
            SetParent(uiInstance.LayoutParent);
            Show();
        }
    }

    public T Hide() {
        if (Instantiated) {
            Text.gameObject.SetActive(false);
        }
        return Self;
    }
    public T Show() {
        Text.gameObject.SetActive(true);
        return Self;
    }
    protected T SetParent(Transform parent, bool worldPositionStays = true) {
        Text.transform.SetParent(parent, worldPositionStays);
        return Self;
    }
    protected T SetAsLastSibling() {
        Text.transform.SetAsLastSibling();
        return Self;
    }
    protected T SetColor(Color color) {
        Text.color = color;
        return Self;
    }
    protected T SetText(string text) {
        Text.text = text;
        return Self;
    }
    protected T AppendText(string text) {
        Text.text += text;
        return Self;
    }
    protected T AppendTextIf(bool condition, string text) {
        if (condition) {
            return AppendText(text);
        }
        return Self;
    }
    protected T SetFontSize(float fontSize) {
        Text.fontSize = fontSize;
        return Self;
    }

    public virtual void Destroy() {
        if (Instantiated) {
            Text.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(Text);
        }
    }
}
