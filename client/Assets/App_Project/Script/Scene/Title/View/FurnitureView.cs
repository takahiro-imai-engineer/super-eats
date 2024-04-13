using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class FurnitureView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Transform furnitureRoot;
    [Header("登場開始位置")]
    [SerializeField] private Vector3 entryPosition;
    [SerializeField] private Transform effectRoot;

    //================================================================================
    // ローカル変数
    //================================================================================
    private Vector3 initScale;
    private readonly string EFFECT_PATH = "Effect/SpawnEffect";

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        furnitureRoot.gameObject.SetActive(false);
        initScale = furnitureRoot.localScale;
        effectRoot.gameObject.SetActive(false);
        var effect = Resources.Load<GameObject>(EFFECT_PATH);
        Instantiate(effect, effectRoot);
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="isPurchase"></param>
    public void Show(bool isPurchase)
    {
        if (isPurchase.IsFalse())
        {
            furnitureRoot.gameObject.SetActive(true);
            furnitureRoot.localPosition = Vector3.zero;
            return;
        }
        furnitureRoot.gameObject.SetActive(true);
        effectRoot.gameObject.SetActive(true);
        const float SCALE_ANIMATION_TIME = 0.3f;
        furnitureRoot.localScale = Vector3.one * 0.01f;
        furnitureRoot
            .DOScale(initScale, SCALE_ANIMATION_TIME)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
        furnitureRoot
            .DOLocalMove(Vector3.zero, 0.8f)
            .SetEase(Ease.OutBounce)
            .SetDelay(0.2f)
            .SetLink(gameObject);

    }
}
