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
    bool hideOnce = false;

    public ItemInfoUI() { }

    public ItemInfoUI Hide() {
        ItemInfoText.gameObject.SetActive(false);
        hideOnce = false;
        return this;
    }
    public ItemInfoUI HideOnce() {
        ItemInfoText.gameObject.SetActive(false);
        hideOnce = true;
        return this;
    }
    public ItemInfoUI Show() {
        if (hideOnce) {
            hideOnce = false;
            return this;
        }
        ItemInfoText.gameObject.SetActive(true);
        return this;
    }
    public ItemInfoUI SetParent(Transform parent, bool worldPositionStays = true) {
        if (hideOnce) {
            return this;
        }
        ItemInfoText.transform.SetParent(parent, worldPositionStays);
        return this;
    }
    public ItemInfoUI SetColor(Color color) {
        if (hideOnce) {
            return this;
        }
        ItemInfoText.color = color;
        return this;
    }
    public ItemInfoUI SetText(string text) {
        if (hideOnce) {
            return this;
        }
        ItemInfoText.text = text;
        return this;
    }
    public ItemInfoUI AppendText(string text) {
        if (hideOnce) {
            return this;
        }
        ItemInfoText.text += text;
        return this;
    }
    public ItemInfoUI AppendTextIf(bool condition, string text) {
        if (hideOnce) {
            return this;
        }
        if (condition) {
            return AppendText(text);
        }
        return this;
    }
    public ItemInfoUI SetFontSize(float fontSize) {
        if (hideOnce) {
            return this;
        }
        ItemInfoText.fontSize = fontSize;
        return this;
    }
    public ItemInfoUI SetWordWrap(bool wordWrap) {
        if (hideOnce) {
            return this;
        }
        ItemInfoText.enableWordWrapping = wordWrap;
        return this;
    }
}
