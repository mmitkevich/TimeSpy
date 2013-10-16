using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSpy
{
    public interface ITreeNode
    {
        object Parent { get; }
        System.Collections.ICollection Children { get; }
    }

    public interface IDetail
    {
        string Name { get; }
        T Get<T>(Detailed d, T f = default(T), Func<object,T> conv=null);
        string ToString(object obj, string fmtname = null, Func<object, string> fmt = null);
        Detailed D { get; }
    }


    public class Detailed:Dictionary<string,object>
    {
        public static Dictionary<string, Func<object, string>> Formatters = new Dictionary<string, Func<object, string>>();

        public static T Convert<T>(object obj)
        {
            T result = default(T);
            try
            {
                if (obj != null)
                {
                    bool tryToCast = true;

                    // Checks for the date
                    if (typeof(T) == typeof(DateTime))
                    {
                        if (string.IsNullOrEmpty(obj.ToString()))
                        {
                            tryToCast = false;
                        }
                    }

                    if (tryToCast)
                    {
                        result = (T)System.Convert.ChangeType(obj, typeof(T));
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Failed to convert {0} to type {1}, returned {2}", obj, typeof(T).Name, result);
            }
            return result;

        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public T Get<T>(string name, T f = default(T), Func<object, T> conv = null)
        {
            object obj = Get(name, null);
            if (obj == null)
                return f;
            if (conv == null)
                conv = Convert<T>;
            return conv(obj);
        }
        
        public string ToString(object obj, string fmtname = null, Func<object, string> fmt = null)
        {
            if (obj == null)
                return "";//<NULL>
            if (fmt == null)
            {
                if (fmtname == null)
                    fmtname = (string)Get("Format", "");

                if (!Formatters.TryGetValue(fmtname, out fmt))
                    fmt = x => string.Format("{0:"+fmtname+"}", x);
            }
            return fmt(obj);
        }

        protected static IDetail[] CachedDetails;
        public virtual IDetail[] AllDetails(string scheme = null)
        {
            if (null != CachedDetails)
                return CachedDetails;
            CachedDetails = Keys.Select(k => new TDetail(k)).ToArray();
            return CachedDetails;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kv in this)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
            }
            return sb.ToString();
        }

        public object Get(string d, object f = null)
        {
            TryGetValue(d, out f);
            return f;
        }

        public Detailed Set(string d, object value)
        {
            base[d]=value;
            return this;
        }
        
        public object this[string d]
        {
            get { return Get(d); }
            set { Set(d, value); }
        }

        public Detailed Merge(IDictionary<string,object> other, bool overwrite=true)
        {
            foreach (var kv in other)
            {
                if (overwrite || !ContainsKey(kv.Key))
                    this[kv.Key] = kv.Value;
            }
            return this;
        }

    }


    public class TDetail : Detailed, IDetail
    {
        public string Name { get { return base.Get<string>("Name"); } set { base.Set("Name",value); } }

        public Detailed D { get { return this; } }

        public Func<object, object> Transformer;

        public TDetail(string name, Func<object, object> transformer = null)
        {
            Name = name;
            Transformer = transformer;
        }

        protected Func<object, T> GetConverter<T>(Func<object, T> conv = null)
        {
            if (Transformer != null)
                if (conv == null)
                    conv = obj => Convert<T>(Transformer(obj));
                else
                    conv = obj => conv(Transformer(obj));
            return conv;
        }

        public T Get<T>(Detailed d, T f=default(T), Func<object, T> conv = null)
        {
            conv = GetConverter<T>(conv); 
            return d.Get(Name, f, conv);
        }
    }

    public struct Detail<T>
    {
        public IDetail D;

        public Detail(IDetail d)
        {
            D = d;
        }

        public Detail(string name)
        {
            D = new TDetail(name,null);
        }

        public Detail(string name, Func<object, object> transform)
        {
            D = new TDetail(name, transform);
        }

        public T Get(Detailed d)
        {
            return D.Get<T>(d);
        }

        public Detail<T> Set(Detailed d, object value)
        {
            d.Set(D.Name, Detailed.Convert<T>(value));
            return this;
        }

        public Detail<T> Set(string d, object v)
        {
            Detailed D1 = D as Detailed;
            D1.Set(d, v);
            return this;
        }

        public Detail<T> Merge(Detailed d, bool overwrite)
        {
            Detailed D1 = D as Detailed;
            D1.Merge(d, overwrite);
            return this;
        }
    }

    public class DetailedNode<K,T> : Detailed, ITreeNode where T : Detailed,ITreeNode
    {
        protected DetailedCollection<K,T> _links;
        
        public Func<T,K> KeyGetter;

        public T Parent
        {
            get;
            set;
        }

        public DetailedCollection<K, T> Children
        {
            get {
                if (_links == null)
                    _links = new DetailedCollection<K,T>(KeyGetter);
                return _links;
            }
            set {
                _links = value;
            }
        }

        public DetailedNode(Func<T, K> keyGetter=null)
        {
            KeyGetter = keyGetter;                    
        }

        object ITreeNode.Parent
        {
            get { return Parent; }
        }
        System.Collections.ICollection ITreeNode.Children
        {
            get { return this.Children; }
        }
    }

    public class DetailedCollection<K,T> : ICollection<T>,IEnumerable<T>,System.Collections.ICollection where T:class
    {
        public Dictionary<K, T> Index = new Dictionary<K, T>();
        public Func<T, K> KeyGetter;

        public DetailedCollection(Func<T, K> keyGetter)
        {
            KeyGetter = keyGetter;
        }

        public T this[K id]
        {
            get {
                T v = null;
                Index.TryGetValue(id, out v);
                return v;
            }
            set { Index[id] = value; }
        }

        public void Add(T item)
        {
            Index.Add(KeyGetter(item), item);
        }

        public void Clear()
        {
            Index.Clear();
        }

        public bool Contains(T item)
        {
            return Index.ContainsKey(KeyGetter(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Index.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Index.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return Index.Remove(KeyGetter(item));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Index.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Index.Values.GetEnumerator();
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            Index.Values.CopyTo((T[])array, index);
        }

        int System.Collections.ICollection.Count
        {
            get { return Index.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return null; }
        }
    }

    
}
