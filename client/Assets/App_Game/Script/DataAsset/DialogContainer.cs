using UnityEngine;

/// <summary>
/// ダイアログコンテナ
/// </summary>
//[CreateAssetMenu]
public class DialogContainer : ScriptableObject {

    /// <summary>コンテナ名</summary>
    public string ContainerName;

    /// <summary>ダイアログのプレハブ</summary>
    public GeneralDialog[] Prefabs;
}
