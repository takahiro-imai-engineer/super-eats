using UnityEngine;

namespace app_system
{
    /// <summary>
    /// シングルトン. スレッドセーフ.
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static readonly T _instance = new T();

        protected Singleton()
        {
            Debug.Assert(null == _instance);
        }

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
