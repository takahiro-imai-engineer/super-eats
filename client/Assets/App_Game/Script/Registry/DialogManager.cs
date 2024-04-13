using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using app_system;

/// <summary>
/// ダイアログマネージャ
/// </summary>
public class DialogManager : MonoBehaviourSingleton<DialogManager>, IGameRegistry {

	/// <summary>ボタンタイプ</summary>
	public enum ButtonType {

		/// <summary>YES</summary>
		[Label( "Yes" )]
		YES,
		/// <summary>NO</summary>
		[Label( "No" )]
		NO,
		/// <summary>どちらでもない</summary>
		[Label( "N/A" )]
		NA,
	}

    /// <summary>ボタンデスクリプタ</summary>
    [System.Serializable]
    public class ButtonDesc {

        /// <summary>ボタンタイプ</summary>
        public ButtonType ButtonType;

        /// <summary>ローカルテキスト変換キー</summary>
        public string LocalTextKey;

        /// <summary>ローカルテキスト変換後のラベル</summary>
        [HideInInspector]
        [System.NonSerialized]
        public string LocalTexLabel;

        /// <summary>サウンド名</summary>
        public string SoundName;
    }

	/// <summary>ボタンコンテンツ</summary>
	[System.Serializable]
	public struct ButtonContent {

		/// <summary>ボタンタイプ</summary>
		public ButtonType type;

		/// <summary>ボタン</summary>
		public Button button;
	}

    //================================================================================
    // インスペクタ
    //================================================================================

    /// <summary>ダイアログ用キャンバス</summary>
    [SerializeField]
    private Canvas dialogCanvas;

	/// <summary>ダイアログ用キャンバス 取得</summary>
	public Canvas DialogCanvas{ get { return dialogCanvas; }}

    //================================================================================

    /// <summary>ダイアログオブジェクトリスト</summary>
    private Dictionary<System.Type, GeneralDialog> dialogObjects = new Dictionary<System.Type, GeneralDialog>();

    /// <summary>デフォルトのサウンド再生コールバック</summary>
    private UnityAction<string> defaultOnPlaySoundListener;

    //================================================================================
    // ゲームセッティング
    //================================================================================

    /// <summary>ダイアログコンテナのリスト</summary>
    private List<DialogContainer> dialogContainers = new List<DialogContainer>();

    /// <summary>デフォルトのボタンデスクリプタのリスト</summary>
    private Dictionary<ButtonType, ButtonDesc> defaultButtonDescs = new Dictionary<ButtonType, ButtonDesc>();

	public List<ButtonType> ForceButtonTypes = new List<ButtonType>();
    public bool IsDisplayChangeButton;


    /// <summary>
    /// ゲームセッティングのセットアップ
    /// </summary>
    /// <param name="gameSetting">ゲームセッティング</param>
    public void SetupGameSetting( GameSetting gameSetting ) {

        // 常駐ダイアログアセットをダイアログコンテナのリストへ追加
        if( gameSetting.ResidentialDialogAsset != null ) {
            dialogContainers.Add( gameSetting.ResidentialDialogAsset );
        }

        // デフォルトのボタンデスクリプタのリストを設定
        foreach( var buttonDesc in gameSetting.DefaultButtonDescList ) {
            defaultButtonDescs[buttonDesc.ButtonType] = buttonDesc;
        }
    }

    /// <summary>
    /// シャットダウン
    /// 
    /// タイトルへ戻る等、全リセットを行う場合にコールされる
    /// </summary>
    public void Shutdown() {

    }

    //================================================================================
    // エントリ
    //================================================================================

