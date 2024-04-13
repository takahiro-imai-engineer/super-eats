using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// シャッタースクリーン(画面切り替え)
/// </summary>
public class ShutterScreen : OverlayObject
{

    /// <summary>シャッター画像</summary>
    [SerializeField] private RectTransform shutterTransform;
    [SerializeField] private float moveDuration;

    //================================================================================
    // Mono
    //================================================================================

    /// <summary>
    /// 開始
    /// </summary>
    private void Awake()
    {
        shutterTransform.anchoredPosition = new Vector2(0, Screen.height);
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="onUpShutterCallback"></param>
    /// <param name="onCompleteCallback"></param>
    public void Show(UnityAction onUpShutterCallback = null, UnityAction onCompleteCallback = null)
    {
        Sequence sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                shutterTransform.anchoredPosition = new Vector2(0, Screen.height);
            })
            .Append
            (
                shutterTransform
                    .DOAnchorPosY(0, moveDuration / 2)
            )
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                onUpShutterCallback?.Invoke();
            })
            .Append
            (
                shutterTransform
                    .DOAnchorPosY(Screen.height, moveDuration / 2)
            )
            .OnComplete(() =>
            {
                onCompleteCallback?.Invoke();
                OverlayGroup.Instance.Dismiss<ShutterScreen>();
            });
    }
}
