using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// インゲームカメラ管理クラス
/// </summary>
public class InGameCameraController : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private CinemachineVirtualCamera startCamera;
    [SerializeField] private CinemachineVirtualCamera followCamera;
    [SerializeField] private CinemachineVirtualCamera dashFollowCamera;
    [SerializeField] private CinemachineVirtualCamera deathCamera;
    [SerializeField] private ResultCamera resultCamera;
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

        offset = followCamera.gameObject.transform.localPosition;
        // ベース維持
        var scaleWidth = (Screen.height / defaultScreenY) * (defaultScreenX / Screen.width);
        var scaleRatio = Mathf.Max(scaleWidth, 1.0f);
        Debug.Log($"カメラ補正倍率: {scaleRatio}");
        startCamera.m_Lens.FieldOfView = Mathf.Atan(Mathf.Tan(startCamera.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
        followCamera.m_Lens.FieldOfView = Mathf.Atan(Mathf.Tan(followCamera.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
        dashFollowCamera.m_Lens.FieldOfView = Mathf.Atan(Mathf.Tan(dashFollowCamera.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;
        deathCamera.m_Lens.FieldOfView = Mathf.Atan(Mathf.Tan(deathCamera.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad) * scaleRatio) * 2.0f * Mathf.Rad2Deg;

        // var position = followCamera.transform.localPosition;
        // position.y = (Mathf.Floor(position.y * scaleRatio * 10)) / 10;
        // position.z = (Mathf.Floor(position.z * scaleRatio * 10)) / 10;
        // followCamera.transform.localPosition = position;

        GameVariant.Instance.Get<InGameVariant>().BaseScreenSize = new Vector2(defaultScreenX, defaultScreenY);
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(PlayerController playerController)
    {
        // followCamera.gameObject.transform.position = offset;
        startCamera.Priority = 200;
        startCamera.m_Follow = playerController.transform;
        startCamera.m_LookAt = playerController.transform;
        followCamera.Priority = 100;
        followCamera.m_Follow = playerController.transform;
        followCamera.m_LookAt = playerController.transform;
        dashFollowCamera.Priority = 100;
        dashFollowCamera.m_Follow = playerController.transform;
        dashFollowCamera.m_LookAt = playerController.transform;
        deathCamera.Priority = 100;
        deathCamera.m_Follow = playerController.transform;
        deathCamera.m_LookAt = playerController.transform;

        resultCamera.Init(playerController);
    }

    public void StartGame()
    {
        startCamera.Priority = 100;
        followCamera.Priority = 200;
    }

    /// <summary>
    /// カメラ更新
    /// </summary>
    public void UpdateCamera(PlayerController playerController)
    {
        // followCamera.gameObject.transform.position = playerController.Position + offset;
        // dashFollowCamera.gameObject.transform.position = playerController.Position + offset;
        if (playerController.IsDash)
        {
            followCamera.Priority = 100;
            dashFollowCamera.Priority = 200;
        }
        else
        {
            followCamera.Priority = 200;
            dashFollowCamera.Priority = 100;
        }
    }

    /// <summary>
    /// 死亡カメラ
    /// </summary>
    public void ShowDeathCamera()
    {
        deathCamera.transform.position = followCamera.transform.position;
        deathCamera.Priority = 300;
        deathCamera.m_Follow = null;
    }

    /// <summary>
    /// 結果カメラ表示
    /// </summary>
    /// <param name="playerPosition"></param>
    public void ShowResultCamera(Vector3 playerPosition)
    {
        resultCamera.Show(playerPosition);
    }

}