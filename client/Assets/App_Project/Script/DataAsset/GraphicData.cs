using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グラフィックデータ
/// </summary>
[CreateAssetMenu(menuName = "[Game]/Data Asset/Create GraphicData")]
public class GraphicData : ScriptableObject
{

    [SerializeField] private LightSetting lightSetting1;
    [SerializeField] private LightSetting lightSetting2;

    [Header("Skyboxに使用するマテリアル")]
    [SerializeField] private string skyboxMaterialName;
    [Header("影のレンダリングに使用する色")]
    [SerializeField] private Color realtimeShadowColor;
    [Header("環境光の色")]
    [SerializeField] private Color environmentAmbientLight;
    [Header("フォグの色")]
    [SerializeField] private Color fogColor;
    [Header("フォグの濃さ")]
    [SerializeField] private float fogDensity;

    /// <summary>ライト1の設定</summary>
    public LightSetting LightSetting1 => lightSetting1;

    /// <summary>ライト2の設定</summary>
    public LightSetting LightSetting2 => lightSetting2;

    /// <summary>Skyboxに使用するマテリアル</summary>
    public string SkyboxMaterialName => skyboxMaterialName;
    /// <summary>影のレンダリングに使用する色</summary>
    public Color RealtimeShadowColor => realtimeShadowColor;
    /// <summary>環境光の色</summary>
    public Color EnvironmentAmbientLight => environmentAmbientLight;
    /// <summary>フォグの色</summary>
    public Color FogColor => fogColor;
    /// <summary>フォグの濃さ</summary>
    public float FogDensity => fogDensity;

    [System.Serializable]
    public class LightSetting
    {
        [Header("ライトを有効化するか")]
        public bool isActive = false;
        [Header("ライトの角度")]
        public Vector3 lightAngle;

        [Header("ライトの色")]
        public Color lightColor;
        [Header("ライトの強度")]
        public float lightIntensity = 1f;
    }
}