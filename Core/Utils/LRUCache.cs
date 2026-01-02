using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnhancedItemInfo.Core.Utils;

/// <summary>
/// LRU缓存的Dictionary，效率存疑
/// 线程安全，但无脑加锁
/// </summary>
/// <param name="capacity">缓存大小</param>
public class LRUCache<TKey, TValue>(int capacity) where TValue : notnull {
    sealed class Record(TValue value, LinkedListNode<TKey> node) {
        public TValue Value = value;
        public LinkedListNode<TKey> Node = node;
    }

    readonly Dictionary<TKey, Record> map = new(capacity);
    readonly LinkedList<TKey> list = [];

    /// <summary>
    /// 缓存大小
    /// </summary>
    public int Capacity {
        get;
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(Capacity));
            }
            field = value;
            while (map.Count > value) {
                PopLeastRecent();
            }
        }
    } = capacity;
    /// <summary>
    /// 已使用的缓存大小
    /// </summary>
    public int Count => map.Count;

#if DEBUG
    int access = 0;
    int hit = 0;
    /// <summary>
    /// 缓存命中次数
    /// </summary>
    public int Hit => hit;

    /// <summary>
    /// 缓存访问次数
    /// </summary>
    public int Access => access;

    /// <summary>
    /// 缓存命中概率
    /// </summary>
    public float HitRate => access == 0 ? 0f : 1f * hit / access;

    public (int Hit, int Access, float HitRate) Status => (hit, access, access == 0 ? 0f : 1f * hit / access);
#endif

    void MoveToRecent(LinkedListNode<TKey> node) {
        list.Remove(node);
        list.AddLast(node);
    }
    void PopLeastRecent() {
        if (list.First == null) {
            return;
        }
        var first = list.First;
        map.Remove(first.Value);
        list.RemoveFirst();
    }

    /// <summary>
    /// 增加缓存，如果key存在会覆盖
    /// </summary>
    public void Add(TKey key, TValue value) {
        if (map.TryGetValue(key, out var record)) {
            record.Value = value;
            MoveToRecent(record.Node);
            return;
        }
        // 新增
        if (map.Count >= Capacity) {
            PopLeastRecent();
        }
        map.Add(key, new(value, list.AddLast(key)));
    }
    /// <summary>
    /// 移除指定缓存
    /// </summary>
    /// <param name="key">待移除缓存的key</param>
    /// <returns>若key存在则返回true，否则返回false</returns>
    public bool Remove(TKey key) {
        if (map.TryGetValue(key, out var record)) {
            map.Remove(key);
            list.Remove(record.Node);
            return true;
        }
        return false;
    }


    /// <summary>
    /// 尝试读取缓存
    /// </summary>
    /// <param name="key">查询的键值</param>
    /// <param name="value">成功返回对应值，失败置为default</param>
    /// <returns>成功返回true，失败返回false</returns>
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value) {
#if DEBUG
        access += 1;
#endif
        if (map.TryGetValue(key, out var record)) {
#if DEBUG
            hit += 1;
#endif
            value = record.Value;
            MoveToRecent(record.Node);
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// 获取或增加缓存 <br/>
    /// 约等于 
    /// <code>
    /// if(TryGetValue(key, out var value)) return value;
    /// value = factory(key);
    /// Add(key, value);
    /// return value;
    /// </code>
    /// </summary>
    /// <param name="key">查询的键值对</param>
    /// <param name="factory">构造新增值的方法</param>
    /// <returns>获取的缓存或新增值</returns>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory) {
#if DEBUG
        access += 1;
#endif
        if (map.TryGetValue(key, out var record)) {
#if DEBUG
            hit += 1;
#endif
            MoveToRecent(record.Node);
            return record.Value;
        }
        if (map.Count >= Capacity) {
            PopLeastRecent();
        }
        var value = factory(key);
        map.Add(key, new(value, list.AddLast(key)));
        return value;
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void Clear() {
#if DEBUG
        hit = access = 0;
#endif
        map.Clear();
        list.Clear();
    }
}
