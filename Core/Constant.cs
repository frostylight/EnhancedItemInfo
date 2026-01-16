using Duckov;
using Duckov.Utilities;
using EnhancedItemInfo.Utils;
using UnityEngine;
using Logger = EnhancedItemInfo.Utils.Logger;

namespace EnhancedItemInfo.Core;

public static class Constant {
    public static readonly int CashTypeID = GameplayDataSettings.ItemAssets.CashItemTypeID;

    public static readonly string ItemBulletTag = GameplayDataSettings.Tags.Bullet.name;
    public const string ItemAccessoryTag = "Accessory";
    public const string ItemEquipmentTag = "Equipment";
    public static readonly string ItemSpecialTag = GameplayDataSettings.Tags.Special.name;
    public const string ItemKeyTag = "Key";
    public const string ItemFormulaTag = "Formula_Blueprint";

    public const string RegisteredMarkBackgroundKey = "MarkBackground";
    public const string RegisteredMarkTextKey = "MarkText";
    public static readonly Vector2 MarkPostion = new(-5, -5);
    public static readonly Vector2 MarkSize = new(28, 28);
    public static readonly Color MarkBackgroundColor = ColorUtils.RGBA(0xffffff_cc);
    public static readonly Color MarkColor = Color.magenta;

    public static Color Transparent = ColorUtils.RGBA(0xffffff_00);
    public static Color White = ColorUtils.RGBA(0xffffff_40);
    public static Color Green = ColorUtils.RGBA(0x7cff7c_40);
    public static Color Blue = ColorUtils.RGBA(0x7cd5ff_40);
    public static Color Purple = ColorUtils.RGBA(0xd0acff_40);
    public static Color Orange = ColorUtils.RGBA(0xffdc24_96);
    public static Color LightRed = ColorUtils.RGBA(0xff5858_96);
    public static Color Red = ColorUtils.RGBA(0xbb0000_96);

    public const string ModSettingAPI_FullName = "ModSetting.ModBehaviour";

    public static int MaxLevel {
        get {
            if (field == -1) {
                var expManager = EXPManager.Instance;
                if (expManager == null) {
                    return 99; // 可能会随版本更新
                }
                field = expManager.LevelFromExp(long.MaxValue); // 真 最大等级
                Logger.Debug($"MaxLevel {field}");
            }
            return field;
        }
    } = -1;
}
