using DG.Tweening;
using UnityEngine;

/// <summary>
/// プレイヤーのエフェクトを管理するクラス
/// </summary>
public class PlayerEffectView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("ダッシュエフェクト")]
    /// <summary>ダッシュのエフェクト</summary>
    [SerializeField] private GameObject dashEffect;
    [Header("食べ物獲得エフェクト")]
    /// <summary>食べ物入手のエフェクト</summary>
    [SerializeField] private GameObject getFoodEffect;
    [Header("滑り開始エフェクト")]
    /// <summary>滑り開始エフェクト</summary>
    [SerializeField] private GameObject startSlipEffect;
    /// <summary>滑りアイコン</summary>
    [SerializeField] private MeshRenderer heavyIcon;
    [Header("重りアイコンの移動終了位置(ローカル座標)")]
    /// <summary>滑りアイコンの移動終了位置</summary>
    [SerializeField] private Vector3 heavyIconMoveEndPosition;
    [Header("汗(滑り時)エフェクト")]
    /// <summary>汗(滑り時)のエフェクト</summary>
    [SerializeField] private GameObject sweatEffect;
    [Header("気絶エフェクト")]
    /// <summary>気絶のエフェクト</summary>
    [SerializeField] private GameObject stunnedEffect;
    [Header("ライバル攻撃エフェクト")]
    /// <summary>ライバル攻撃のエフェクト</summary>
    [SerializeField] private GameObject rivalAttackEffectPrefab;
    /// <summary>右側のライバル攻撃のTransform</summary>
    [SerializeField] private Transform rightRivalAttackTransform;
    /// <summary>左側のライバル攻撃のTransform</summary>
    [SerializeField] private Transform leftRivalAttackTransform;
    [Header("ライバル攻撃エフェクト発生遅延時間")]
    [SerializeField] private float rivalAttackDelayTime = 0.25f;
    [Header("強風エフェクト")]
    /// <summary>強風エフェクト</summary>
    [SerializeField] private GameObject windEffect;
    [Header("自転車スパークエフェクト")]
    /// <summary>強風エフェクト</summary>
    [SerializeField] private GameObject bicycleSparkEffect;
    //================================================================================
    // 定数
    //================================================================================
    static readonly float HEAVY_ICON_FADE_TIME = 3f;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        dashEffect.SetActive(false);
        getFoodEffect.SetActive(false);
        startSlipEffect.SetActive(false);
        sweatEffect.SetActive(false);
        stunnedEffect.SetActive(false);
        windEffect.SetActive(false);
        bicycleSparkEffect.SetActive(false);
    }

    /// <summary>
    /// ダッシュエフェクトを表示
    /// </summary>
    public void ShowDashEffect()
    {
        dashEffect.SetActive(true);
    }

    /// <summary>
    /// ダッシュエフェクトを非表示
    /// </summary>
    public void HideDashEffect()
    {
        dashEffect.SetActive(false);
    }

    /// <summary>
    /// 食べ物入手エフェクトを表示
    /// </summary>
    public void ShowGetFoodEffect()
    {
        getFoodEffect.SetActive(true);
    }

    /// <summary>
    /// 食べ物入手エフェクトを非表示
    /// </summary>
    public void HideGetFoodEffect()
    {
        getFoodEffect.SetActive(false);
    }

    /// <summary>
    /// 滑り開始エフェクトを表示
    /// </summary>
    public void ShowStartSlipEffect()
    {
        startSlipEffect.SetActive(true);
        heavyIcon.transform.localPosition = Vector3.zero;

        var c = heavyIcon.material.color;
        c.a = 1.0f;
        heavyIcon.material.color = c;

        Sequence sequence = DOTween.Sequence()
            .Append(
                DOTween.ToAlpha(
                    () => heavyIcon.material.color,
                    color => heavyIcon.material.color = color,
                    0f,
                    HEAVY_ICON_FADE_TIME
                )
            )
            .Join(
                heavyIcon.transform.DOLocalMoveY(
                    heavyIconMoveEndPosition.y,
                    HEAVY_ICON_FADE_TIME
                )
            )
            .OnComplete(() =>
            {
                startSlipEffect.SetActive(false);
            });
    }

    /// <summary>
    /// 汗エフェクトを表示
    /// </summary>
    public void ShowSweatEffect()
    {
        sweatEffect.SetActive(true);
    }

    /// <summary>
    /// 汗エフェクトを非表示
    /// </summary>
    public void HideSweatEffect()
    {
        sweatEffect.SetActive(false);
    }

    /// <summary>
    /// 気絶エフェクトを表示
    /// </summary>
    public void ShowStunnedEffect()
    {
        stunnedEffect.SetActive(true);
    }

    /// <summary>
    /// 気絶エフェクトを非表示
    /// </summary>
    public void HideStunnedEffect()
    {
        stunnedEffect.SetActive(false);
    }

    /// <summary>
    /// ライバル攻撃エフェクトを生成する
    /// </summary>
    /// <param name="instantiatePosition"></param>
    public void GenerateRivalAttackEffect(bool isRight)
    {
        DOVirtual.DelayedCall(
            rivalAttackDelayTime,
            () =>
            {
                if (isRight)
                {
                    //Instantiate(rivalAttackEffectPrefab, rightRivalAttackTransform.position, Quaternion.identity);
                    Instantiate(rivalAttackEffectPrefab, rightRivalAttackTransform.position, Quaternion.identity, rightRivalAttackTransform);
                }
                else
                {
                    //Instantiate(rivalAttackEffectPrefab, leftRivalAttackTransform.position, Quaternion.identity);
                    Instantiate(rivalAttackEffectPrefab, leftRivalAttackTransform.position, Quaternion.identity, leftRivalAttackTransform);
                }
            }
        );
    }

    /// <summary>
    /// 風エフェクトを表示
    /// </summary>
    public void ShowWindEffect(Vector3 pushVector)
    {
        if (pushVector.x < 0)
        {
            windEffect.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
        else
        {
            windEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        windEffect.SetActive(true);
    }

    /// <summary>
    /// 風エフェクトを非表示
    /// </summary>
    public void HideWindEffect()
    {
        windEffect.SetActive(false);
    }

    public void ShowBicycleEffect()
    {
        bicycleSparkEffect.SetActive(true);
    }

    public void HideBicycleEffect()
    {
        bicycleSparkEffect.SetActive(false);
    }
}