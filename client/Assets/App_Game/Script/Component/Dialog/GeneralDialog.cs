using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;
using TMPro;
using app_system;

/// <summary>
/// 汎用ダイアログ
/// </summary>
public class GeneralDialog : MonoBehaviour
{

    //================================================================================
    // インスペクタ
    //================================================================================

    /// <summary>キャプションテキスト</summary>
    [SerializeField]
    protected TextMeshProUGUI captionText;

    /// <summary>メッセージテキスト</summary>
    [SerializeField]
    protected Text messageText;

    /// <summary>メッセージテキスト</summary>
    [SerializeField]
    private TextMeshProUGUI messageTextMesh;

    /// <summary>ボタンコンテンツ</summary>
    [SerializeField]
    protected List<DialogManager.ButtonContent> buttonContents = new List<DialogManager.ButtonContent>();
    public DialogManager.ButtonContent GetButtonContent(DialogManager.ButtonType buttonType) { return buttonContents.FirstOrDefault(content => content.type == buttonType); }

    /// <summary>ダイアログのアニメーター</summary>
    [SerializeField]
    protected Animator dialogAnimator;

    public System.Action enterAnimationCompleteEvent;

    /// <summary>表示順。大きいほど手前に表示される</summary>
    [SerializeField]
    public int displayOrder = 0;

    public DialogManager.ButtonType[] ExitButtonTypes { get; protected set; }

    //================================================================================

    /// <summary>ボタンタイプのサウンド名</summary>
    private Dictionary<DialogManager.ButtonType, string> buttnonSounds = new Dictionary<DialogManager.ButtonType, string>();

    /// <summary>サウンド再生コールバック</summary>
    private UnityAction<string> onPlaySoundListener;

    /// <summary>アニメ名ハッシュ</summary>
	protected static readonly int ANIM_HASH_OPEN = Animator.StringToHash("Open");

    /// <summary>アニメ名ハッシュ</summary>
	protected static readonly int ANIM_HASH_CLOSE = Animator.StringToHash("Close");
    public bool IsOpened { get; private set; }

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// コンストラクタ
    /// </summary>
    protected void Awake()
    {

        // タッチイベントをブロックさせる
        gameObject.AddComponent<GraphicCast>();

        // アンセーフエリア・アンカー調整
        if (gameObject.GetComponent<UnsafeAreaAnchor>() == null)
        {
            gameObject.AddComponent<UnsafeAreaAnchor>().IsSelfOnly = true;
        }
    }

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// キャプション設定
    /// </summary>
    /// <param name="caption">キャプション</param>
    /// <returns>汎用ダイアログ</returns>
    public GeneralDialog SetCaption(string caption)
    {
        if (captionText != null)
        {
            captionText.text = caption;
        }
        return this;
    }

