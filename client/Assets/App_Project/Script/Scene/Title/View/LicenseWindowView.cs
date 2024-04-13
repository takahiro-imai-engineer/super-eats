using UnityEngine;
using TMPro;

public class LicenseWindowView : GeneralDialog
{
    //================================================================================
    // インスペクタ
    //================================================================================ate TextMeshProUGUI _text;

    //================================================================================
    // メソッド
    //================================================================================
    public void SetData()
    {
        var licenseText = Resources.Load<TextAsset>("License");
        SetMessage(licenseText.text);
    }
}
