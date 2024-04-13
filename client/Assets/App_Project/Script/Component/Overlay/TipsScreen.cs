using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Tipsスクリーン
/// </summary>
public class TipsScreen : OverlayObject
{

    /// <summary>ローディングアイコン</summary>
    [SerializeField] private Image loadingGauge;
    [SerializeField] private Transform tipsRoot;
    [SerializeField] private Image tipsImage;
    [SerializeField] private List<string> tipsSpriteList;

    public void Init()
    {
        loadingGauge.fillAmount = 0f;
        var saveData = UserDataProvider.Instance.GetSaveData();
        ShowTips(saveData.TipsId);
        saveData.TipsId++;
        if (saveData.TipsId > tipsSpriteList.Count)
        {
            saveData.TipsId = 0;
        }
        UserDataProvider.Instance.WriteSaveData();
    }

    public void ShowTips(int tipsId)
    {
        tipsImage.gameObject.SetActive(false);
        if (tipsSpriteList.Count <= tipsId)
        {
            tipsId = 0;
        }
        // Instantiate(tipsPrefabs[tipsId], tipsRoot);
        LoadTips(tipsSpriteList[tipsId]);
    }

    private void LoadTips(string spriteName)
    {
        var tipsSprite = AssetManager.Instance.LoadTipsSprite(spriteName);

        if (tipsSprite == null)
        {
            Debug.LogError($"Tips画像の読み込みに失敗: spriteName={spriteName}");
            return;
        }
        tipsImage.sprite = tipsSprite;
        tipsImage.gameObject.SetActive(true);

    }

    public void SetProgress(float progress, float duration = 0f)
    {
        if (duration == 0f)
        {
            loadingGauge.fillAmount = progress;
            return;
        }
        float currentProgress = loadingGauge.fillAmount;
        DOTween.To(
            () => currentProgress, // 何を対象にするのか
            value => currentProgress = value, // 値の更新
            1f, // 最終的な値
            duration // アニメーション時間
        ).OnUpdate(() =>
        {
            loadingGauge.fillAmount = currentProgress;
        })
        .SetLink(gameObject);
    }
}
