using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.IO;

namespace Wam.Utility.Cache
{
  public   class LocalMemoryCache:ICache
    {
        string CACHE_LOCKER_PREFIX = "CM_DL_";

        public object GetWithCache(string cacheKey, Func<object> getter, int cacheTimeSecond) {


           object data= MemoryCache.Default.Get(cacheKey);
            if (data!=null)
            {
                return data;
            }
            string locker = CACHE_LOCKER_PREFIX + cacheKey;
            lock (string.Intern(locker))
            {

                data = MemoryCache.Default.Get(cacheKey);
                if (data!=null)
                {
                    return data;
                }

                data = getter();
                CacheItemPolicy cip = new CacheItemPolicy();
                cip.SlidingExpiration =TimeSpan.FromSeconds( cacheTimeSecond);
                MemoryCache.Default.Set(cacheKey, data, cip);

            }

            return data;
        }


      public  T GetWithCache<T>(string cacheKey, Func<T> getter, int cacheTimeSecond)
            where T : class
        {
            var data = MemoryCache.Default.Get(cacheKey) as T;
            if (data != null)
            {
                return data;
            }
            string locker = CACHE_LOCKER_PREFIX + cacheKey;
            lock (string.Intern(locker))
            {

                data = MemoryCache.Default.Get(cacheKey) as T;
                if (data != null)
                {
                    return data;
                }

                data = getter();
                string filePath = @"C:\Users\admin\Source\Repos\NewRepo\Wam.Project\UnitTestProject2\Configuration\Data\Shop.config";
             

                List<string> filePaths = new List<string>() { filePath };
             //   HostFileChangeMonitor hfcm = new HostFileChangeMonitor(filePaths);
                

                //hfcm.NotifyOnChanged(new OnChangedCallback((o) =>
                //{
                //    MemoryCache.Default.Remove(cacheKey);
                //}));


                CacheItemPolicy cip = new CacheItemPolicy();
                cip.SlidingExpiration = TimeSpan.FromSeconds(cacheTimeSecond);
               // cip.ChangeMonitors.Add(hfcm);
                MemoryCache.Default.Set(cacheKey, data, cip);

            }

            return data;
        }

        public void Remove(string cacheKey) {


        }

        public void FlushAll() {


        }

      public IEnumerable<KeyValuePair<string, object>> GetList() {
           

            return new List< KeyValuePair<string,object>>();
        }
    }
}
