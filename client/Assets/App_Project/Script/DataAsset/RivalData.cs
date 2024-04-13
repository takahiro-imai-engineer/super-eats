using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ライバルデータ
/// </summary>
// [CreateAssetMenu]
public class RivalData : ScriptableObject
{
    [Header("名前")]
    [SerializeField] private string name = "Rivale_00";

    [Header("キャラアイコン名")]
    [SerializeField] private string iconName = "SampleCharacterIcon";
    [Header("国旗")]
    [SerializeField] private Sprite nationalFlag = null;

    /// <summary>キャラクターの名前</summary>
    public string Name => name;

    /// <summary>キャラクターのアイコン名</summary>
    public string IconName => iconName;
    /// <summary>国旗</summary>
    public Sprite NationalFlag => nationalFlag;
}