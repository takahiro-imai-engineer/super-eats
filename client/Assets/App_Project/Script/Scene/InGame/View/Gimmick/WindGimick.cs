using DG.Tweening;
using UnityEngine;

public class WindGimick : GimmickView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("押すベクトル")]
    /// <summary>押すベクトル</summary>
    [SerializeField] private Vector3 pushVector;
    [Header("エフェクト実行とプッシュ実行の遅延時間")]
    /// <summary>ジャンプエフェクト</summary>
    [SerializeField] private float pushDelayTime = 1f;
    //================================================================================
    // 定数
    //================================================================================
    //================================================================================
    // メソッド
    //================================================================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("自機の取得に失敗");
                return;
            }
            playerController.WindPush(pushVector, pushDelayTime);
        }
    }
}