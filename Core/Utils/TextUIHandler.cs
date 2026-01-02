using TMPro;
using UnityEngine;

namespace EnhancedItemInfo.Core.Utils;

public class TextUIHandler<TSelf> where TSelf : TextUIHandler<TSelf> {
    protected TextMeshProUGUI? textui;

    protected TSelf Self => (TSelf)this;

    protected TextUIHandler() {
        textui = null;
    }
    protected TextUIHandler(TextMeshProUGUI textui) {
        this.textui = textui;
    }

    public bool IsAttached => textui;
    public virtual TSelf Attach(TextMeshProUGUI textui) {
#if DEBUG
        if (textui == null) {
            Logger.Debug("Try to Attach null TextMeshProUGUI");
        }
        else if (this.textui != null) {
            Logger.Debug("Attach to a new TextMeshProUGUI before Detach");
        }
#endif
        this.textui = textui;
        return Self;
    }
    public virtual TextMeshProUGUI? Detach() {
        var ret = textui;
        textui = null;
        return ret;
    }

    public virtual TSelf Hide() {
        textui?.gameObject.SetActive(false);
        return Self;
    }
    public virtual TSelf Show() {
        textui?.gameObject.SetActive(true);
        return Self;
    }
    public virtual TSelf SetParent(Transform parent, bool worldPositionStays = true) {
        textui?.transform.SetParent(parent, worldPositionStays);
        return Self;
    }
    public virtual TSelf SetColor(Color color) {
        textui?.color = color;
        return Self;
    }
    public virtual TSelf SetText(string text) {
        textui?.text = text;
        return Self;
    }
    public virtual TSelf SetFontSize(float fontSize) {
        textui?.fontSize = fontSize;
        return Self;
    }
    public virtual TSelf SetAlignment(TextAlignmentOptions alignment) {
        textui?.alignment = alignment;
        return Self;
    }
}

public class TextUIHandler: TextUIHandler<TextUIHandler> {
    public TextUIHandler() : base() { }
    public TextUIHandler(TextMeshProUGUI textui) : base(textui) { }
}