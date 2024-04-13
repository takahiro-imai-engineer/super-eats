using app_system;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 食べ物の描画クラス
/// </summary>
public class FoodView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("食べ物データ")]
    /// <summary>食べ物データ</summary>
    [SerializeField] private FoodData foodData = null;
    [Header("エフェクトルート")]
    /// <summary>エフェクトルート</summary>
    [SerializeField] private GameObject effectRoot = null;

    [Header("プレイヤーに引き寄せられるか")]
    /// <summary>プレイヤーに引き寄せられるか</summary>
    [SerializeField] private bool isSucked = true;
    //================================================================================
    // ローカル
    //================================================================================
    private Transform foodTransform;
    private PlayerController playerInfo = null;
    /// <summary>引き寄せられる範囲</summary>
    private float suckedRange = 0.1f;
    /// <summary>引き寄せられるスピード</summary>
    private float suckedSpeed = 15.0f;
    //================================================================================
    // 定数
    //================================================================================
    static readonly float FOOD_FADE_TIME = 0.25f;
    static readonly float FOOD_DESTROY_TIME = 0.25f;
    //================================================================================
    // プロパティ
    //================================================================================
    /// <summary>食べ物データ</summary>
    public FoodData FoodData => foodData;
    /// <summary>プレイヤーに引き寄せられるか</summary>
    public bool IsSucked => isSucked;
    //================================================================================
    // メソッド
    //================================================================================
    private void Start()
    {
        foodTransform = this.transform;
        if (foodData == null)
        {
            Debug.LogError("FoodDataが設定されていません。");
        }
        // NOTE: 食べ物に自機の情報を渡す
        var playerInfo = GameVariant.Instance.Get<InGameVariant>().InGameModel.PlayerInfo; ;
        SetPlayerInfo(playerInfo);
    }

    private void FixedUpdate()
    {
        if (!IsSucked || playerInfo == null)
        {
            return;
        }
        // NOTE: 指定範囲内にある場合、引き寄せ
        if ((foodTransform.position - playerInfo.Position).sqrMagnitude > suckedSpeed * suckedSpeed)
        {
            return;
        }
        var rad = Mathf.Atan2(
            playerInfo.Position.z - foodTransform.position.z,
            playerInfo.Position.x - foodTransform.position.x);

        var nextPosition = foodTransform.position;
        nextPosition.x += suckedSpeed * Mathf.Cos(rad) * Time.deltaTime;
        nextPosition.z += suckedSpeed * Mathf.Sin(rad) * Time.deltaTime;
        foodTransform.position = nextPosition;
    }

    /// <summary>
    /// プレイヤーの情報を設定
    /// </summary>
    /// <param name="playerInfo"></param>
    public void SetPlayerInfo(PlayerController playerInfo)
    {
        if (!IsSucked)
        {
            return;
        }
        this.playerInfo = playerInfo;
        switch (playerInfo.BagMagnetLevel)
        {
            case 0:
                this.playerInfo = null;
                break;
            case 1:
                suckedRange = 0.1f;
                suckedSpeed = 15f;
                break;
            case 2:
                suckedRange = 0.18f;
                suckedSpeed = 16f;
                break;
            case 3:
                suckedRange = 0.25f;
                suckedSpeed = 18f;
                break;
            case 4:
                suckedRange = 0.32f;
                suckedSpeed = 20f;
                break;
            case 5:
                suckedRange = 0.4f;
                suckedSpeed = 25f;
                break;
            case 6:
                suckedRange = 0.6f;
                suckedSpeed = 15f;
                break;
            default:
                this.playerInfo = null;
                break;
        }
    }

    /// <summary>
    /// 食べ物を落とす
    /// </summary>
    /// <param name="dropPower"></param>
    public void Drop(Vector3 dropPower)
    {
        this.effectRoot?.SetActive(false);
        this.GetComponent<Collider>().isTrigger = false;
        this.gameObject.layer = LayerMask.NameToLayer("DropFood");
        var dropFoodRigidboy = this.gameObject.AddComponent<Rigidbody>();
        dropFoodRigidboy.AddForce(dropPower, ForceMode.Impulse);
        var foodMeshList = this.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in foodMeshList)
        {
            item.material.DOFade(0, FOOD_FADE_TIME);
        }
        Destroy(this.gameObject, FOOD_DESTROY_TIME);
    }
}