using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using app_system;

/// <summary>
/// オーバーレイグループ
/// </summary>
public class OverlayGroup : MonoBehaviourSingleton<OverlayGroup>, IGameRegistry {

    //================================================================================
    // インスペクタ
    //================================================================================

    /// <summary>オーバーレイ用キャンバス</summary>
    [SerializeField]
    private Canvas overlayCanvas;

    //================================================================================

    /// <summary>オーバーレイオブジェクトリスト</summary>
    private Dictionary<System.Type, OverlayObject> overlayObjects = new Dictionary<System.Type, OverlayObject>();

    /// <summary>ウェイトコルーチンリスト</summary>
    private Dictionary<System.Type, Coroutine> waitCoroutines = new Dictionary<System.Type, Coroutine>();

    /// <summary>シャットダウン時に削除しないオーバーレイオブジェクトの名前リスト</summary>
    private List<string> keepAliveOverlayObjects;

    //================================================================================
    // ゲームセッティング
    //================================================================================

    /// <summary>オーバーレイコンテナのリスト</summary>
    private List<OverlayContainer> overlayContainers = new List<OverlayContainer>();

    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting( GameSetting gameSetting ) {

        // 常駐オーバーレイアセットをオーバーレイコンテナのリストへ追加
        if( gameSetting.ResidentialOverlayAsset != null ) {
            overlayContainers.Add( gameSetting.ResidentialOverlayAsset );
        }

        // シャットダウン時に削除しないオーバーレイオブジェクトの名前リスト
        keepAliveOverlayObjects = gameSetting.KeepAliveOverlayObjects;
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown() {

        // 全コルーチン停止
        StopAllCoroutines();

        // アクティブなオーバーレイオブジェクトリスト
        var overlayObjs = new List<OverlayObject>( overlayObjects.Values );

        // リストクリア
        overlayObjects.Clear();
        waitCoroutines.Clear();

        // 削除
        overlayObjs.ForEach( overlayObj => {

            // シャットダウン時に削除しないオーバーレイオブジェクトは除く
            if( keepAliveOverlayObjects.Contains( overlayObj.GetType().Name ) == false ) {
                Destroy( overlayObj.gameObject );
            }
            else {
                // 残す
                overlayObjects[overlayObj.GetType()] = overlayObj;
            }
        } );
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// 表示
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <param name="prefab">プレハブ</param>
    /// <param name="canvas">オーバーレイ用キャンバスを使用したくない場合に指定する</param>
    /// <returns>オーバーレイオブジェクト</returns>
    public T Show<T>( Canvas canvas = null ) where T : OverlayObject {

        // 表示
        return ( T )Show( typeof( T ), canvas );
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <typeparam name="type">タイプ</typeparam>
    /// <param name="prefab">プレハブ</param>
    /// <param name="canvas">オーバーレイ用キャンバスを使用したくない場合に指定する</param>
    /// <returns>オーバーレイオブジェクト</returns>
    public OverlayObject Show( System.Type type, Canvas canvas = null ) {

        // オーバーレイコンテナから対象のオーバーレイオブジェクトを取得
        OverlayObject overlayPrefab = null;
        foreach( var overlayContainer in overlayContainers ) {
            overlayPrefab = overlayContainer.Prefabs.FirstOrDefault( prefab => ( prefab != null && prefab.GetType() == type ) );
            if( overlayPrefab != null ) {
                break;
            }
        }
        if( overlayPrefab == null ) {
            return null;
        }

        // 表示（プレハブ指定）
        return Show( type, overlayPrefab, canvas );
    }

    /// <summary>
    /// 表示（プレハブ指定）
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <param name="prefab">プレハブ</param>
    /// <param name="canvas">オーバーレイ用キャンバスを使用したくない場合に指定する</param>
    /// <returns>オーバーレイオブジェクト</returns>
    public T Show<T>( T overlayPrefab, Canvas canvas = null ) where T : OverlayObject {

        // 表示（プレハブ指定）
        return ( T )Show( typeof( T ), overlayPrefab, canvas );
    }

    /// <summary>
    /// 表示（プレハブ指定）
    /// </summary>
    /// <typeparam name="type">タイプ</typeparam>
    /// <param name="prefab">プレハブ</param>
    /// <param name="canvas">オーバーレイ用キャンバスを使用したくない場合に指定する</param>
    /// <returns>オーバーレイオブジェクト</returns>
    public OverlayObject Show( System.Type type, OverlayObject overlayPrefab, Canvas canvas = null ) {

        // 同じオーバーレイオブジェクトは使えない
        if( overlayObjects.ContainsKey( type ) ) {

            // ウェイトコルーチンを停止
            if( waitCoroutines.ContainsKey( type ) ) {
                StopCoroutine( waitCoroutines[type] );

                // ウェイトコルーチンリストから削除
                waitCoroutines.Remove( type );
            }
            return null;
        }

        // 作成
        var overlay = Instantiate( overlayPrefab );
        if( canvas == null ) {
            canvas = overlayCanvas;
        }
        overlay.transform.SetParent( canvas.transform, false );

        // オーバーレイオブジェクトリストへ登録
        overlayObjects.Add( type, overlay );

        // 表示順でソート
        sortInDisplayOrder();

        return overlay;
    }

    /// <summary>
    /// 消去
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <param name="delayTime">消去までの待ち時間</param>
    public void Dismiss<T>( float delayTime = 0.0f ) where T : OverlayObject {

        // 消去
        Dismiss( typeof( T ), delayTime );
    }

    /// <summary>
    /// 消去
    /// </summary>
    /// <typeparam name="type">タイプ</typeparam>
    /// <param name="delayTime">消去までの待ち時間</param>
    public void Dismiss( System.Type type, float delayTime = 0.0f ) {

        // 対象のオーバーレイオブジェクト
        var overlay = Get( type );
        if( overlay == null ) {
            return;
        }

        // ウェイトコルーチンを停止
        if( waitCoroutines.ContainsKey( type ) ) {
            StopCoroutine( waitCoroutines[type] );

            // ウェイトコルーチンリストから削除
            waitCoroutines.Remove( type );
        }

        // 待ち時間後に削除
        var coroutine = this.TimeToAction( delayTime, delegate {

            // ウェイトコルーチンリストから削除
            waitCoroutines.Remove( type );

            // オーバーレイオブジェクトリストから削除
            overlayObjects.Remove( type );

            // 削除
            Destroy( overlay.gameObject );
        } );

        // ウェイトコルーチンリストへ登録
        if( coroutine != null && overlayObjects.ContainsKey( type ) ) {
            waitCoroutines[type] = coroutine;
        }
    }

    /// <summary>
    /// 取得
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <returns>オーバーレイオブジェクト</returns>
    public T Get<T>() where T : OverlayObject {

        // 対象のオーバーレイオブジェクト
        OverlayObject overlay;
        if( overlayObjects.TryGetValue( typeof( T ), out overlay ) == false ) {
            return null;
        }

        return ( T )overlay;
    }

    /// <summary>
    /// 取得
    /// </summary>
    /// <typeparam name="type">タイプ</typeparam>
    /// <returns>オーバーレイオブジェクト</returns>
    public OverlayObject Get( System.Type type ) {

        // 対象のオーバーレイオブジェクト
        OverlayObject overlay;
        if( overlayObjects.TryGetValue( type, out overlay ) == false ) {
            return null;
        }

        return overlay;
    }

    /// <summary>
    /// 表示順でソート
    /// </summary>
    public void SortInDisplayOrder() {
        sortInDisplayOrder();
    }

    /// <summary>
    /// オーバーレイコンテナ追加
    /// 
    /// 非常駐のオーバーレイコンテナの追加・削除を行う場合に使用
    /// </summary>
    /// <param name="overlayContainer">オーバーレイコンテナ</param>
    /// <returns>true なら成功</returns>
    public bool Add( OverlayContainer overlayContainer ) {

        // 既存なら追加しない
        if( overlayContainers.Count( container => container.ContainerName == overlayContainer.ContainerName ) > 0 ) {
            return false;
        }

        // オーバーレイコンテナのリストへ追加
        overlayContainers.Add( overlayContainer );

        return true;
    }

    /// <summary>
    /// オーバーレイコンテナ削除
    /// 
    /// OverlayGroup.Add() で追加したオーバーレイコンテナを削除する
    /// </summary>
    /// <param name="containerName">コンテナ名（overlayContainer.ContainerName）</param>
    public void Remove( string containerName ) {

        // オーバーレイコンテナのリストから削除
        //overlayContainers.RemoveAll( container => container.ContainerName == containerName );
        var overlayContainer = overlayContainers.FirstOrDefault( container => container.ContainerName == containerName );
        if( overlayContainer != null ) {
            overlayContainers.Remove( overlayContainer );

            // コンテナ自体も解放
            Resources.UnloadAsset( overlayContainer );
        }
    }

    //================================================================================
    // ローカル
    //================================================================================

    /// <summary>
    /// 表示順でソート
    /// </summary>
    private void sortInDisplayOrder() {

        // 表示順の大きい方から、兄弟の先頭に入れていく（つまり表示順が最大のものが一番最後になる）
        var sortedOverlayObjects = overlayObjects.Values.OrderByDescending( overlay => overlay.DisplayOrder );
        foreach( var overlay in sortedOverlayObjects ) {

            // オーバーレイ用キャンバスにいるもののみ
            if( overlay.transform.parent == overlayCanvas.transform ) {
                overlay.transform.SetAsFirstSibling();
            }
        }
    }
}
