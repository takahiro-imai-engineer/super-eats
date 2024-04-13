using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app_system
{
    public static class DictionaryExtention
    {
        /// <summary>
        /// 指定されたキーに存在する要素を返します.
        /// 存在しない場合はnullを返します.
        /// </summary>
        public static Value Get<Value>(this IDictionary<int, Value> self, int key) where Value : class
        {
            return self.ContainsKey(key) ? self[key] : null;
        }
    }
}