using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     ローディングスクリーン
/// </summary>
public class GameLoadingScreen : OverlayObject
{
    /// <summary>ローディングアイコン</summary>
    [SerializeField] private Image loadingGauge;

    /// <summary>英語背景素材</summary>
    [SerializeField] private Texture enBackGroundTexture;

    /// <summary>日本語背景素材</summary>
    [SerializeField] private Texture jaBackGroundTexture;

    /// <summary>背景</summary>
    [SerializeField] private RawImage backGroundImage;

    public void Init()
    {
        loadingGauge.fillAmount = 0f;
        Debug.Log($"言語: {Application.systemLanguage}");
        if (Application.systemLanguage != SystemLanguage.Japanese)
            backGroundImage.texture = enBackGroundTexture;
        else
            backGroundImage.texture = jaBackGroundTexture;
    }

    public void SetProgress(float progress, float duration = 0f)
    {
        if (duration == 0f)
        {
            loadingGauge.fillAmount = progress;
            return;
        }

        var currentProgress = loadingGauge.fillAmount;
        DOTween.To(
                () => currentProgress, // 何を対象にするのか
                value => currentProgress = value, // 値の更新
                1f, // 最終的な値
                duration // アニメーション時間
            ).OnUpdate(() => { loadingGauge.fillAmount = currentProgress; })
            .SetLink(gameObject);
    }
}
