using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Model;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json;

namespace Com.Ctrip.Framework.Apollo
{
    public static class ApolloConfigHelper
    {
        private static Config config = ConfigService.GetAppConfig();
        private static readonly ILog logger = LogManager.GetLogger(typeof(ApolloConfigHelper));
        private static ConcurrentDictionary<String, CacheItem> cache = new ConcurrentDictionary<String, CacheItem>();

        static ApolloConfigHelper()
        {
            config.ConfigChanged += OnChanged;
        }

        public static T GetProperty<T>(string key, ApolloValueTypes ValueType = ApolloValueTypes.NONE)
        {
            CacheItem Item = null;
            if (cache.ContainsKey(key))
            {
                Item = cache[key];
            }
            else
            {
                Item = new CacheItem();
                Item.Modify(config.GetProperty(key, null));
                cache.AddOrUpdate(key, Item, (k, oldValue) => Item);
            }

            return Item.GetValue<T>(ValueType);
        }

        private static void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            changeEvent.ChangedKeys.ToList().ForEach(a =>
            {
                var change = changeEvent.GetChange(a);
  
                if (change.ChangeType == PropertyChangeType.MODIFIED)
                {
                    if (cache.ContainsKey(a))
                    {
                        var c = cache[a];
                        c.Modify(change.NewValue);
                        return;
                    }
                    var Item = new CacheItem();
                    Item.Modify(change.NewValue);
                    cache.AddOrUpdate(a, Item, (k, oldValue) => Item);
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
                        var temp = ValueType == ApolloValueTypes.JSON ? JsonConvert.DeserializeObject<T>(Source) as object : Source as object;
                        if (temp == null)
                        {
                            return default(T);
                        }
                        Interlocked.Exchange<Object>(ref _Value, temp);
                        Modified = false;
                    }
                    catch (Exception e)
                    {
                        _Value = null;
                        logger.Error(e);
                    }
                }
                return _Value == null ? default(T) : (T)Convert.ChangeType(_Value,typeof(T));
            }
        }
    }
}
