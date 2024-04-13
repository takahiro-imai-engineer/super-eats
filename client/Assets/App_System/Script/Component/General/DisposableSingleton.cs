using UnityEngine;

namespace app_system
{
    /// <summary>
    /// シングルトン. 破棄可能.
    /// </summary>
    public class DisposableSingleton<T> where T : class, new()
    {
        /// <summary> インスタンス </summary>
        private static T _instance;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected DisposableSingleton()
        {
            Debug.Assert(null == _instance);
        }

        /// <summary>
        /// インスタンス存在確認
        /// </summary>
       public static bool IsExist
        {
            get
            {
                return (_instance != null);
            }
        }

        /// <summary>
        /// インスタンス取得
        /// </summary>
        public static T Instance
        {
            get
            {
                if(null == _instance)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }

        /// <summary>
        /// インスタンス破棄
        /// </summary>
        public static void Dispose()
        {
            _instance = null;
        }
    }
}
