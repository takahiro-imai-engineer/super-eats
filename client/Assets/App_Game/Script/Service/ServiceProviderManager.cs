using System;
using System.Collections.Generic;

namespace app_system
{
    /// <summary>
    /// サービスプロバイダ 管理クラス
    /// </summary>
    /// INFO : 生成済みのサービスプロバイダインスタンスが管理されています.
    /// INFO : 生成済みのサービスプロバイダインスタンスの一括破棄が可能です.
    public class ServiceProviderManager
    {
        /// <summary>
        /// ディスポーザのテーブル
        /// </summary>
        private static Dictionary<Type, IDisposable> Disposers = new Dictionary<Type, IDisposable>();

        /// <summary>
        /// ディスポーザの数を取得
        /// </summary>
        public static int Count { get { return Disposers.Count; } }

        /// <summary>
        /// ディスポーザがテーブルに存在するかどうか
        /// </summary>
        public static bool IsExist(Type type)
        {
            return Disposers.ContainsKey(type);
        }

        /// <summary>
        /// ディスポーザをテーブルに追加
        /// </summary>
        public static void Add(Type type, IDisposable disposer)
        {
            Disposers.Add(type, disposer);
        }

        /// <summary>
        /// ディスポーザをテーブルから削除
        /// </summary>
        public static void Remove(Type type)
        {
            Disposers.Remove(type);
        }

        /// <summary>
        /// テーブルにあるディスポーザの破棄処理を全て実行
        /// </summary>
        public static void DisposeAll()
        {
            new List<Type>(Disposers.Keys).ForEach(type => Disposers[type].Dispose());
        }
    }
}
