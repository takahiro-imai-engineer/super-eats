using UnityEngine;

/// <summary>
/// UI 入力無効化
/// </summary>
public class DeInputUI : OverlayObject {

    //================================================================================
    // メソッド
    //================================================================================

    /// <summary>
    /// 無効化 ON
    /// </summary>
    /// <param name="canvas">キャンバス指定</param>
    public static DeInputUI On( Canvas canvas = null ) {
        var deInput = OverlayGroup.Instance.Show<DeInputUI>( canvas );
        if( deInput == null ) {  // 既に表示中
            deInput = OverlayGroup.Instance.Get<DeInputUI>();
        }
        return deInput;
    }

    /// <summary>
    /// 無効化 OFF
    /// </summary>
    public static void Off() {
        OverlayGroup.Instance.Dismiss<DeInputUI>();
    }

    /// <summary>
    /// 無効化中かどうか
    /// </summary>
    public static bool IsDeInput {
        get { return OverlayGroup.Instance.Get<DeInputUI>() != null; }
    }

    public void OnDisable()
    {
        DeInputUI.Off();
    }
}
