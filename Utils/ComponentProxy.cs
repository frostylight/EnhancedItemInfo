using System;
using UnityEngine;

namespace EnhancedItemInfo.Utils;

public class ComponentProxy: MonoBehaviour {
    public Action? OnDisabled = null;
    public Action? OnEnabled = null;
    public Action? OnDestroyed = null;

    public void OnEnable() => OnEnabled?.Invoke();
    public void OnDisable() => OnDisabled?.Invoke();
    public void OnDestroy() => OnDestroyed?.Invoke();
}