    /// <summary>
    /// メッセージ設定
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <returns>汎用ダイアログ</returns>
    public GeneralDialog SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
        if (messageTextMesh != null)
        {
            messageTextMesh.text = message;
        }
        return this;
    }

    public GeneralDialog SetExitButtonTypes(params DialogManager.ButtonType[] exitButtonTypes)
    {
        ExitButtonTypes = exitButtonTypes;
        return this;
    }

    /// <summary>
    /// 有効なボタンを設定
    /// </summary>
    /// <param name="buttonTypes">有効なボタンタイプ</param>
    /// <returns>汎用ダイアログ</returns>
    public GeneralDialog SetEnabledButton(DialogManager.ButtonType[] buttonTypes)
    {

        // 有効なボタンタイプがボタンコンテンツに無ければ無効にする
        buttonContents.ForEach(content =>
        {
            if (buttonTypes.Contains(content.type) == false)
            {
                content.button.gameObject.SetActive(false);
                content.button = null;
            }
        });
        return this;
    }

    /// <summary>
    /// ボタン押下時のイベント設定
    /// </summary>
    /// <param name="onClickListener">ボタン押下時のイベントリスナー</param>
    /// <returns>汎用ダイアログ</returns>
    public GeneralDialog SetOnClickListener(UnityAction<DialogManager.ButtonType> onClickListener)
    {

        // 各ボタンにクリックイベントを設定
        buttonContents.ForEach(content =>
        {
            if (content.button != null)
            {
                content.button.onClick.RemoveAllListeners();    // 全イベントリスナーを削除しておく
                content.button.onClick.AddListener(delegate
                {
                    if (onClickListener != null)
                    {
                        onClickListener.Invoke(content.type);
                    }

                    // ボタンタイプのサウンド名
                    string soundName;
                    if (buttnonSounds.TryGetValue(content.type, out soundName))
                    {

                        // サウンド再生コールバック
                        if (onPlaySoundListener != null)
                        {
                            onPlaySoundListener.Invoke(soundName);
                        }
                    }
                });
            }
        });
        return this;
    }

    /// <summary>
    /// ボタンのテキストを設定
    /// </summary>
    /// <param name="buttonType">ボタンタイプ</param>
    /// <param name="text">テキスト</param>
    /// <returns>汎用ダイアログ</returns>
    public GeneralDialog SetButtonText(DialogManager.ButtonType buttonType, string text)
    {

        // 指定されたボタンタイプのボタンのテキストを設定
        foreach (var content in buttonContents)
        {
            if (content.type == buttonType)
            {
                var buttonText = content.button.GetComponentInChildren<Text>(false);
                if (buttonText != null)
                {
                    buttonText.text = text;
                }
                else
                {
                    var buttonTextMeshPro = content.button.GetComponentInChildren<TextMeshProUGUI>(false);
                    if (buttonTextMeshPro != null)
                    {
                        buttonTextMeshPro.text = text;
                    }
                }
                break;
            }
        }
        return this;
    }

    /// <summary>
    /// ボタンタイプのサウンド名を設定
    /// </summary>
    /// <param name="buttonType">ボタンタイプ</param>
    /// <param name="soundName">サウンド名</param>
	/// <returns>汎用ダイアログ</returns>
    public GeneralDialog SetButtonSound(DialogManager.ButtonType buttonType, string soundName)
    {

        // 指定されたボタンタイプのサウンド名を設定
        buttnonSounds[buttonType] = soundName;

        return this;
    }

    /// <summary>
    /// サウンド再生コールバック設定
    /// </summary>
    /// <param name="onPlaySoundListener">サウンド再生コールバック</param>
    public void SetOnPlaySoundListener(UnityAction<string> onPlaySoundListener)
    {
        this.onPlaySoundListener = onPlaySoundListener;
    }

    /// <summary>
    /// 消去
    /// </summary>
    public void Dismiss()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 表示順の設定（大きいほど手前に表示される）
    /// </summary>
    /// <param name="order">表示順</param>
    public void SetDisplayOrder(int order)
    {
        displayOrder = order;
    }

    /// <summary>
    /// 表示順（大きいほど手前に表示される）
    /// </summary>
    public int DisplayOrder
    {
        get { return displayOrder; }
    }

    //--------------------------------------------------------------------------------
    // アニメーション
    //--------------------------------------------------------------------------------

    /// <summary>
    /// Enter アニメ
    /// </summary>
    public virtual IEnumerator EnterAnimation()
    {

        IsOpened = false;
        if (dialogAnimator != null)
        {
            // オープンアニメ開始
            dialogAnimator.Play(ANIM_HASH_OPEN, 0, 0);

            // ボタン無効
            var group = GetComponentInParent<CanvasGroup>();
            //group.interactable = false;
            group.blocksRaycasts = false;

            // アニメーション待ち
            yield return null;
            yield return null;
            yield return new WaitForAnimation(dialogAnimator, 0);

            if (enterAnimationCompleteEvent != null) { enterAnimationCompleteEvent.Invoke(); }

            // ボタン有効に戻す
            //group.interactable = true;
            group.blocksRaycasts = true;
            IsOpened = true;
        }
        else
        {
            if (enterAnimationCompleteEvent != null) { enterAnimationCompleteEvent.Invoke(); }
            IsOpened = true;
        }

    }

    /// <summary>
    /// Exit アニメ
    /// </summary>
    /// <param name="onCompleteListener">終了コールバック</param>
    public virtual IEnumerator ExitAnimation(UnityAction onCompleteListener)
    {
        if (dialogAnimator != null)
        {
            // クローズアニメ開始
            dialogAnimator.Play(ANIM_HASH_CLOSE, 0, 0);

            // ボタン無効
            var group = GetComponentInParent<CanvasGroup>();
            //group.interactable = false;
            group.blocksRaycasts = false;

            // アニメーション待ち
            yield return null;
            yield return null;
            yield return new WaitForAnimation(dialogAnimator, 0);

            // ボタン有効に戻す
            //group.interactable = true;
            group.blocksRaycasts = true;

            IsOpened = false;
            // 終了コールバック
            onCompleteListener.Invoke();
        }
        else
        {
            IsOpened = false;
            // 終了コールバック
            onCompleteListener.Invoke();
        }


    }

    // [SerializeField]
    // private StaticBluredScreen blurFilter = null;
    public void UpdateTex()
    {
        // if (null == blurFilter)
        //     return;

        // blurFilter.Capture();
    }

    public void ReleaseTex()
    {
        // if (null == blurFilter)
        //     return;

        // blurFilter.Release();

    }

}
