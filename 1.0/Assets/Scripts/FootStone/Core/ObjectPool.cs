using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FootStone
{
    //-------------------------------------------------------------------------------------------------------------------------------

    public abstract class ObjectPool
    {
        #region Statistics
        class Tuple
        {
            internal long AllocCount = 0;
            internal long PoolCount = 0;
            internal long DropCount = 0;
            internal Tuple Clone()
            {
                return new Tuple()
                {
                    AllocCount = this.AllocCount,
                    PoolCount = this.PoolCount,
                    DropCount = this.DropCount
                };
            }
        }
        private static long s_total_alloc = 0;
        private static long s_total_pool = 0;
        private static long s_total_drop = 0;
        private static HashMap<Type, Tuple> s_statistics = new HashMap<Type, Tuple>();
        private static List<ObjectPool> s_instances = new List<ObjectPool>();
        private static bool is_statistics = false;
        public static bool EnableObjectPool
        {
            protected get; set;
        } = true;
        public static int MaxObjectCount
        {
            get; set;
        }
        public static long TotalAllocCount
        {
            get { lock (s_statistics) return s_total_alloc; }
        }
        public static long TotalPoolCount
        {
            get { lock (s_statistics) return s_total_pool; }
        }
        public static long TotalDropCount
        {
            get { lock (s_statistics) return s_total_drop; }
        }
        public static bool EnableStatistics
        {
            get { return is_statistics; }
            set { is_statistics = value; }
        }
        protected static void RecordAlloc(Type type)
        {
            if (is_statistics)
            {
                lock (s_statistics)
                {
                    s_total_alloc++;
                    var ac = s_statistics.GetOrAdd(type, (t) => new Tuple());
                    ac.AllocCount++;
                }
            }
        }
        protected static void RecordInPool(Type type)
        {
            if (is_statistics)
            {
                lock (s_statistics)
                {
                    s_total_pool++;
                    var ac = s_statistics.GetOrAdd(type, (t) => new Tuple());
                    ac.PoolCount++;
                }
            }
        }
        protected static void RecordOutPool(Type type)
        {
            if (is_statistics)
            {
                lock (s_statistics)
                {
                    s_total_pool--;
                    var ac = s_statistics.GetOrAdd(type, (t) => new Tuple());
                    ac.PoolCount--;
                }
            }
        }
        protected static void RecordDropPool(Type type)
        {
            if (is_statistics)
            {
                lock (s_statistics)
                {
                    s_total_drop++;
                    var ac = s_statistics.GetOrAdd(type, (t) => new Tuple());
                    ac.DropCount++;
                }
            }
        }
        public static void PrintStatus(TextWriter output, string prefix = "  ", int namePlaceHolder = 16, int totalPlaceHolder = 64)
        {
            if (is_statistics)
            {
                var map = new SortedDictionary<Type, Tuple>(new TypeComparer());
                long total_alloc;
                long total_pool;
                long total_drop;
                lock (s_statistics)
                {
                    total_alloc = s_total_alloc;
                    total_pool = s_total_pool;
                    total_drop = s_total_drop;
                    foreach (var e in s_statistics)
                    {
                        map.Add(e.Key, e.Value.Clone());
                    }
                }
                output.PrintTitle("Object Pool", "Alloc Infomation", prefix, namePlaceHolder);
                foreach (var e in map)
                {
                    if (e.Value.DropCount > 0)
                    {
                        output.PrintLine(string.Format($"{e.Value.PoolCount}(+{e.Value.DropCount}) / {e.Value.AllocCount}"), e.Key.ToVisibleName(), prefix, namePlaceHolder);
                    }
                    else
                    {
                        output.PrintLine(string.Format($"{e.Value.PoolCount} / {e.Value.AllocCount}"), e.Key.ToVisibleName(), prefix, namePlaceHolder);
                    }
                }
                output.PrintLine(string.Format($"{total_pool}(+{total_drop}) / {total_alloc}"), "[Total]", prefix, namePlaceHolder);
            }
        }
        public static void ClearPool()
        {
            if (is_statistics)
            {
                lock (s_statistics)
                {
                    s_total_alloc = 0;
                    s_total_pool = 0;
                    s_total_drop = 0;
                    s_statistics.Clear();
                }
            }
            lock (s_statistics)
            {
                foreach (var pool in s_instances)
                {
                    pool.Clear();
                }
            }
        }
        #endregion
        protected ObjectPool(Type type)
        {
        }
        protected void RegistGlobal()
        {
            lock (s_instances) s_instances.Add(this);
        }
        protected void UnregistGlobal()
        {
            lock (s_instances) s_instances.Remove(this);
        }
        protected abstract void Clear();

    }

    //-------------------------------------------------------------------------------------------------------------------------------
    public abstract class AbstractObjectPool<T> : ObjectPool
    {
        public delegate T ActionCreater();
        protected readonly ActionCreater m_ActionOnCreate;
        protected readonly Action<T> m_ActionOnGet;
        protected readonly Action<T> m_ActionOnRelease;
        public abstract int StackCount { get; }
        public AbstractObjectPool(ActionCreater actionCreate = null, Action<T> actionOnGet = null, Action<T> actionOnRelease = null) : base(typeof(T))
        {
            this.m_ActionOnGet = actionOnGet;
            this.m_ActionOnRelease = actionOnRelease;
            this.m_ActionOnCreate = actionCreate;
        }
        public virtual T CreateNew()
        {
            if (m_ActionOnCreate != null)
            {
                return m_ActionOnCreate();
            }
            else
            {
                return ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
            }
        }
        public abstract T Get();
        public abstract void Release(T element);
    }
    //-------------------------------------------------------------------------------------------------------------------------------
    public class ObjectPool<T> : AbstractObjectPool<T>
    {
        public override int StackCount => m_Stack.Count;
        public ObjectPool(ActionCreater actionCreate = null, Action<T> actionOnGet = null, Action<T> actionOnRelease = null)
            : base(actionCreate, actionOnGet, actionOnRelease)
        {
            this.RegistGlobal();
        }
        public override T Get()
        {
            if (EnableObjectPool)
            {
                if (this.TryPopInternal(out var element))
                {
                    RecordOutPool(element.GetType());
                }
                else
                {
                    element = CreateNew();
                    RecordAlloc(element.GetType());
                }
                if (this.m_ActionOnGet != null)
                {
                    this.m_ActionOnGet(element);
                }
                return element;
            }
            else
            {
                var element = CreateNew();
                if (this.m_ActionOnGet != null)
                {
                    this.m_ActionOnGet(element);
                }
                return element;
            }
        }
        public override void Release(T element)
        {
            if (EnableObjectPool)
            {
                if (EnableStatistics)
                {
                    if (this.TryPeekInternal(out var top) && object.ReferenceEquals(top, element))
                    {
                        throw new Exception("Internal error. Trying to destroy object that is already released to pool.");
                    }
                }
                if (this.m_ActionOnRelease != null)
                {
                    this.m_ActionOnRelease(element);
                }
                if (MaxObjectCount > 0 && this.StackCount >= MaxObjectCount)
                {
                    RecordDropPool(element.GetType());
                    return;
                }
                this.PushInternal(element);
                RecordInPool(element.GetType());
            }
            else
            {
                if (this.m_ActionOnRelease != null)
                {
                    this.m_ActionOnRelease(element);
                }
            }
        }
//#if NETSTANDARD
        private readonly ConcurrentBag<T> m_Stack = new ConcurrentBag<T>();
        protected override void Clear()
        {
            while (m_Stack.TryTake(out var e)) ;
        }
        private bool TryPopInternal(out T ret)
        {
            return this.m_Stack.TryTake(out ret);
        }
        private bool TryPeekInternal(out T ret)
        {
            return this.m_Stack.TryPeek(out ret);
        }
        private void PushInternal(T element)
        {
            this.m_Stack.Add(element);
        }
//#else
//        private readonly Stack<T> m_Stack = new Stack<T>();
//        protected override void Clear()
//        {
//            lock (m_Stack) m_Stack.Clear();
//        }
//        private bool TryPopInternal(out T ret)
//        {
//            return this.m_Stack.SynchronizedTryTake(out ret);
//        }
//        private bool TryPeekInternal(out T ret)
//        {
//            return this.m_Stack.SynchronizedTryPeek(out ret);
//        }
//        private void PushInternal(T element)
//        {
//            lock (m_Stack) { this.m_Stack.Push(element); }
//        }
//#endif
    }
    //-------------------------------------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------------------------------------
    public static class StringBuilderObjectPool
    {
        private static readonly ObjectPool<AutoRelease> s_Pool = new ObjectPool<AutoRelease>(s_ListPool_OnCreate, null, null);
        private static AutoRelease s_ListPool_OnCreate()
        {
            return new AutoRelease();
        }
        public static AutoRelease AllocAutoRelease()
        {
            AutoRelease ret = s_Pool.Get() as AutoRelease;
            return ret;
        }
        private static void Release(AutoRelease toRelease)
        {
            s_Pool.Release(toRelease);
        }
        public class AutoRelease : StringWriter
        {
            internal AutoRelease() : base(new StringBuilder()) { }
            public override Encoding Encoding { get { return CUtils.UTF8; } }
            public StringBuilder Output { get { return GetStringBuilder(); } }
            public override string ToString()
            {
                return GetStringBuilder().ToString();
            }
            protected override void Dispose(bool disposing)
            {
                //base.Dispose(disposing);
                GetStringBuilder().Remove(0, GetStringBuilder().Length);
                StringBuilderObjectPool.Release(this);
            }
        }
    }
    //-------------------------------------------------------------------------------------------------------------------------------
}
