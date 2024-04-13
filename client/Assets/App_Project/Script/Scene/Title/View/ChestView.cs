using template;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ChestView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private Transform chestTransform;
    [SerializeField] private Animator chestAnimator;

    //================================================================================
    // 定数
    //================================================================================
    private static readonly string CHEST_OPEN_TRIGGER_NAME = "OpenTrigger";
    private static readonly string CHEST_CLOSE_TRIGGER_NAME = "CloseTrigger";

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="onCompleteCallback"></param>
    public void Show(UnityAction onCompleteCallback)
    {
        Sequence sequence = DOTween.Sequence()
                .OnStart(() =>
                {
                    root.SetActive(true);
                });
        // 表示
        sequence
            .Append(
                chestTransform.DOLocalMoveY(0f, 0.8f)
            ).AppendCallback(() =>
            {
                chestAnimator.SetTrigger(CHEST_OPEN_TRIGGER_NAME);
                SoundManager.Instance.Play<SEContainer>(SEName.SE_GET_DIAMOND);
            })
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                onCompleteCallback.Invoke();
            });
        // 非表示
        const float HIDE_DURATION = 0.3f;
        const float GENERATE_COIN_WAIT_DURATION = 0.5f;
        sequence
            .AppendInterval(GENERATE_COIN_WAIT_DURATION)
            .Append(
                chestTransform.DOScale(Vector3.zero, HIDE_DURATION)
            )
            .Join(
                chestTransform.DOLocalRotate(new Vector3(0, -460f, 0), HIDE_DURATION, RotateMode.FastBeyond360)
            )
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                root.SetActive(false);
            });
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {

    }
}
