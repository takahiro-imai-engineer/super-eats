using DG.Tweening;
using UnityEngine;

/// <summary>
/// ライバルの描画クラス
/// </summary>
public class RivalView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("ルート")]
    /// <summary>ルート</summary>
    [SerializeField] private GameObject root;
    [Header("ライバル配達員のタイプ")]
    /// <summary>RivalType</summary>
    [SerializeField] private InGameConstant.RivalType rivalType = InGameConstant.RivalType.FoodDelivery;
    /// <summary>人間アニメーター</summary>
    [SerializeField] private Animator humanAnimator;
    /// <summary>バイクアニメーター</summary>
    [SerializeField] private Animator bikeAnimator;
    /// <summary>食べ物のルート</summary>
    [SerializeField] private GameObject foodRoot;
    [Header("奪える食べ物のデータ")]
    /// <summary>食べ物データ</summary>
    [SerializeField] private FoodData foodData;
    [Header("ライバルのアイコン")]
    /// <summary>アイコン</summary>
    [SerializeField] private RivalIconView rivalIconView;
    //================================================================================
    // プロパティ
    //================================================================================
    /// <summary>配達員タイプ</summary>
    public InGameConstant.RivalType RivalType => rivalType;
    /// <summary>食べ物データ</summary>
    public FoodData FoodData => foodData;
    //================================================================================
    // メソッド
    //================================================================================
    private void Start()
    {
        if (rivalIconView != null)
        {
            var rivalData = StageManager.Instance.GetRandomRivalData();
            rivalIconView.Init(rivalData);
        }
    }

    /// <summary>
    /// クラッシュ
    /// </summary>
    public void Clash()
    {
        if (foodRoot)
        {
            foodRoot.SetActive(false);
        }
        if (rivalIconView != null)
        {
            rivalIconView.gameObject.SetActive(false);
        }
        var colliders = this.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        if (humanAnimator != null)
        {
            humanAnimator.SetBool("isClash", true);
        }
        if (bikeAnimator != null)
        {
            bikeAnimator.SetBool("isClash", true);
        }

        var movingGimick = this.GetComponentInParent<MovingGimick>();
        if (movingGimick != null)
        {
            DOVirtual.DelayedCall(
                1f,
                () =>
                {
                    movingGimick.Stop();
                }
            );
        }
    }
}