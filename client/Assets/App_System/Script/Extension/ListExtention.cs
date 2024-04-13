using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app_system
{
    public static class ListExtention
    {
        /// <summary>
        /// 指定されたインデックスに存在する要素を返します.
        /// 存在しない場合はnullを返します.
        /// </summary>
        public static T Get<T>(this IList<T> self, int index) where T : class
        {
            return (index < self.Count) ? self[index] : null;
        }
    }
}
