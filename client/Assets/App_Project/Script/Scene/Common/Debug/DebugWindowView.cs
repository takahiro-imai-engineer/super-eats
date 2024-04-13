using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using template;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// デバッグビュークラス
/// </summary>
public class DebugWindowView : GeneralDialog
{
    [System.Serializable]
    class DebugButton
    {
        public Button Buttton;
        public Text Text;
    }

    [System.Serializable]
    class DebugParameter
    {
        public InputField InputField;
        public Text Text;
    }

    [System.Serializable]
    class DebugSelectBox
    {
        public Dropdown Dropdown;
        //public Text Text;
    }
    //================================================================================
    // インスペクタ
    //================================================================================
    /// <summary>ウィンドウ</summary>
    [SerializeField] GameObject window;

    [SerializeField] DebugParameter progressDegreesInputField;
    [SerializeField] DebugParameter currentMoneyNumInputField;
    [SerializeField] DebugParameter totalMoneyNumInputField;
    [SerializeField] DebugParameter currentJewelNumInputField;
    [SerializeField] DebugButton isDebugStageButton;
    [SerializeField] DebugSelectBox stageSelectBox;
    [SerializeField] List<string> debugStageDataList;
    [SerializeField] DebugButton isShowAdsButton;
    [SerializeField] DebugSelectBox QualitySettingSelectBox;
    //================================================================================
    // 定数
    //================================================================================
    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>セーブデータ</summary>
    private SaveData saveData = null;
    /// <summary>インゲームデータ</summary>
    private InGameVariant inGameVariant = new InGameVariant();
    private List<String> QualitySettingList = new List<string>();

    private bool isDebugStage = true;
    private bool isShowAds = true;
    private StageData debugStageData = null;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 表示.
    /// </summary>
    public static DebugWindowView Show(DebugWindowView dialogPrefab)
    {
        var dialog = DialogManager.Instance.Show<DebugWindowView>(
            dialogPrefab,
            string.Empty,
            string.Empty,
            null,
            DialogManager.ButtonType.YES, DialogManager.ButtonType.NO
        );
        dialog.SetData();
        dialog.SetButtonText(DialogManager.ButtonType.NO, "x");
        return dialog;
    }

    public void SetData()
    {
        inGameVariant = GameVariant.Instance.Get<InGameVariant>();
        window.SetActive(true);
        QualitySettingSelectBox.Dropdown.ClearOptions();
        QualitySettingList = QualitySettings.names.Select(d => d.ToString()).ToList();
        for (int i = 0; i < QualitySettingList.Count; i++)
        {
            QualitySettingSelectBox.Dropdown.options.Add(new Dropdown.OptionData
            {
                text = QualitySettingList[i]
            });
        }
        QualitySettingSelectBox.Dropdown.onValueChanged.AddListener(delegate
        {
            OnValueChangedQualitySetting();
        });
        stageSelectBox.Dropdown.ClearOptions();
        for (int i = 0; i < debugStageDataList.Count; i++)
        {
            stageSelectBox.Dropdown.options.Add(new Dropdown.OptionData
            {
                text = debugStageDataList[i]
            });
        }
        stageSelectBox.Dropdown.onValueChanged.AddListener(delegate
        {
            OnValueChangedStageData();
        });
        isDebugStageButton.Buttton.onClick.AddListener(() =>
        {
            SetIsDebugStage(!isDebugStage);
        });
        isShowAdsButton.Buttton.onClick.AddListener(() =>
        {
            SetIsShowAds(!isShowAds);
        });

        progressDegreesInputField.InputField.onEndEdit.AddListener(UpdateProgressDegrees);
        currentMoneyNumInputField.InputField.onEndEdit.AddListener(UpdateCurrentMoney);
        totalMoneyNumInputField.InputField.onEndEdit.AddListener(UpdateTotalMoney);
        currentJewelNumInputField.InputField.onEndEdit.AddListener(UpdateCurrentJewel);

        Open();
    }

    /// <summary>
    /// 開く.
    /// </summary>
    public void Open()
    {
        saveData = UserDataProvider.Instance.GetSaveData();


        isDebugStage = inGameVariant.IsDebugStage;
        isShowAds = inGameVariant.IsShowAds;

        QualitySettingSelectBox.Dropdown.value = QualitySettings.GetQualityLevel();
        debugStageData = inGameVariant.SelectStageData;
        var debugStageName = debugStageData != null ? debugStageData.name : "";
        stageSelectBox.Dropdown.value = debugStageDataList.FindIndex(d => d == debugStageName);
        progressDegreesInputField.InputField.text = saveData.StageLevel.ToString();
        currentMoneyNumInputField.InputField.text = saveData.MoneyNum.ToString();
        totalMoneyNumInputField.InputField.text = saveData.TotalMoneyNum.ToString();
        currentJewelNumInputField.InputField.text = saveData.JewelNum.ToString();

        SetIsDebugStage(inGameVariant.IsDebugStage);
    }

