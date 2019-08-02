using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigHelper
    {
        private readonly IConfig config;
        private static readonly Action<LogLevel, string, Exception> logger = LogManager.LogFactory(typeof(ApolloConfigHelper).FullName);
        private readonly ConcurrentDictionary<String, CacheItem> cache = new ConcurrentDictionary<String, CacheItem>();

        private ApolloConfigHelper(){}

        public ApolloConfigHelper(IConfig config)
        {
            this.config = config;
            this.config.ConfigChanged += OnChanged;
        }


        public T GetProperty<T>(string key, ApolloValueTypes ValueType = ApolloValueTypes.NONE)
        {
            CacheItem Item = null;
            if (cache.ContainsKey(key))
            {
                Item = cache[key];
            }
            else
            {
                Item = new CacheItem();
                string newValue;
                config.TryGetProperty(key, out newValue);
                Item.Modify(newValue);
                cache.AddOrUpdate(key, Item, (k, oldValue) => Item);
            }

            return Item.GetValue<T>(ValueType);
        }

        private void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            changeEvent.ChangedKeys.ToList().ForEach(a =>
            {
                var change = changeEvent.GetChange(a);
                switch (change.ChangeType)
                {
                    case PropertyChangeType.Added:
                    case PropertyChangeType.Modified:
                        if (cache.ContainsKey(a))
                        {
                            var c = cache[a];
                            c.Modify(change.NewValue);
                            return;
                        }
                        var Item = new CacheItem();
                        Item.Modify(change.NewValue);
                        cache.AddOrUpdate(a, Item, (k, oldValue) => Item);
                        return;
                    case PropertyChangeType.Deleted:
                        CacheItem RemoveItem;
                        if (cache.ContainsKey(a))
                        {
                            cache.TryRemove(a, out RemoveItem);
                        }
                        return;
                }
            });
        }

        class CacheItem
        {
            private String Source;
            private Object _Value = new object();
            public Boolean Modified { get; set; } = true;

            public CacheItem()
            {

            }

            public void Modify(String Src)
            {
                Source = Src;
                Modified = true;
            }

            public T GetValue<T>(ApolloValueTypes ValueType)
            {
                if (Modified)
                {
                    try
                    {
                        if (Source != null)
                        {
                            var temp = ValueType == ApolloValueTypes.JSON ? JsonConvert.DeserializeObject<T>(Source) as object : Source as object;
                            if (temp == null)
                            {
                                return default(T);
                            }
                            Interlocked.Exchange<Object>(ref _Value, temp);
                        }
                        else
                        {
                            _Value = null;
                        }

                        Modified = false;
                    }
                    catch (Exception e)
                    {
                        _Value = null;
                        logger(LogLevel.Error,null,e);
                    }
                }
                return _Value == null ? default(T) : (T)Convert.ChangeType(_Value,typeof(T));
            }
        }
    }
}
