using EnhancedItemInfo.Attributes;
using System.Collections.Generic;

namespace EnhancedItemInfo.Config.ConfigItem;

internal class Group(string key, string description = "", float scale = 0.7f, bool open = false) {
    public string Key { get; } = key;
    public string Description { get; set; } = description;
    public List<string> Keys { get; } = [];
    public float Scale { get; set; } = scale;
    public bool Open { get; set; } = open;

    public Group(ConfigGroupAttribute attr)
        : this(attr.Key, attr.Description, attr.Scale, attr.Open) { }
}
