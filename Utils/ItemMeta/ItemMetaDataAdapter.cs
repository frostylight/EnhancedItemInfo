using Duckov.Utilities;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedItemInfo.Utils.ItemMeta;

public readonly struct ItemMetaDataAdapter(ItemMetaData itemMetaData): IItemMeta {
    readonly ItemMetaData itemMetaData = itemMetaData;
    public int TypeID => itemMetaData.id;
    public string Name => itemMetaData.Name;
    public int Quality => itemMetaData.quality;
    public DisplayQuality DisplayQuality => itemMetaData.displayQuality;
    public IEnumerable<Tag>? Tags => itemMetaData.tags;
    public int PriceEach => itemMetaData.priceEach;
}

