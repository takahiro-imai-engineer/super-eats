using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using app_system;

/// <summary>
/// ゲーム変数
/// 
/// UserVariant を継承したユーザー定義クラスを作成する
/// シーン間での受け渡し等に使用できる
/// ストレージ保存はされないので、あくまでテンポラリとして使用する
/// </summary>
public class GameVariant : MonoBehaviourSingleton<GameVariant>, IGameRegistry {

    /// <summary>ユーザー定義のゲーム変数のリスト</summary>
    private Dictionary<System.Type, UserVariant> userVariants = new Dictionary<System.Type, UserVariant>();

    //================================================================================
    // ゲームセッティング
    //================================================================================

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting( GameSetting gameSetting ) {
		
	}

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown() {

        // 全ゲーム変数を削除
        userVariants.Clear();
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// 取得
    /// </summary>
    public T Get<T>() where T : UserVariant, new() {

		// タイプ
        System.Type type = typeof( T );

		// リストから取得
		UserVariant userVariant;
		if( userVariants.TryGetValue( type, out userVariant ) ) {
			return ( T )userVariant;
		}

		// リストに無ければ作成
		userVariant = new T();

		// リストに追加
        userVariants.Add( type, userVariant );

		return ( T )userVariant;
	}

	/// <summary>
	/// 削除
	/// </summary>
	public void Remove<T>() where T : UserVariant {

		// リストから削除
		userVariants.Remove( typeof( T ) );
	}

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="userVariant">ユーザー定義のゲーム変数</param>
    /// <returns>成功なら true</returns>
    public bool Store<T>( T userVariant ) where T : UserVariant, new() {

        // 無い場合は作成
        Get<T>();

        // タイプ
        System.Type type = typeof( T );

        // リストに設定
        if( userVariants.ContainsKey( type ) ) {
            userVariants[type] = userVariant;
            return true;
        }

        return false;
    }
}

/// <summary>
/// ユーザー定義のゲーム変数
/// </summary>
public abstract class UserVariant {}
