using UnityEngine;

/// <summary>
/// ジャンプギミッククラス
/// </summary>
public class JumpBoardView : GimmickView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("前に加える力")]
    /// <summary>前進力</summary>
    [SerializeField] private float forwardPower;
    [Header("上に加える力")]
    /// <summary>上昇力</summary>
    [SerializeField] private float upPower;
    [Header("ジャンプモーションを再生する遅延時間")]
    /// <summary>ジャンプモーションを再生する遅延時間</summary>
    [SerializeField] private float jumpTime = 0.5f;
    /// <summary>ジャンプエフェクト</summary>
    [SerializeField] private GameObject jumpEffect = null;
    //================================================================================
    // メソッド
    //================================================================================
    private void Awake()
    {
        jumpEffect?.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            jumpEffect?.SetActive(true);
            other.gameObject.GetComponent<PlayerController>().JumpBoard(jumpTime, forwardPower, upPower);
        }
    }
}