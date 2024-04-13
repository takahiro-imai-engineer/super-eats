using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// プレイヤーの描画クラス
/// </summary>
public class PlayerView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    /// <summary>アニメーター</summary>
    [SerializeField] private Animator playerAnimator;
    /// <summary>バックのルート</summary>
    [SerializeField] private GameObject bagRoot;

    /// <summary>アバター</summary>
    [SerializeField] private AvatarView avatarView;
    /// <summary>自転車</summary>
    [SerializeField] private BicycleView bicycleView;
    /// <summary>バッグ</summary>
    [SerializeField] private BagView bagView;
    /// <summary>エフェクト</summary>
    [SerializeField] private PlayerEffectView playerEffectView;

    //================================================================================
    // ローカル
    //================================================================================
    private Transform playerTransform;
    // ラグドールに使用しているRigidbody
    private Rigidbody[] ragdollRigidbodies;
    /// <summary>傾きの更新時間</summary>
    private float bendUpdateTime = 0f;
    /// <summary>前フレームの傾き</summary>
    protected float prevAngle = 0f;
    /// <summary>前の傾き位置X</summary>
    private Vector3 prevBendPosition;
    /// <summary>クラッシュ前のバッグのサイズ</summary>
    private float prevBagScale = 0f;

    //================================================================================
    // 定数
    //================================================================================
    /// <summary>１フレーム辺りの回転量制限</summary>
    private float LIMIT_FRAME_ROTATE = 90f;
    /// <summary>ライバルから食べ物を奪った時の獲得の遅延</summary>
    private float GET_FOOD_BY_RIVAL_DELAY_TIME = 0.5f;

    //================================================================================
    // getter
    //================================================================================
    public AvatarView AvatarView => avatarView;
    public BicycleView BicycleView => bicycleView;
    public BagView BagView => bagView;
    public PlayerEffectView PlayerEffectView => playerEffectView;
    public float BagScale
    {
        get { return bagRoot.transform.localScale.x; }
    }

    /// <summary>滑り中か</summary>
    public bool IsSlipping
    {
        get;
        private set;
    }
    /// <summary>空回り中か</summary>
    public bool IsSpinWheel
    {
        get;
        private set;
    }
    //================================================================================
    // メソッド
    //================================================================================
    public void Init()
    {
        // isHit = false;
        // rigidbody.isKinematic = false;
        bendUpdateTime = 0f;
        prevBendPosition = Vector3.zero;
        IsSlipping = false;
        IsSpinWheel = false;
        playerTransform = this.transform;
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);

        bagRoot.SetActive(true);
        bagRoot.transform.localScale = Vector3.one;
        prevBagScale = bagRoot.transform.localScale.x;

        if (playerEffectView != null)
        {
            playerEffectView.Init();
        }
    }

    /// <summary>
    /// アバター変更
    /// </summary>
    public void ChangeAvatar(AvatarData avatarData, bool isChangeAnimation = false)
    {
        avatarView.ChangeAvatar(avatarData.avatarId, isChangeAnimation);
    }

    /// <summary>
    /// バッグ変更
    /// </summary>
    public void ChangeBag(BagData bagData, bool isChangeAnimation = false, bool isTitleScene = false)
    {
        bagView.ChangeBag(bagData, isChangeAnimation, isTitleScene);
    }

    /// <summary>
    /// 自転車変更
    /// </summary>
    public void ChangeBicycle(BicycleData bicycleData, bool isChangeAnimation = false)
    {
        bicycleView.ChangeBicycle(bicycleData, isChangeAnimation);
    }

    /// <summary>
    /// 傾き
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <param name="dragSpeed"></param>
    /// <param name="beforeInertiaX"></param>
    /// <param name="limitTilt"></param>
    /// /// <param name="isSlipState"></param>
    public void Bend(Vector3 currentPosition, float dragSpeed, float beforeInertiaX, float limitTilt, bool isSlipState)
    {
        bendUpdateTime += Time.deltaTime;
        float diffPositionX = (currentPosition - prevBendPosition).x;
        float preInertiaX = beforeInertiaX;
        // if (bendUpdateTime <= 0.1f) {
        //     return;
        // }

        // NOTE: 自機の回転
        float rangeAngle = limitTilt; //　傾く角度
        if (diffPositionX == 0f || dragSpeed == 0f)
        {
            // NOTE: 徐々に0度に戻す
            float angle = Mathf.Clamp(0f, prevAngle - LIMIT_FRAME_ROTATE * Time.deltaTime, prevAngle + LIMIT_FRAME_ROTATE * Time.deltaTime);
            angle = Mathf.Clamp(angle, -rangeAngle, rangeAngle);
            playerTransform.localRotation = Quaternion.Slerp(Quaternion.Euler(0f, 0f, angle), playerTransform.localRotation, Time.deltaTime);
            // Debug.Log (prevAngle + " → " + angle);
            prevAngle = angle;
        }
        else
        {
            // NOTE: 移動分だけ時期を回転
            float moveDistance = 0.01f; //　移動範囲距離。この値をもとに傾きの割合を計算。値は感覚値
            float rate = Mathf.Clamp((prevBendPosition - currentPosition).x / moveDistance, -1, 1f);
            rate = Mathf.Clamp(rate * Mathf.Abs(rate) * 2f, -1f, 1f); // カーブを描くように補正
            float angle = rangeAngle * rate;
            angle = Mathf.Clamp(angle, prevAngle - LIMIT_FRAME_ROTATE * Time.deltaTime, prevAngle + LIMIT_FRAME_ROTATE * Time.deltaTime);
            angle = Mathf.Clamp(angle, -rangeAngle, rangeAngle);
            playerTransform.localRotation = Quaternion.Slerp(Quaternion.Euler(0f, 0f, angle), playerTransform.localRotation, Time.deltaTime);
            // Debug.Log (prevAngle + " → " + angle);
            prevAngle = angle;
        }

        // BendMotion(currentPosition.x - prevBendPosition.x);
        prevBendPosition = currentPosition;
        bendUpdateTime = 0f;

        // 空回りモーション
        if (IsSlipping && diffPositionX < 0f && beforeInertiaX > 0)
        {
            // 滑り中。右の空回り
            IsSpinWheel = true;
        }
        else if (IsSlipping && diffPositionX > 0f && beforeInertiaX < 0)
        {
            // 滑り中。左の空回り
            IsSpinWheel = true;
        }
        else if (beforeInertiaX < -0.25f || 0.25f < beforeInertiaX)
        {
            // 滑り左右
            IsSlipping = true;
        }
        else
        {
            // 滑り終了
            IsSlipping = false;
            IsSpinWheel = false;
        }

        // NOTE: 滑り状態時、汗エフェクトの表示
        if (isSlipState)
        {
            playerEffectView.ShowSweatEffect();
        }
        else
        {
            playerEffectView.HideSweatEffect();
        }
    }

    /// <summary>
    /// ダッシュ
    /// </summary>
    public void Dash(float dashTime)
    {
        // StartDashMotion();
        playerEffectView.ShowDashEffect();
        DOVirtual.DelayedCall(
            dashTime,
            () =>
            {
                // StopDashMotion();
                playerEffectView.HideDashEffect();
            }
        );
    }

    /// <summary>
    /// 食べ物取得
    /// </summary>
    public void GetFood()
    {
        float currentScale = bagRoot.transform.localScale.x;
        float nextScale = Mathf.Clamp(currentScale + ParameterManager.Instance.PlayerParameter.PLAYER_INCREASE_BAG_SCALE, 1, ParameterManager.Instance.PlayerParameter.PLAYER_MAX_BAG_SCALE);
        bagRoot.transform.localScale = Vector3.one * nextScale;
        bagRoot.transform.DOPunchScale(Vector3.one * nextScale, 0.2f, 11, 0.408f).SetEase(Ease.OutQuad);

        playerEffectView.ShowGetFoodEffect();
    }

    /// <summary>
    /// ライバルを攻撃
    /// </summary>
    /// <param name="isRight"></param>
    /// <param name="motionCompleteCallback"></param>
    public void AttackRival(bool isRight, UnityAction motionCompleteCallback)
    {

        AttackRivalMotion(isRight);
        playerEffectView.GenerateRivalAttackEffect(isRight);
        DOVirtual.DelayedCall(
            GET_FOOD_BY_RIVAL_DELAY_TIME,
            () =>
            {
                motionCompleteCallback.Invoke();
            }
        );
    }

    /// <summary>
    /// クラッシュ
    /// </summary>
    public void Clash()
    {
        ClashMotion();
    }

    /// <summary>
    /// 吹っ飛び開始
    /// </summary>
    public void StartBlow()
    {
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerEffectView.ShowStunnedEffect();
        DamageMotion();
    }

    /// <summary>
    /// 吹っ飛び終了
    /// </summary>
    public void EndBlow()
    {
        playerEffectView.HideStunnedEffect();
        // ジタバタモーション停止
        RecoveryDamageMotion();
    }

    /// <summary>
    /// 障害物衝突
    /// </summary>
    /// <param name="rate"></param>
    public void HitObstacle(float rate = 1f)
    {
        prevBagScale = bagRoot.transform.localScale.x;

        bagRoot.transform.localScale -= Vector3.one * rate * ParameterManager.Instance.PlayerParameter.PLAYER_INCREASE_BAG_SCALE;
        if (bagRoot.transform.localScale.x <= 1)
        {
            bagRoot.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 成功
    /// </summary>
    public void Success()
    {
        // rigidbody.isKinematic = true;
        playerEffectView.HideSweatEffect();
    }

    /// <summary>
    /// 失敗
    /// </summary>
    public void Failed()
    {
        playerEffectView.HideSweatEffect();
    }

    //================================================================================
    // アニメーション関連
    //================================================================================
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsPrepare = Animator.StringToHash("isPrepare");
    private static readonly int IsMove = Animator.StringToHash("isMove");
    private static readonly int IsClear = Animator.StringToHash("isClear");
    private static readonly int IsClash = Animator.StringToHash("isClash");
    private static readonly int IsDeath = Animator.StringToHash("isDeath");
    private static readonly int IsDash = Animator.StringToHash("isDash");
    private static readonly int IsDamage = Animator.StringToHash("isDamage");
    private static readonly int CrashSpeed = Animator.StringToHash("CrashSpeed");
    private static readonly int RightAttackTrigger = Animator.StringToHash("RightAttackTrigger");
    private static readonly int LeftAttackTrigger = Animator.StringToHash("LeftAttackTrigger");

    /// <summary>
    /// アニメーションを初期化
    /// </summary>
    public void InitMotion()
    {

        playerAnimator.SetBool(IsPrepare, true);
        playerAnimator.SetBool(IsMove, false);
        playerAnimator.SetBool(IsClear, false);
        playerAnimator.SetBool(IsClash, false);
        playerAnimator.SetBool(IsDeath, false);
        playerAnimator.SetBool(IsDash, false);
        playerAnimator.SetBool(IsDamage, false);
        playerAnimator.ResetTrigger(RightAttackTrigger);
        playerAnimator.ResetTrigger(LeftAttackTrigger);

        bicycleView.Animator.SetBool(IsPrepare, true);
        bicycleView.Animator.SetBool(IsMove, false);
        bicycleView.Animator.SetBool(IsClear, false);
        bicycleView.Animator.SetBool(IsClash, false);
        bicycleView.Animator.SetBool(IsDeath, false);
        bicycleView.Animator.SetBool(IsDash, false);
        bicycleView.Animator.SetBool(IsDamage, false);
        bicycleView.Animator.ResetTrigger(RightAttackTrigger);
        bicycleView.Animator.ResetTrigger(LeftAttackTrigger);

        // playerAnimator.SetFloat("HorizontalMoveSpeed", 0f);
        // bicycleView.Animator.SetFloat("HorizontalMoveSpeed", 0f);
    }

    public void ResetMotion()
    {
        InitMotion();
        playerAnimator.Play(Start);
        bicycleView.Animator.Play(Start);
    }

    /// <summary>
    /// ホーム用: 登場モーション
    /// </summary>
    public void StartEnterStageMotion()
    {
        playerAnimator.SetTrigger("EnterStageTrigger");
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartMoveMotion()
    {
        playerAnimator.SetBool(IsPrepare, false);
        bicycleView.Animator.SetBool(IsPrepare, false);
        MoveMotion();
    }

    /// <summary>
    /// 移動モーション
    /// </summary>
    public void MoveMotion()
    {
        playerAnimator.SetBool(IsMove, true);
        bicycleView.Animator.SetBool(IsMove, true);
    }

    /// <summary>
    /// 横移動
    /// </summary>
    /// <param name="diffPositionX"></param>
    public void BendMotion(float diffPositionX = 0f)
    {
        // playerAnimator.SetFloat("HorizontalMoveSpeed", diffPositionX / Time.deltaTime);
        // bicycleView.Animator.SetFloat("HorizontalMoveSpeed", diffPositionX / Time.deltaTime);
    }

    /// <summary>
    /// ジタバタダメージモーション
    /// </summary>
    public void DamageMotion()
    {
        playerAnimator.SetBool(IsDamage, true);
        bicycleView.Animator.SetBool(IsDamage, true);
    }

    /// <summary>
    /// ジタバタダメージから復帰
    /// </summary>
    public void RecoveryDamageMotion()
    {
        playerAnimator.SetBool(IsDamage, false);
        bicycleView.Animator.SetBool(IsDamage, false);
    }

    /// <summary>
    /// クラッシュ
    /// </summary>
    public void ClashMotion()
    {
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.SetBool(IsClash, true);
        bicycleView.Animator.SetBool(IsClash, true);
        // playerAnimator.SetFloat("HorizontalMoveSpeed", 0f);
        // bicycleView.Animator.SetFloat("HorizontalMoveSpeed", 0f);
    }

    /// <summary>
    /// クラッシュから復帰
    /// </summary>
    public void RecoveryClashMotion()
    {
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.SetBool(IsClash, false);
        bicycleView.Animator.SetBool(IsClash, false);
    }

    /// <summary>
    /// ダッシュ開始
    /// </summary>
    public void StartDashMotion()
    {
        playerAnimator.SetBool(IsDash, true);
        bicycleView.Animator.SetBool(IsDash, true);
    }

    /// <summary>
    /// ダッシュ停止
    /// </summary>
    public void StopDashMotion()
    {
        playerAnimator.SetBool(IsDash, false);
        bicycleView.Animator.SetBool(IsDash, false);
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    public void JumpMotion()
    {
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.Play(Jump);
        bicycleView.Animator.Play(Jump);
    }

    /// <summary>
    /// ライバル配達員を攻撃
    /// </summary>
    /// <param name="isRight"></param>
    public void AttackRivalMotion(bool isRight)
    {
        if (isRight)
        {
            playerAnimator.SetTrigger(RightAttackTrigger);
            bicycleView.Animator.SetTrigger(RightAttackTrigger);
        }
        else
        {
            playerAnimator.SetTrigger(LeftAttackTrigger);
            bicycleView.Animator.SetTrigger(LeftAttackTrigger);
        }
    }

    /// <summary>
    /// クリア
    /// </summary>
    public void ClearMotion()
    {
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.SetBool(IsClear, true);
        bicycleView.Animator.SetBool(IsClear, true);
    }

    /// <summary>
    /// 失敗
    /// </summary>
    public void FailedMotion(float deathMotionTime = 0.5f)
    {
        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.SetFloat(CrashSpeed, deathMotionTime);
        bicycleView.Animator.SetFloat(CrashSpeed, deathMotionTime);
        playerAnimator.SetBool(IsDeath, true);
        bicycleView.Animator.SetBool(IsDeath, true);
    }

}