using Duckov.Utilities;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedItemInfo.Utils.ItemMeta;

public sealed class ItemAdapter(Item item): IItemMeta {
    readonly Item item = item;
    public int TypeID => item.TypeID;
    public string Name => item.name;
    public int Quality => item.Quality;
    public DisplayQuality DisplayQuality => item.DisplayQuality;
    public IEnumerable<Tag>? Tags => item.Tags;
    public int PriceEach => item.Value;
}
