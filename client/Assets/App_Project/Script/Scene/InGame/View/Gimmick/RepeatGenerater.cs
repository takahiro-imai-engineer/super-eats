using UnityEngine;
using template;

/// <summary>
/// 指定のオブジェクトを定期的に生成するクラス
/// </summary>
public class RepeatGenerater : GimmickView
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [Header("生成の親位置")]
    /// <summary>親位置</summary>
    [SerializeField] private Transform generateParent;
    [Header("生成する座標")]
    /// <summary>生成位置</summary>
    [SerializeField] private Transform generatePosition;
    [Header("生成するオブジェクト")]
    /// <summary>生成位置</summary>
    [SerializeField] private GameObject generateObject;
    [Header("生成間隔(秒)")]
    /// <summary>生成間隔</summary>
    [SerializeField] private float repeatGenerateTime = 1f;
    [Header("生成時に再生するSE")]
    [SerializeField] private InGameConstant.GimickSE generateSE = InGameConstant.GimickSE.BoxFall;
    //================================================================================
    // ローカル
    //================================================================================
    private float elapsedTime = 0;
    //================================================================================
    // メソッド
    //================================================================================
    private void Awake()
    {
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime <= repeatGenerateTime)
        {
            return;
        }
        elapsedTime = 0f;
        Instantiate(generateObject, generatePosition.position, generatePosition.rotation, generateParent);
        if (generateSE != InGameConstant.GimickSE.None)
        {
            SoundManager.Instance.PlayGimickSE(generateSE);
        }
    }
}