    /// <summary>
    /// 開始
    /// </summary>
    private void Start() {

        // デフォルトのボタンデスクリプタのローカルテキストを変換
        // foreach( var buttonDesc in defaultButtonDescs.Values ) {
        //     buttonDesc.LocalTexLabel = LocalTextManager.Instance.ToText( buttonDesc.LocalTextKey );
        // }
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// ダイアログキャンバスにつなげる
    /// </summary>
    /// <param name="isChangeLayer">レイヤーを変更するかどうか</param>
    public void HookDialogCanvas( GameObject dialog, bool isChangeLayer = true ) {

        // レイヤー変更
        if( isChangeLayer ) {
            GameObjectHelper.SetLayerRecursively( dialog, dialogCanvas.gameObject.layer );
        }

        // ダイアログキャンバスにつなげる
        dialog.transform.SetParent( dialogCanvas.transform, false );
    }

    /// <summary>
    /// デフォルトのサウンド再生コールバック設定
    /// </summary>
    /// <param name="soundName">サウンド再生コールバック</param>
    public void SetDefaultOnPlaySoundListener( UnityAction<string> onPlaySoundListener ) {
        defaultOnPlaySoundListener = onPlaySoundListener;
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <typeparam name="T">ダイアログのタイプ</typeparam>
    /// <param name="caption">キャプション</param>
    /// <param name="message">メッセージ</param>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="buttonTypes">ボタンタイプ</param>
    /// <returns>汎用ダイアログ</returns>
    public T Show<T>( string caption, string message, UnityAction<ButtonType> onCompleteListener, params ButtonType[] buttonTypes ) where T : GeneralDialog {

        // ダイアログコンテナから対象のダイアログを取得
        T dialogPrefab = null;
        foreach( var dialogContainer in dialogContainers ) {
            dialogPrefab = ( T )dialogContainer.Prefabs.FirstOrDefault( prefab => ( prefab != null && prefab.GetType() == typeof( T ) ) );
            if( dialogPrefab != null ) {
                break;
            }
        }
        if( dialogPrefab == null ) {
            return null;
        }

        // 表示（プレハブ指定）
		return Show<T>( dialogPrefab, caption, message, onCompleteListener, buttonTypes );
    }

    /// <summary>
    /// 表示（プレハブ指定）
    /// </summary>
    /// <typeparam name="T">ダイアログのタイプ</typeparam>
    /// <param name="dialogPrefab">ダイアログのプレハブ</param>
    /// <param name="caption">キャプション</param>
    /// <param name="message">メッセージ</param>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="buttonTypes">ボタンタイプ</param>
    /// <returns>汎用ダイアログ</returns>
	public T Show<T>( T dialogPrefab, string caption, string message, UnityAction<ButtonType> onCompleteListener, params ButtonType[] buttonTypes ) where T : GeneralDialog {
        return Show<T>( dialogPrefab, caption, message, onCompleteListener, null, buttonTypes );
    }

    /// <summary>
    /// 表示（プレハブ指定）
    /// </summary>
    /// <typeparam name="T">ダイアログのタイプ</typeparam>
    /// <param name="dialogPrefab">ダイアログのプレハブ</param>
    /// <param name="caption">キャプション</param>
    /// <param name="message">メッセージ</param>
    /// <param name="onCompleteListener">終了コールバック</param>
    /// <param name="enterAnimationCompleteEvent"></param>
    /// <param name="buttonTypes">ボタンタイプ</param>
    /// <returns>汎用ダイアログ</returns>
    public T Show<T>( T dialogPrefab, string caption, string message, UnityAction<ButtonType> onCompleteListener, System.Action enterAnimationCompleteEvent = null, params ButtonType[] buttonTypes ) where T : GeneralDialog {

        // 同じダイアログオブジェクトは使えない
        if( dialogObjects.ContainsKey( typeof( T ) ) ) {
            return null;
        }

        // キャンバスグループのアルファ値は初期値
        dialogCanvas.GetComponent<CanvasGroup>().alpha = 1.0f;

        // 作成
        var dialog = Instantiate( dialogPrefab );
        dialog.transform.SetParent( dialogCanvas.transform, false );

        // 表示順でソート
        SortInDisplayOrder();

        // キャプション、メッセージ設定
        dialog.SetCaption( caption ).SetMessage( message );

        // 有効なボタンを設定
        if (ForceButtonTypes.Any()) { buttonTypes = ForceButtonTypes.ToArray(); }
        dialog.SetEnabledButton( buttonTypes );
        // ExitAnimationを実行するボタンを設定
        dialog.SetExitButtonTypes(buttonTypes);

        // ボタン押下時のイベント設定
        dialog.SetOnClickListener( delegate ( ButtonType type ) {

            // Exitアニメあり
            if(dialog.ExitButtonTypes.Any(buttonType => buttonType == type))
            {
                // ダイアログオブジェクトリストから削除
                dialogObjects.Remove(dialog.GetType());
                // Exitアニメ開始
                StartCoroutine(dialog.ExitAnimation(()=>{
                    // 終了コールバック
                    if (onCompleteListener != null) { onCompleteListener.Invoke(type); }
                    // 強制ボタン設定クリア
                    ForceButtonTypes.Clear();
                    // 消去
                    dialog.Dismiss();
                }));
            }
            // Exitアニメなし
            else
            {
                // 終了コールバック
                if (onCompleteListener != null) { onCompleteListener.Invoke(type); }
                // 強制ボタン設定クリア
                ForceButtonTypes.Clear();
                // INFO : 消去しない
            }
        } );

        // ボタンのテキストを設定
        foreach( var type in buttonTypes ) {

            // ゲームセッティングで、デフォルトのボタンデスクリプタが指定されていればそちらを使う
            ButtonDesc buttonDesc = null;
            string label = null;
            string soundName = null;
            if( defaultButtonDescs.TryGetValue( type, out buttonDesc ) ) {
                //label = LocalTextManager.Instance.ToText( buttonDesc.LocalTextKey );
                label = buttonDesc.LocalTexLabel;
                soundName = buttonDesc.SoundName;
            }

            // ボタンのテキストを設定
            dialog.SetButtonText( type, string.IsNullOrEmpty( label ) ? type.GetLabel() : label );

            // ボタンタイプのサウンド名を設定
            if( string.IsNullOrEmpty( soundName ) == false ) {
                dialog.SetButtonSound( type, soundName );
            }
        }

        // デフォルトのサウンド再生コールバック
        if( defaultOnPlaySoundListener != null ) {
            dialog.SetOnPlaySoundListener( defaultOnPlaySoundListener );
        }

        // ダイアログオブジェクトリストへ登録
        dialogObjects.Add( typeof( T ), dialog );
        dialog.enterAnimationCompleteEvent = enterAnimationCompleteEvent;

        // Enter アニメ開始
        StartCoroutine( dialog.EnterAnimation() );

        return dialog;
    }

    /// <summary>
    /// 消去
    /// </summary>
    /// <typeparam name="T">ダイアログのタイプ</typeparam>
    /// <param name="dialog">ダイアログ</param>
    /// <param name="onCompleteListener">終了コールバック</param>
    public void Dismiss<T>( T dialog, UnityAction onCompleteListener ) where T : GeneralDialog {

        if (dialog == null) {
            // 終了コールバック
            if( onCompleteListener != null ) {
                onCompleteListener.Invoke();
            }
            return;
        }

        // ダイアログオブジェクトリストから削除
        dialogObjects.Remove( dialog.GetType() );

        // Exit アニメ開始
        StartCoroutine( dialog.ExitAnimation( delegate {

            // 終了コールバック
            if( onCompleteListener != null ) {
                onCompleteListener.Invoke();
            }

            // 消去
            dialog.Dismiss();
        } ) );
    }

    /// <summary>
    /// 取得
    /// </summary>
    /// <typeparam name="T">タイプ</typeparam>
    /// <returns>ダイアログ</returns>
    public T Get<T>() where T : GeneralDialog {
        GeneralDialog dialog = null;
        dialogObjects.TryGetValue( typeof( T ), out dialog );
        return ( T )dialog;
    }

    /// <summary>
    /// ダイアログコンテナ追加
    /// 
    /// 非常駐のダイアログコンテナの追加・削除を行う場合に使用
    /// </summary>
    /// <param name="dialogContainer">ダイアログコンテナ</param>
    /// <returns>true なら成功</returns>
    public bool Add( DialogContainer dialogContainer ) {

        // 既存なら追加しない
        if( dialogContainers.Count( container => container.ContainerName == dialogContainer.ContainerName ) > 0 ) {
            return false;
        }

        // ダイアログコンテナのリストへ追加
        dialogContainers.Add( dialogContainer );

        return true;
    }

    /// <summary>
    /// ダイアログコンテナ削除
    /// 
    /// DialogManager.Add() で追加したダイアログコンテナを削除する
    /// </summary>
    /// <param name="containerName">コンテナ名（DialogContainer.ContainerName）</param>
    public void Remove( string containerName ) {

        // ダイアログコンテナのリストから削除
        //dialogContainers.RemoveAll( container => container.ContainerName == containerName );
        var dialogContainer = dialogContainers.FirstOrDefault( container => container.ContainerName == containerName );
        if( dialogContainer != null ) {
            dialogContainers.Remove( dialogContainer );

            // コンテナ自体も解放
            Resources.UnloadAsset( dialogContainer );
        }
    }

    /// <summary>
    /// 表示順でソート
    /// </summary>
    public void SortInDisplayOrder() {

        // ダイアログ用キャンバスにいるダイアログ
        var dialogs = dialogCanvas.GetComponentsInChildren<GeneralDialog>();

        // ダイアログ用キャンバスにいるオーバーレイ
        var overlays = dialogCanvas.GetComponentsInChildren<OverlayObject>();

        // Transform と表示順のセットを作る
        var overlayTranses = new Dictionary<Transform, int>();
        foreach( var dialog in dialogs ) {
            overlayTranses[dialog.transform] = dialog.DisplayOrder;
        }
        foreach( var overlay in overlays ) {
            overlayTranses[overlay.transform] = overlay.DisplayOrder;
        }

        // 表示順の大きい方から、兄弟の先頭に入れていく（つまり表示順が最大のものが一番最後になる）
        var sortedOverlayTranses = overlayTranses.OrderByDescending( overlayTrans => overlayTrans.Value );
        foreach( var overlayTrans in sortedOverlayTranses ) {

            // ダイアログ用キャンバスにいるもののみ
            if( overlayTrans.Key.parent == dialogCanvas.transform ) {
                overlayTrans.Key.SetAsFirstSibling();
            }
        }
    }
}
