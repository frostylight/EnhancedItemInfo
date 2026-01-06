using Duckov.Modding;
using EnhancedItemInfo.Core;
using EnhancedItemInfo.Utils;
using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedItemInfo.Config;

using AddToggleType = Action<ModInfo, string, string, bool, Action<bool>?>;

internal static class ModSettingAPI {
    enum State {
        Unbound,
        Enable,
        Disabled
    }

    static Type? ModSettingType = null;
    static readonly Version version = new(0, 5, 0);

    static State state = State.Unbound;
    static bool Enable => state == State.Enable;
    static bool Disable => state == State.Disabled;
    static bool UnBound => state == State.Unbound;

    public static bool Init() {
        if (!UnBound) {
            return Enable;
        }
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            ModSettingType = asm.GetType(Constant.ModSettingAPI_FullName);
            if (ModSettingType != null) {
                break;
            }
        }
        if (ModSettingType == null) {
            Logger.Info("ModSetting not found");
            return false;
        }
        state = State.Enable;
        var remoteVersion = Traverse.Create(ModSettingType).Field("VERSION").GetValue<Version>();
        if (remoteVersion == null) {
            Logger.Warn($"Unable to check version of ModSetting");
        }
        else if (remoteVersion < version) {
            Logger.Warn($"ModSetting({remoteVersion}) older than {version}");
        }
        return true;
    }

    class MethodState<T> where T : Delegate {
        public State state = State.Unbound;
        public MethodInfo? methodInfo = null;
        public T? methodDelegate = null;

        public bool Enable => state == State.Enable;
        public bool Disable => state == State.Disabled;
        public bool UnBound => state == State.Unbound;

        public bool Init(string name, Type[] param) {
            if (ModSettingAPI.Disable) {
                state = State.Disabled;
                return false;
            }
            if (!ModSettingAPI.Enable) {
                return false;
            }
            if (!UnBound) {
                return Enable;
            }
            methodInfo = AccessTools.DeclaredMethod(ModSettingType!, name, param);
            if (methodInfo == null) {
                Logger.Error($"Failed to get MethodInfo of {name}");
                state = State.Disabled;
                return false;
            }
            state = State.Enable;
            try {
                methodDelegate = (T)Delegate.CreateDelegate(typeof(T), methodInfo);
            }
            catch (Exception ex) {
                Logger.Error($"Failed to create delegate of {name}", ex);
                methodDelegate = null;
            }
            return true;
        }
    }

    static readonly MethodState<AddToggleType> AddToggleState = new();
    public static bool AddToggle(ModInfo modInfo, string key, string description, bool enable, Action<bool>? onValueChange = null) {
        if (Disable || AddToggleState.Disable) {
            return false;
        }
        if (AddToggleState.UnBound) {
            if (!AddToggleState.Init("AddToggle", [typeof(ModInfo), typeof(string), typeof(string), typeof(bool), typeof(Action<bool>)])) {
                return false;
            }
        }
        if (!AddToggleState.Enable) {
            return false;
        }
        if (AddToggleState.methodDelegate != null) {
            try {
                AddToggleState.methodDelegate(modInfo, key, description, enable, onValueChange);
                return true;
            }
            catch (Exception ex) {
                Logger.Error($"Unable to Invoke AddToggle Delegate", ex);
                AddToggleState.methodDelegate = null;
            }
        }
        try {
            AddToggleState.methodInfo!.Invoke(null, [modInfo, key, description, enable, onValueChange]);
            return true;
        }
        catch (Exception ex) {
            Logger.Error($"Unable to Invoke AddToggle", ex);
            AddToggleState.state = State.Disabled;
        }
        return false;
    }
}
