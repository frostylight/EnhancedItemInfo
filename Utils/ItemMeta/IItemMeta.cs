using Duckov.Utilities;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedItemInfo.Utils.ItemMeta;

public interface IItemMeta {
    public int TypeID { get; }
    public string Name { get; }
    public int Quality { get; }
    public DisplayQuality DisplayQuality { get; }
    public IEnumerable<Tag>? Tags { get; }
    public int PriceEach { get; }
}
