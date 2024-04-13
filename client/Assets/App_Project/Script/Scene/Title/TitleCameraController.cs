using Cinemachine;
using UnityEngine;

/// <summary>
/// インゲームカメラ管理クラス
/// </summary>
public class TitleCameraController : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private CinemachineVirtualCamera homeCamera;
    [SerializeField] private CinemachineVirtualCamera shopBagCamera;
    [SerializeField] private CinemachineVirtualCamera shopAvatarCamera;
    [SerializeField] private CinemachineVirtualCamera shopBicycleCamera;
    [Header("基準となる解像度")]
    [SerializeField] private float defaultScreenX;
    [SerializeField] private float defaultScreenY;
    //================================================================================
    // ローカル
    //================================================================================
    private Vector3 offset = new Vector3();
    //================================================================================
    // メソッド
    //================================================================================
    private void Awake()
    {

        offset = homeCamera.transform.localPosition;
        // ベース維持
        var scaleRatio = Mathf.Max(0f, 1.0f);
        Debug.Log($"カメラ補正倍率: {scaleRatio}");

        homeCamera.m_Lens.FieldOfView = Mathf.Atan(Mathf.Tan(homeCamera.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
        homeCamera.Priority = 100;
        shopBagCamera.Priority = 0;
        shopAvatarCamera.Priority = 0;
        shopBicycleCamera.Priority = 0;

        GameVariant.Instance.Get<InGameVariant>().BaseScreenSize = new Vector2(defaultScreenX, defaultScreenY);
    }

    public void ShowHome()
    {
        homeCamera.Priority = 100;
        shopBagCamera.Priority = 0;
        shopAvatarCamera.Priority = 0;
        shopBicycleCamera.Priority = 0;
    }

    public void ShowShop(TitleConstant.ShopItemType shopItemType)
    {
        homeCamera.Priority = 0;
        shopBagCamera.Priority = (shopItemType == TitleConstant.ShopItemType.Bag) ? 100 : 0;
        shopAvatarCamera.Priority = (shopItemType == TitleConstant.ShopItemType.Avatar) ? 100 : 0;
        shopBicycleCamera.Priority = (shopItemType == TitleConstant.ShopItemType.Bicycle) ? 100 : 0;
    }
}