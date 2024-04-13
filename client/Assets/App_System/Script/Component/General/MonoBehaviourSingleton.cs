using UnityEngine;

namespace app_system
{
    /// <summary>
    /// シングルトン
    /// </summary>
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        /// <summary>インスタンス</summary>
        private static T instance;

        /// <summary>
        /// インスタンスプロパティ
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogWarning(typeof(T) + " is nothing");
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected virtual void Awake()
        {

            // 既存なら削除
            if (Instance != this)
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// インスタンスの存在確認
        /// </summary>
        /// <returns>true=インスタンスが存在する</returns>
        protected bool existInstance()
        {
            if (this != Instance)
            {
                DestroyImmediate(gameObject);
                return true;
            }

            return false;
        }

        /// <summary>
        /// インスタンス解放
        /// </summary>
        protected void releaseInstance()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
} // app_system