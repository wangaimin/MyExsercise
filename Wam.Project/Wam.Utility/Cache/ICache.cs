using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wam.Utility.Cache
{

    /// <summary>
    /// 缓存接口
    /// </summary>
    interface ICache
    {
        object GetWithCache(string cacheKey,Func<object> getter,int cacheTimeSecond);


        T GetWithCache<T>(string cacheKey,Func<T> getter,int cacheTimeSecond)
            where T :class;

        void Remove(string cacheKey);

        void FlushAll();

        IEnumerable<KeyValuePair<string, object>> GetList();

    }
}
