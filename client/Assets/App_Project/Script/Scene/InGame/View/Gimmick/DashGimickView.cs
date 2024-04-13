using UnityEngine;

/// <summary>
/// ダッシュギミッククラス
/// </summary>
public class DashGimickView : GimmickView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    /// <summary>加速に使うパワーの倍率</summary>
    [SerializeField] private float speedPower;
    /// <summary>加速時間</summary>
    [SerializeField] private float accelerationTime;

    //================================================================================
    // 当たり判定
    //================================================================================
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Acceleration(accelerationTime, speedPower);
        }
    }
}