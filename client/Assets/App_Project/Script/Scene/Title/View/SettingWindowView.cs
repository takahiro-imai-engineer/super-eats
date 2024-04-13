using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using template;
using TMPro;
using UnityEngine.Events;

public class SettingWindowView : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite inactiveButtonSprite;
    [SerializeField] private SettingButton bgmButton;
    [SerializeField] private SettingButton seButton;
    [SerializeField] private SettingToggle frameRateToggle;

    [System.Serializable]
    class SettingButton
    {
        public Button Button;
        public TextMeshProUGUI Text;
    }

    [System.Serializable]
    class SettingToggle
    {
        public Toggle Toggle1;
        public Toggle Toggle2;
    }
    //================================================================================
    // ローカル
    //================================================================================
    /// <summary>セーブデータ</summary>
    private SaveData saveData;
    //================================================================================
    // メソッド
    //================================================================================

    public void SetData()
    {
        saveData = UserDataProvider.Instance.GetSaveData();
        UpdateSetting();
    }

    public void UpdateSetting()
    {
        bgmButton.Text.text = saveData.IsMuteBgm ? "OFF" : "ON";
        bgmButton.Button.GetComponent<Image>().sprite = saveData.IsMuteBgm ? inactiveButtonSprite : activeButtonSprite;
        seButton.Text.text = saveData.IsMuteSe ? "OFF" : "ON";
        seButton.Button.GetComponent<Image>().sprite = saveData.IsMuteSe ? inactiveButtonSprite : activeButtonSprite;
        frameRateToggle.Toggle1.isOn = !saveData.IsFrameRate60;
        frameRateToggle.Toggle2.isOn = saveData.IsFrameRate60;
    }

    public void OnClickSeSetting()
    {
        saveData.IsMuteSe = !saveData.IsMuteSe;
        SoundManager.Instance.SetMute<SEContainer>(saveData.IsMuteSe);
        UserDataProvider.Instance.WriteSaveData();
        UpdateSetting();
    }

    public void OnClickBgmSetting()
    {
        saveData.IsMuteBgm = !saveData.IsMuteBgm;
        SoundManager.Instance.SetMute<BGMContainer>(saveData.IsMuteBgm);
        UserDataProvider.Instance.WriteSaveData();
        UpdateSetting();
    }

    public void OnClickFrameRate30Setting(Toggle toggle)
    {
        if(!toggle.isOn)
        {
            UpdateSetting();
            return;
        }
        saveData.IsFrameRate60 = !toggle.isOn;
        GameRegistry.SetFrameRate(!toggle.isOn ? 60 : 30);
        UserDataProvider.Instance.WriteSaveData();
        UpdateSetting();
    }

    public void OnClickFrameRate60Setting(Toggle toggle)
    {
        if(!toggle.isOn)
        {
            UpdateSetting();
            return;
        }
        saveData.IsFrameRate60 = toggle.isOn;
        GameRegistry.SetFrameRate(toggle.isOn ? 60 : 30);
        UserDataProvider.Instance.WriteSaveData();
        UpdateSetting();
    }

    public void OnClickPrivacyPolicy()
    {
        Application.OpenURL(GameConstant.PRIVACY_POLICY_URL);
    }

    public void OnClickLicense()
    {
        DialogManager.Instance.Show<LicenseWindowView>(
            string.Empty,
            string.Empty,
            null,
            DialogManager.ButtonType.YES
        ).SetData();
    }
}
