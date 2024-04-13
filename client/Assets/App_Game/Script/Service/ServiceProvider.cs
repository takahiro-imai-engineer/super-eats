using System.Collections.Generic;
using UnityEngine;

namespace app_system
{
    /// <summary>
    /// サービスプロバイダ 基底クラス
    /// INFO : インスタンス取得時、インスタンスが存在しない場合、自動生成します.
    /// INFO : インスタンスの破棄が可能です.
    /// INFO : サービスプロバイダマネージャーで、生成済みのインスタンスが管理されています.
    /// INFO : サービスプロバイダマネージャーから、生成済みのインスタンスの一括破棄が可能です.
    /// </summary>
    public class ServiceProvider<T> : System.IDisposable where T : class, new()
    {
        /// <summary>
        /// インスタンス取得
        /// </summary>
        /// INFO : インスタンスが存在しない場合、自動生成します.
        public static T Instance
        {
            get
            {
                // インスタンス存在確認
                if (app_system.DisposableSingleton<T>.IsExist)
                {
                    //Debug.Log(string.Format("■ {0} : {1} : 取得（すでに生成済み）", typeof(T), ServiceManager.Count));
                    return app_system.DisposableSingleton<T>.Instance;
                }
                // インスタンス取得
                var instance = app_system.DisposableSingleton<T>.Instance;
                // サービス登録確認
                var type = typeof(T);
                if (ServiceProviderManager.IsExist(type))
                {
                    //Debug.LogError(string.Format("■ {0} : {1} : サービス登録済", typeof(T), ServiceManager.Count));
                    return instance;
                }
                // サービス登録
                ServiceProviderManager.Add(type, instance as System.IDisposable);
                //Debug.Log(string.Format("■ {0} : {1} : 生成", typeof(T), ServiceManager.Count));
                return instance;
            }
        }

        /// <summary>
        /// インスタンス破棄
        /// </summary>
        public void Dispose()
        {
            // インスタンス存在確認
            if (!app_system.DisposableSingleton<T>.IsExist)
            {
                //Debug.Log(string.Format("■ {0} : {1} : 破棄（すでに破棄済み）", typeof(T), ServiceManager.Count));
                return;
            }
            // サービス登録解除
            ServiceProviderManager.Remove(typeof(T));
            // インスタンス破棄
            app_system.DisposableSingleton<T>.Dispose();
            //Debug.Log(string.Format("■ {0} : {1} : 破棄", typeof(T), ServiceManager.Count));
        }
    }
}

