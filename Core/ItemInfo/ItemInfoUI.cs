using Duckov.Utilities;
using TMPro;
using UnityEngine;

namespace EnhancedItemInfo.Core.ItemInfo;

/// <summary>
/// 对TextMeshProUGUI的链式调用封装
/// </summary>
public class ItemInfoUI {
    TextMeshProUGUI ItemInfoText {
        get {
            if (field == null) {
                field = UnityEngine.Object.Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                field.gameObject.SetActive(false);
                field.transform.localScale = Vector3.one;
                field.fontSize = 18f;
            }
            return field;
        }
    } = null;
    DetailedCounter Counter {
        get {
            field ??= new();
            return field;
        }
    } = null;
    bool hideOnce = false;
    bool useCounter = false;

    public ItemInfoUI() { }

    public ItemInfoUI Hide() {
        ItemInfoText.gameObject.SetActive(false);
        hideOnce = false;
        useCounter = false;
        return this;
    }
    public ItemInfoUI HideOnce() {
        ItemInfoText.gameObject.SetActive(false);
        hideOnce = true;
        useCounter = false;
        return this;
    }
    public ItemInfoUI Show() {
        if (!hideOnce) {
            if (useCounter) {
                ItemInfoText.text = Counter.ToString();
                useCounter = false;
            }
            ItemInfoText.gameObject.SetActive(true);
        }
        hideOnce = false;
        return this;
    }
    public ItemInfoUI SetParent(Transform parent, bool worldPositionStays = true) {
        if (!hideOnce) {
            ItemInfoText.transform.SetParent(parent, worldPositionStays);
            // 保证文本相对位置
            ItemInfoText.transform.SetAsLastSibling();
        }
        return this;
    }
    public ItemInfoUI SetColor(Color color) {
        if (!hideOnce) {
            ItemInfoText.color = color;
        }
        return this;
    }
    public ItemInfoUI SetText(string text) {
        if (!hideOnce) {
            if (!useCounter) {
                ItemInfoText.text = text;
            }
        }
        return this;
    }
    public ItemInfoUI AppendText(string text) {
        if (!hideOnce) {
            if (!useCounter) {
                ItemInfoText.text += text;
            }
        }
        return this;
    }
    public ItemInfoUI AppendTextIf(bool condition, string text) {
        if (!hideOnce) {
            if (condition) {
                return AppendText(text);
            }
        }
        return this;
    }
    public ItemInfoUI SetFontSize(float fontSize) {
        if (!hideOnce) {
            ItemInfoText.fontSize = fontSize;
        }
        return this;
    }
    public ItemInfoUI SetWordWrap(bool wordWrap) {
        if (!hideOnce) {
            ItemInfoText.enableWordWrapping = wordWrap;
        }
        return this;
    }
    public ItemInfoUI UseCounter(string prefix) {
        if (!hideOnce) {
            useCounter = true;
            Counter.Clear(prefix);
        }
        return this;
    }
    public ItemInfoUI AddPart(string name, long value) {
        if (!hideOnce) {
            if (useCounter) {
                Counter.AddPart(name, value);
            }
        }
        return this;
    }

    public void Destroy() {
        ItemInfoText.gameObject.SetActive(false);
        UnityEngine.Object.Destroy(ItemInfoText);
    }
}
