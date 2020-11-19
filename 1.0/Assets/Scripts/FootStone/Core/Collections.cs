using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public interface IHashMap<K, V> : IDictionary<K, V>
    {
        V Get(K key);

        void Put(K key, V val);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns>added</returns>
        bool TryAddOrUpdate(K key, V val);
        bool TryAdd(K key, V val);
        bool TryGetOrCreate(K key, out V value, Func<K, V> create);
        V RemoveByKey(K key);
        void PutAll(IDictionary<K, V> map);
    }

    public class HashMap<K, V> : System.Collections.Generic.Dictionary<K, V>, IHashMap<K, V>
    {
        public HashMap() { }
        public HashMap(int capacity) : base(capacity) { }
        public HashMap(IEqualityComparer<K> comparer) : base(comparer) { }
        public HashMap(int capacity, IEqualityComparer<K> comparer) : base(capacity, comparer) { }
        public HashMap(IDictionary<K, V> map) : base(map) { }
        public HashMap(IDictionary<K, V> map, IEqualityComparer<K> comparer) : base(map, comparer) { }

        public V Get(K key)
        {
            V ret;
            if (base.TryGetValue(key, out ret))
            {
                return ret;
            }
            return default(V);
        }



        public void Put(K key, V val)
        {
            this[key] = val;
        }

        public bool TryAdd(K key, V val)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, val);
                return true;
            }
            return false;
        }
        public bool TryAddOrUpdate(K key, V val)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, val);
                return true;
            }
            this[key] = val;
            return false;
        }
        public bool TryGetOrCreate(K key, out V ret, Func<K, V> create)
        {
            if (base.TryGetValue(key, out ret))
            {
                return true;
            }
            ret = create(key);
            base.Add(key, ret);
            return false;
        }
        public V RemoveByKey(K key)
        {
            V ret;
            if (base.TryGetValue(key, out ret))
            {
                base.Remove(key);
            }
            return ret;
        }

        public void PutAll(IDictionary<K, V> map)
        {
            foreach (KeyValuePair<K, V> e in map)
            {
                Put(e.Key, e.Value);
            }
        }
        public void AddAll(IDictionary<K, V> map)
        {
            foreach (KeyValuePair<K, V> e in map)
            {
                Add(e.Key, e.Value);
            }
        }
    }


    public static class CollectionsUtil
    {
        public static V GetOrAdd<K, V>(this IHashMap<K, V> map, K key, Func<K, V> create)
        {
            map.TryGetOrCreate(key, out var ret, create);
            return ret;
        }
        public static V GetOrNew<K, V>(this IHashMap<K, V> map, K key) where V : new()
        {
            map.TryGetOrCreate(key, out var ret, k => new V());
            return ret;
        }
    }
}