    /// <summary>
    /// アバター変更.
    /// </summary>
    private void OnValueChangedQualitySetting()
    {
        var selectQualitySetting = QualitySettingList[QualitySettingSelectBox.Dropdown.value];
        saveData.GraphicLevel = QualitySettingSelectBox.Dropdown.value;
        saveData.Save();
        GraphicManager.Instance.SetGraphic(QualitySettingSelectBox.Dropdown.value);
    }

    ///// <summary>
    ///// ステージデバッグ設定.
    ///// </summary>
    private void SetIsDebugStage(bool enable)
    {
        isDebugStage = enable;
        isDebugStageButton.Text.text = (isDebugStage) ? "ON" : "OFF";
        inGameVariant.IsDebugStage = isDebugStage;
    }

    /// <summary>
    /// ステージデータ変更.
    /// </summary>
    private void OnValueChangedStageData()
    {
        debugStageData = AssetManager.Instance.LoadStageData(stageSelectBox.Dropdown.options[stageSelectBox.Dropdown.value].text);
        inGameVariant.SelectStageData = debugStageData;
        Debug.Log($"{debugStageData.StageName}をデバッグから選択");
    }

    /// <summary>
    /// ステージ進行度更新.
    /// </summary>
    private void UpdateProgressDegrees(string text)
    {
        if (int.TryParse(text, out int stageLevel))
        {
            saveData.StageLevel = stageLevel;
            saveData.TotalStageLevel = stageLevel;
            saveData.Save();
        }
        progressDegreesInputField.InputField.text = stageLevel.ToString();
    }

    /// <summary>
    /// コイン数変更.
    /// </summary>
    private void UpdateCurrentMoney(string text)
    {
        if (int.TryParse(text, out int moneyNum))
        {
            saveData.MoneyNum = moneyNum;
            saveData.Save();
        }
        currentMoneyNumInputField.InputField.text = moneyNum.ToString();
    }

    /// <summary>
    /// 累計コイン数変更.
    /// </summary>
    private void UpdateTotalMoney(string text)
    {
        if (int.TryParse(text, out int moneyNum))
        {
            saveData.TotalMoneyNum = moneyNum;
            saveData.Save();
        }
        totalMoneyNumInputField.InputField.text = moneyNum.ToString();
    }

    /// <summary>
    /// ジュエル数変更.
    /// </summary>
    private void UpdateCurrentJewel(string text)
    {
        if (int.TryParse(text, out int jewelNum))
        {
            saveData.JewelNum = jewelNum;
            saveData.Save();
        }
        currentJewelNumInputField.InputField.text = jewelNum.ToString();
    }

    ///// <summary>
    ///// 広告表示設定.
    ///// </summary>
    private void SetIsShowAds(bool enable)
    {
        isShowAds = enable;
        isShowAdsButton.Text.text = (isShowAds) ? "ON" : "OFF";
        inGameVariant.IsShowAds = isShowAds;
        AdsManager.Instance.SetIsShowAds(isShowAds);
    }
    //================================================================================
    // UIイベント
    //================================================================================
    public void TestRewardAds()
    {
        AdsManager.Instance.LoadRewardAd(() =>
        {
            Debug.Log("リワード広告完了");
        }, () =>
        {
            Debug.Log("リワード広告失敗");
        });
    }

    public void TestInterstitialAds()
    {
        AdsManager.Instance.LoadInterstitialAd();
        AdsManager.Instance.ShowInterstitialAd(() =>
        {
            Debug.Log("強制広告完了");
        }, () =>
        {
            Debug.Log("強制広告失敗");
        });
    }

    public void TestBannerAds()
    {
        AdsManager.Instance.LoadBanner();
    }

    public void LaunchTestSuite()
    {
        AdsManager.Instance.LaunchTestSuite();
    }

    /// <summary>
    /// データリセットボタン押下時.
    /// </summary>
    public void OnClickDataResetButton()
    {
        saveData.Init();
        saveData.Save();
    }

    /// <summary>
    /// リロードボタン押下時.
    /// </summary>
    public void OnClickReloadScene()
    {
        Fade.FadeOut(() =>
        {
            DialogManager.Instance.Dismiss<DebugWindowView>(this, null);
            SceneManager.LoadScene(GameConstant.Scene.Title.ToString());
        }, duration: 0.5f);
    }
}