using System.Collections;
using System.Collections.Generic;
using System.Linq;
using app_system;
using DG.Tweening;
using NativeUtil;
using template;
using TouchUtility;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// プレイヤー管理クラス
/// </summary>
public class PlayerController : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    /// <summary>ルート</summary>
    [SerializeField] private GameObject root;
    /// <summary>プレイヤーのルート</summary>
    [SerializeField] private GameObject playerRoot;
    /// <summary>プレイヤービュー</summary>
    [SerializeField] private PlayerView playerView;
    /// <summary>クラッシュ時の食べ物のビュー</summary>
    [SerializeField] private DropedFoodView dropedFoodView;
    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>現在のライフ</summary>
    private int currentLife;
    /// <summary>獲得した食べ物のIDリスト</summary>
    private List<int> getFoodIdList;

    /// <summary>自機のステータス</summary>
    private InGameConstant.PlayerStatus playerStatus;

    /// <summary>滑り状態か</summary>
    private bool isSlipState = false;
    /// <summary>無敵状態か</summary>
    private bool isInvincible = false;
    /// <summary>ジャンプ開始したか</summary>
    private bool isStartJump = false;

    /// <summary>タッチ開始したか</summary>
    private bool isTouchStart = false;

    /// <summary>自機のRigidbody</summary>
    private Rigidbody playerRigidbody;
    /// <summary>ドラッグの移動距離</summary>
    private float dragSpeed;
    /// <summary>加速度</summary>
    private float accelerationSpeed = 1f;
    /// <summary>加速倍率</summary>
    private const float accelerationRate = 0.8f;
    /// <summary>現在の速度</summary>
    private float currentSpeed = 0f;
    /// <summary>X軸の慣性</summary>
    private float beforeInertiaX = 0f;
    /// <summary>タッチイベント</summary>
    private InGameConstant.TouchEvent touchEvent;
    /// <summary>食べ物獲得時のコールバック</summary>
    private UnityAction getFoodEvent = null;
    /// <summary>突風時のコールバック</summary>
    private UnityAction<Vector3, float> windEvent = null;
    //================================================================================
    // 定数
    //================================================================================
    static readonly float DEFAULT_ACCELERATION_SPEED = 1f;
    //================================================================================
    // プロパティ
    //================================================================================
    /// <summary>ビュー</summary>
    public PlayerView View => playerView;
    /// <summary>自機の座標</summary>
    public Vector3 Position => this.transform.position;
    /// <summary>現在のライフ</summary>
    public int CurrentLife => currentLife;
    /// <summary>アバターによるボーナスコイン</summary>
    public int AvatarBonusCoin { get; private set; } = 0;
    /// <summary>バッグによる食べ物引き寄せレベル</summary>
    public int BagMagnetLevel { get; private set; } = 0;
    /// <summary>獲得した食べ物のIDリスト</summary>
    public List<int> GetFoodIdList => getFoodIdList;
    /// <summary>コイン数</summary>
    public int CoinCount { get; private set; } = 0;
    /// <summary>ボーナスコイン数</summary>
    public int BonusCoinCount { get; private set; } = 0;
    /// <summary>ジュエル数</summary>
    public int JewelCount { get; private set; } = 0;
    /// <summary>ステータス</summary>
    public InGameConstant.PlayerStatus PlayerStatus => playerStatus;
    /// <summary>滑り状態か</summary>
    public bool IsSlipState => isSlipState;
    /// <summary>ダッシュ中か</summary>
    public bool IsDash => accelerationSpeed > 1f;
    /// <summary>前の滑り値</summary>
    public float BeforeInertiaX => beforeInertiaX;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(UnityAction getFoodEvent, UnityAction<Vector3, float> windEvent)
    {
        this.transform.position = Vector3.zero;
        root.SetActive(true);
        root.transform.localRotation = Quaternion.Euler(0, 0, 0);
        playerRigidbody = this.GetComponent<Rigidbody>();
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.isKinematic = false;
        playerRigidbody.rotation = Quaternion.Euler(0, 0, 0);
        accelerationSpeed = DEFAULT_ACCELERATION_SPEED;
        currentSpeed = 0f;
        beforeInertiaX = 0f;
        getFoodIdList = new List<int>();
        CoinCount = 0;
        BonusCoinCount = 0;
        JewelCount = 0;
        playerStatus = InGameConstant.PlayerStatus.Prepare;
        isSlipState = false;
        isInvincible = false;
        isStartJump = false;
        playerView.Init();
        dropedFoodView.Init();
        this.getFoodEvent = getFoodEvent;
        this.windEvent = windEvent;

        ApplyPlayerData();
        playerView.InitMotion();
    }

    void ApplyPlayerData()
    {
        var saveData = UserDataProvider.Instance.GetSaveData();
        // バッグ
        int bagId = saveData.SelectBagId;
        var bagData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bag, bagId) as BagData;
        // NOTE: レベルは6段階
        BagMagnetLevel = bagData.magnetLevel;
        playerView.ChangeBag(bagData);

        // アバター
        int avatarId = saveData.SelectAvatarId;
        var avatarData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Avatar, avatarId) as AvatarData;
        // NOTE: レベルは6段階
        AvatarBonusCoin = avatarData.bonusCoin;
        playerView.ChangeAvatar(avatarData);

        // 自転車
        int bicycleId = saveData.SelectBicycleId;
        var bicycleData = AssetManager.Instance.GetShopItemData(TitleConstant.ShopItemType.Bicycle, bicycleId) as BicycleData;
        // NOTE: レベルは6段階
        currentLife = bicycleData.playerLife;
        playerView.ChangeBicycle(bicycleData);
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGame()
    {
        playerStatus = InGameConstant.PlayerStatus.Move;
        touchEvent = InGameConstant.TouchEvent.None;

        playerView.StartMoveMotion();
    }

    private void Update()
    {
        if (playerStatus == InGameConstant.PlayerStatus.Move)
        {
            // 移動中
            dragSpeed = 0f;
            var phase = TouchUtil.GetPhase();

            if (phase == GodPhase.Began)
            {
                isTouchStart = true;
                Vector2 touchPosition = TouchUtil.GetPosition();
                touchEvent =
                    touchPosition.x < Screen.width / 2 ? InGameConstant.TouchEvent.Left : touchPosition.x > Screen.width / 2 ? InGameConstant.TouchEvent.Right :
                    InGameConstant.TouchEvent.None;
            }
            else if (phase == GodPhase.Moved && isTouchStart)
            {
                Vector2 touchDeltaPosition = TouchUtil.GetDeltaPosition();
                float dragValue = touchDeltaPosition.x / (float)Screen.width * TouchUtil.BaseScreenSizeX;
                // Debug.Log (dragValue + " = " + touchDeltaPosition.x + " / " + Screen.width + " * " + TouchUtil.BaseScreenSizeX);
                // Debug.LogError (dragValue);
                if (float.IsNaN(dragValue))
                {
                    return;
                }
                else if (Mathf.Abs(dragValue) < ParameterManager.Instance.PlayerParameter.DRAG_LIMIT_MIN)
                {
                    // NOTE: Mobile対応
                    //       ある程度動かないと判定として受け付けない
                    return;
                }

                // NOTE: 食べ物の重量による移動制限
                float dragRate = 1f;
                if (playerView.BagScale <= 1)
                {
                    // 1より小さい時
                    dragRate = 1f;
                }
                else if (playerView.BagScale >= ParameterManager.Instance.PlayerParameter.PLAYER_MAX_BAG_SCALE)
                {
                    // 上限サイズになった時
                    dragRate = 1 - ((ParameterManager.Instance.PlayerParameter.PLAYER_MAX_BAG_SCALE - 1f) * ParameterManager.Instance.PlayerParameter.PLAYER_INRTIA_RATE);
                }
                else
                {
                    // サイズによって、ドラッグ量を下げる
                    dragRate = 1 - ((playerView.BagScale - 1f) * ParameterManager.Instance.PlayerParameter.PLAYER_INRTIA_RATE);
                }
                // NOTE: 移動量を大きくしないといけないパターン
                dragSpeed = dragValue * dragRate;
                // Debug.Log (dragSpeed );

                Vector2 touchPosition = TouchUtil.GetPosition();
                touchEvent =
                    touchPosition.x < Screen.width / 2 ? InGameConstant.TouchEvent.Left : touchPosition.x > Screen.width / 2 ? InGameConstant.TouchEvent.Right :
                    InGameConstant.TouchEvent.None;
            }
            else if (phase == GodPhase.Ended && isTouchStart)
            {
                // タッチ終了かつドラッグ開始なら
                isTouchStart = false;
                touchEvent = InGameConstant.TouchEvent.None;
            }
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void Move()
    {

        if (playerStatus != InGameConstant.PlayerStatus.Move)
        {
            return;
        }

        if (!isSlipState)
        {
            // 通常状態
            NormalStateMove();
        }
        else
        {
            // 滑り状態
            SlipStateMove();
        }

        // プレイヤーの状態更新
        UpDateStatus();
    }

    /// <summary>
    /// 通常状態の移動
    /// </summary>
    private void NormalStateMove()
    {
        var prevPosition = playerRigidbody.position;
        currentSpeed = IsDash ? accelerationSpeed : Mathf.Clamp(currentSpeed + (Time.deltaTime * accelerationRate), 0f, accelerationSpeed);
        var forwardMovement = Vector3.forward * InGameConstant.PLAYER_SPEED * Time.deltaTime * currentSpeed;
        var playerPosition = playerRigidbody.position + forwardMovement;
        if (dragSpeed != 0)
        {
            // NOTE: 空回り中か否かで横の移動速度を決定
            float moveX = dragSpeed * ParameterManager.Instance.PlayerParameter.PLAYER_X_SPEED * Time.deltaTime / (playerView.IsSpinWheel ? ParameterManager.Instance.PlayerParameter.SPIN_WHEEL_PLAYER_X_SPEED_RATE : 1f);
            // 1フレーム辺りの移動量の制限
            playerPosition.x += Mathf.Clamp(
                moveX, -ParameterManager.Instance.PlayerParameter.LIMIT_PLAYER_X_SPEED * Time.deltaTime,
                ParameterManager.Instance.PlayerParameter.LIMIT_PLAYER_X_SPEED * Time.deltaTime
            );
        }

        // 移動制限チェック
        var diffPosition = playerPosition - prevPosition;
        // 傾きアニメーション
        playerView.Bend(playerPosition, dragSpeed, beforeInertiaX, ParameterManager.Instance.PlayerParameter.PLAYER_LIMIT_TILT, false);

        // playerPosition = CheckMoveRange(prevPosition, playerPosition);
        playerRigidbody.MovePosition(playerPosition);
    }

    /// <summary>
    /// 滑り状態の移動
    /// </summary>
    private void SlipStateMove()
    {
        var prevPosition = playerRigidbody.position;
        currentSpeed = IsDash ? accelerationSpeed : Mathf.Clamp(currentSpeed + (Time.deltaTime * accelerationRate), 0f, accelerationSpeed);
        var forwartMovement = Vector3.forward * InGameConstant.PLAYER_SPEED * Time.deltaTime * currentSpeed;
        var playerPosition = playerRigidbody.position + forwartMovement;
        // 1フレーム辺りの移動量の制限
        if (dragSpeed != 0)
        {
            float moveX = dragSpeed * ParameterManager.Instance.PlayerParameter.SLIP_PLAYER_X_SPEED * Time.deltaTime / (playerView.IsSpinWheel ? ParameterManager.Instance.PlayerParameter.SLIP_SPIN_WHEEL_PLAYER_X_SPEED_RATE : 1f);
            playerPosition.x += Mathf.Clamp(
                moveX, -ParameterManager.Instance.PlayerParameter.SLIP_LIMIT_PLAYER_X_SPEED * Time.deltaTime,
                ParameterManager.Instance.PlayerParameter.SLIP_LIMIT_PLAYER_X_SPEED * Time.deltaTime
            );
        }

        // 移動制限チェック
        var diffPosition = playerPosition - prevPosition;
        // NOTE: 滑り値の計算
        if (ParameterManager.Instance.PlayerParameter.SLIP_PLAYER_DECREASE_INRTIA_X_RATE != 0)
        {
            beforeInertiaX += diffPosition.x;

            // NOTE: 滑り値を徐々に減らす
            if (beforeInertiaX > 0 && diffPosition.x <= 0)
            {
                beforeInertiaX = beforeInertiaX - Time.deltaTime < 0 ? 0 : beforeInertiaX - Time.deltaTime;
            }
            else if (beforeInertiaX < 0 && diffPosition.x >= 0)
            {
                beforeInertiaX = beforeInertiaX + Time.deltaTime > 0 ? 0 : beforeInertiaX + Time.deltaTime;
            }
            // Debug.LogWarning (beforeInertiaX);
            float inertiaRate = playerView.IsSpinWheel ? 1f : 1f;
            float inertiaX = Mathf.Clamp(
                beforeInertiaX * ParameterManager.Instance.PlayerParameter.SLIP_PLAYER_DECREASE_INRTIA_X_RATE * inertiaRate * Time.deltaTime,
                -ParameterManager.Instance.PlayerParameter.SLIP_LIMIT_INERTIA_X * Time.deltaTime,
                ParameterManager.Instance.PlayerParameter.SLIP_LIMIT_INERTIA_X * Time.deltaTime
            );
            playerPosition.x += inertiaX;
        }
        // 傾きアニメーション
        playerView.Bend(playerPosition, dragSpeed, beforeInertiaX, ParameterManager.Instance.PlayerParameter.SLIP_PLAYER_LIMIT_TILT, true);

        // playerPosition = CheckMoveRange(prevPosition, playerPosition);
        playerRigidbody.MovePosition(playerPosition);
    }

    /// <summary>
    /// ステータス更新
    /// </summary>
    private void UpDateStatus()
    {

        // NOTE: 食べ物3個で滑り状態に移行
        if (!isSlipState && getFoodIdList.Count >= 3)
        {
            isSlipState = true;
            playerView.PlayerEffectView.ShowStartSlipEffect();
            SoundManager.Instance.Play<SEContainer>(SEName.SE_HEAVY);
        }
        else if (getFoodIdList.Count < 3)
        {
            isSlipState = false;
        }

        // NOTE: 落下した場合、死亡判定
        if (this.Position.y < InGameConstant.PLAYER_GAME_OVER_HEIGHT)
        {
            playerStatus = InGameConstant.PlayerStatus.FallDeath;
        }

    }

    /// <summary>
    /// キャラクターを加速状態にする処理.
    /// </summary>
    public void Acceleration(float time, float power)
    {
        // isAcceleration = true;
        // accelerationTime = time;
        // playerRigidbody.AddForce (new Vector3 (0, 0, power), ForceMode.Impulse);
        accelerationSpeed = power;
        beforeInertiaX = 0;
        playerView.Dash(time);
        // NOTE: 徐々に減速
        DOTween.To(
            () => accelerationSpeed, // 何を対象にするのか
            value => accelerationSpeed = value, // 値の更新
            DEFAULT_ACCELERATION_SPEED, // 最終的な値
            time // アニメーション時間
        );
        SoundManager.Instance.Play<SEContainer>(SEName.SE_DASH);
    }

    /// <summary>
    /// ジャンプ台
    /// </summary>
    public void JumpBoard(float time, float fowardPower, float upPower)
    {
        if (isStartJump)
        {
            return;
        }
        Debug.Log("ジャンプ");
        isStartJump = true;
        beforeInertiaX = 0;
        playerRigidbody.AddForce(new Vector3(0, upPower, fowardPower), ForceMode.Impulse);
        DOVirtual.DelayedCall(
            time,
            () =>
            {
                playerView.JumpMotion();
                isStartJump = false;
            }
        );
        SoundManager.Instance.Play<SEContainer>(SEName.SE_JUMP);
    }

    /// <summary>
    /// 風に押されるギミック
    /// </summary>
    public void WindPush(Vector3 pushVector, float pushDelayTime)
    {
        Debug.Log("風に押された");
        if (this.windEvent != null)
        {
            this.windEvent(pushVector, pushDelayTime);
        }
        Sequence sequence = DOTween.Sequence()
            .OnStart(() => { })
            .InsertCallback(
                pushDelayTime - 0.5f,
                () =>
                {
                    SoundManager.Instance.Play<SEContainer>(SEName.SE_WIND);
                    playerView.PlayerEffectView.ShowWindEffect(pushVector);
                    Debug.Log("エフェクト再生");
                }
            )
            .InsertCallback(
                pushDelayTime,
                () =>
                {
                    beforeInertiaX = 0;
                    playerRigidbody.AddForce(pushVector, ForceMode.Impulse);
                    Debug.Log("押す処理");
                }
            )
            .AppendInterval(0.5f)
            .OnComplete(() =>
            {
                playerView.PlayerEffectView.HideWindEffect();
                Debug.Log("エフェクト非表示");
            });
    }

    /// <summary>
    /// 衝突イベント
    /// </summary>
    /// <param name="hitEvent"></param>
    /// <param name="value"></param>
    private void HitEvent(InGameConstant.HitEvent hitEvent, int value)
    {
        Debug.Log("HitEvent: " + hitEvent);
        if (hitEvent == InGameConstant.HitEvent.Food)
        {
            // 食べ物獲得
            GetFood(value);
        }
        else if (hitEvent == InGameConstant.HitEvent.Coin)
        {
            // コイン獲得
            GetCoin();
        }
        else if (hitEvent == InGameConstant.HitEvent.BonusCoin)
        {
            // ボーナスコイン獲得
            GetBonusCoin();
        }
        else if (hitEvent == InGameConstant.HitEvent.Jewel)
        {
            // ジュエル獲得
            GetJewl();
        }
        else if (hitEvent == InGameConstant.HitEvent.Goal)
        {
            playerStatus = InGameConstant.PlayerStatus.Goal;
        }
    }

    /// <summary>
    /// 食べ物獲得
    /// </summary>
    /// <param name="foodId"></param>
    private void GetFood(int foodId)
    {
        if (foodId != 0)
        {
            Debug.Log("食べ物獲得。ID：" + foodId);
            getFoodIdList.Add(foodId);
        }
        else
        {
            Debug.LogError("食べ物IDの取得に失敗。プレハブを修正する必要があります");
        }
        MobileVibe();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_GET_FOOD);
        playerView.GetFood();
        getFoodEvent?.Invoke();
    }

    /// <summary>
    /// コイン獲得
    /// </summary>
    private void GetCoin()
    {
        Debug.Log("コイン取得");
        CoinCount++;
        MobileVibe();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_GET_COIN);
    }

    /// <summary>
    /// ボーナスコイン獲得
    /// </summary>
    private void GetBonusCoin()
    {
        Debug.Log("ボーナスコイン取得");
        BonusCoinCount++;
        MobileVibe();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_GET_COIN);
    }

    /// <summary>
    /// ジュエル獲得
    /// </summary>
    private void GetJewl()
    {
        Debug.Log("ジュエル取得");
        JewelCount++;
        MobileVibe();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_GET_DIAMOND);
    }

    /// <summary>
    /// 障害物衝突イベント
    /// </summary>
    private void HitObstacleEvent(BlowGimickView blowGimickView)
    {

        if (isInvincible)
        {
            // 無敵状態なら、ダメージ受けない
            return;
        }

        if (blowGimickView != null)
        {
            // 吹っ飛びクラッシュ
            BlowClash(blowGimickView);
        }
        else
        {
            // 吹っ飛び無しクラッシュ
            NoBlowClash();
        }
    }

    /// <summary>
    /// ライバル衝突イベント
    /// </summary>
    private void HitRivalEvent(RivalView rivalView)
    {
        if (rivalView == null)
        {
            Debug.LogError("RivalViewが取得できませんでした。");
            return;
        }
        if (rivalView.RivalType == InGameConstant.RivalType.FoodDelivery)
        {
            // 食べ物配達員
            playerView.AttackRival(isRight: Position.x < rivalView.transform.position.x, () =>
            {
                // ちょっと時間差で食べ物取得
                GetFood(rivalView.FoodData.Id);
                Debug.Log("ライバル配達員から食べ物獲得。");
                SoundManager.Instance.Play<SEContainer>(SEName.SE_PUNCH);
            });
            rivalView.Clash();
        }
    }

    /// <summary>
    /// 吹っ飛びクラッシュ
    /// </summary>
    private void BlowClash(BlowGimickView blowGimickView)
    {

        playerStatus = InGameConstant.PlayerStatus.Blow;
        var blowPathList = blowGimickView.GetBlowPathList();
        float blowrRotate = blowPathList.Last().z > this.Position.z ? 360f : -360;

        Sequence sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                // ジタバタモーション再生
                isInvincible = true;
                DropFood();
                Damage();
                if (CurrentLife > 0)
                {
                    playerView.StartBlow();
                    playerRigidbody.isKinematic = true;
                }
                else
                {
                    playerView.FailedMotion(1 / blowGimickView.DurationTime);
                    playerView.Failed();
                }
                SoundManager.Instance.Play<SEContainer>(SEName.SE_CLASH);
            })
            .Append(
                playerRigidbody.transform
                .DOPath(blowPathList, blowGimickView.DurationTime, PathType.CatmullRom)
                .SetEase(blowGimickView.Ease)
            ).AppendCallback(() =>
            {
                if (CurrentLife > 0)
                {
                    // 吹っ飛び終了
                    playerView.EndBlow();
                }
            });
        sequence.OnComplete(() =>
        {
            // playerView.RecoveryClashMotion ();
            if (currentLife == 0)
            {
                playerStatus = InGameConstant.PlayerStatus.Death;
            }
            else
            {
                playerStatus = InGameConstant.PlayerStatus.Move;
            }
            currentSpeed = 0f;
            isInvincible = false;
            playerRigidbody.isKinematic = false;
        });
        sequence.Play();
    }

    /// <summary>
    /// 吹っ飛ばないクラッシュ
    /// </summary>
    private void NoBlowClash()
    {
        Damage();
        DropFood();
        SoundManager.Instance.Play<SEContainer>(SEName.SE_SMALL_CLASH);

        if (currentLife == 0)
        {
            playerStatus = InGameConstant.PlayerStatus.Death;
            playerView.FailedMotion();
            return;
        }
        isInvincible = true;
        playerView.StartBlow();

        DOVirtual.DelayedCall(
            InGameConstant.PLAYER_NO_BLOW_CLASH_WAIT_TIME,
            () =>
            {
                isInvincible = false;
                playerView.EndBlow();
            });
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    private void Damage()
    {
        currentLife--;
        beforeInertiaX = 0;
    }

    /// <summary>
    /// 食べ物を落とす
    /// </summary>
    private void DropFood()
    {
        if (getFoodIdList.Count > 0)
        {
            int dropFoodIndex = Random.Range(0, getFoodIdList.Count);
            Debug.Log("落とした食べ物ID:" + getFoodIdList[dropFoodIndex]);
            dropedFoodView.DropFood(getFoodIdList[dropFoodIndex]);
            getFoodIdList.RemoveAt(dropFoodIndex);
            getFoodEvent?.Invoke();
            playerView.HitObstacle();
        }
    }

    /// <summary>
    /// 獲得した食べ物をリセット
    /// </summary>
    public void ResetGetFood()
    {
        getFoodIdList = new List<int>();
    }

    /// <summary>
    /// 端末振動
    /// </summary>
    private void MobileVibe()
    {
        // #if UNITY_IOS && !UNITY_EDITOR
        //         IOSUtil.PlaySystemSound (1519);
        // #elif UNITY_ANDROID && !UNITY_EDITOR
        //         AndroidUtil.Vibrate ((long) 10.0f);
        // #endif
    }

    /// <summary>
    /// チュートリアル開始
    /// </summary>
    public void StartTutorial(TutorialGimmick tutorialGimmick)
    {
        var tutorialData = TutorialManager.Instance.GetTutorialData(tutorialGimmick.TutorialId);
        if (tutorialData == null)
        {
            Debug.LogError($"チュートリアルデータの取得に失敗。TutorialId={tutorialGimmick.TutorialId}");
            return;
        }
        Time.timeScale = tutorialData.TimeScale;
        TutorialManager.Instance.ShowInGameTutorial(tutorialData, () =>
        {
            Time.timeScale = 1f;
        });

    }

    /// <summary>
    /// 成功演出
    /// </summary>
    public void Success()
    {
        playerView.Success();
        playerRigidbody.velocity = Vector3.zero;
        playerView.ClearMotion();

        var sb = new System.Text.StringBuilder();
        sb.Append("<color=yellow>=====リザルト=====</color>\n");
        sb.Append($"<color=yellow>獲得した食べ物ID: {string.Join(", ", getFoodIdList.Select(obj => obj.ToString()))}</color>\n");
        foreach (var foodId in getFoodIdList)
        {
            var foodInfo = AssetManager.Instance.GetFoodInfo(foodId);
            sb.Append($"<color=yellow>獲得した食べ物 ID:{foodInfo.Data.Id} 名前:{foodInfo.Data.Name}</color>\n");
        }
        sb.Append($"<color=yellow>コイン数: {CoinCount}</color>\n");
        sb.Append($"<color=yellow>ボーナスコイン数: {BonusCoinCount}</color>\n");
        sb.Append($"<color=yellow>Jewel数: {JewelCount}</color>\n");
        Debug.Log(sb.ToString());
        playerStatus = InGameConstant.PlayerStatus.Finish;
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Death()
    {
        Debug.Log("獲得した食べ物の数：" + getFoodIdList.Count);
        playerView.Failed();
        playerStatus = InGameConstant.PlayerStatus.Finish;
    }

    /// <summary>
    /// 復活
    /// </summary>
    public void Revive()
    {
        playerStatus = InGameConstant.PlayerStatus.Prepare;
        currentSpeed = 0f;
        currentLife = 1;
        isInvincible = false;
        playerRigidbody.isKinematic = false;
        playerView.ResetMotion();
    }

    //================================================================================
    // 当たり判定
    //================================================================================
    private void OnCollisionEnter(Collision other)
    {
        if (playerStatus != InGameConstant.PlayerStatus.Move)
        {
            return;
        }
        if (other.gameObject.CompareTag(Tags.Obstacle))
        {
            // 障害物
            var blowGimickView = other.gameObject.GetComponent<BlowGimickView>();
            HitObstacleEvent(blowGimickView);
            if (other.gameObject.TryGetComponent<ZombieView>(out var zombieView))
            {
                Debug.Log("ゾンビと当たった");
                zombieView.BlowAway((other.transform.position - Position).normalized);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerStatus != InGameConstant.PlayerStatus.Move)
        {
            return;
        }
        if (other.gameObject.CompareTag(Tags.Food))
        {
            // 食べ物
            other.gameObject.SetActive(false);
            var foodView = other.gameObject.GetComponent<FoodView>();
            int foodId = foodView != null ? foodView.FoodData.Id : 0;
            HitEvent(InGameConstant.HitEvent.Food, foodId);
        }
        else if (other.gameObject.CompareTag(Tags.Coin))
        {
            // コイン
            other.gameObject.SetActive(false);
            HitEvent(InGameConstant.HitEvent.Coin, 0);
        }
        else if (other.gameObject.CompareTag(Tags.BonusCoin))
        {
            // ボーナスコイン
            other.gameObject.SetActive(false);
            HitEvent(InGameConstant.HitEvent.BonusCoin, 0);
        }
        else if (other.gameObject.CompareTag(Tags.Jewel))
        {
            // ジュエル
            other.gameObject.SetActive(false);
            HitEvent(InGameConstant.HitEvent.Jewel, 0);
        }
        else if (other.gameObject.CompareTag(Tags.Goal))
        {
            // ゴール
            HitEvent(InGameConstant.HitEvent.Goal, 0);
        }
        else if (other.gameObject.CompareTag(Tags.Obstacle))
        {
            // 障害物
            var blowGimickView = other.gameObject.GetComponent<BlowGimickView>();
            HitObstacleEvent(blowGimickView);
            if (other.gameObject.TryGetComponent<ZombieView>(out var zombieView))
            {
                Debug.Log("ゾンビと当たった");
                zombieView.BlowAway((other.transform.position - Position).normalized);
            }
        }
        else if (other.gameObject.CompareTag(Tags.Rival))
        {
            // ライバル
            var rivalView = other.gameObject.GetComponent<RivalView>();
            HitRivalEvent(rivalView);
        }
        else if (other.gameObject.CompareTag(Tags.Tutorial))
        {
            // チュートリアル
            if (other.gameObject.TryGetComponent<TutorialGimmick>(out var tutorialGimmick))
            {
                StartTutorial(tutorialGimmick);
            }
        }
    }

}