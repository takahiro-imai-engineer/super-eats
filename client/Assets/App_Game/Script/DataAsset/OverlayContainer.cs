using UnityEngine;

/// <summary>
/// オーバレイコンテナ
/// </summary>
//[CreateAssetMenu]
public class OverlayContainer : ScriptableObject {

    /// <summary>コンテナ名</summary>
    public string ContainerName;

    /// <summary>オーバーレイオブジェクトのプレハブ</summary>
    public OverlayObject[] Prefabs;
}
