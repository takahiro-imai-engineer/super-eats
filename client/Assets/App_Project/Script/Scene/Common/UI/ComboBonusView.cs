
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboBonusView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI bonusText;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="continuousSuccessfulCount"></param>
    public void SetComboBonus(int continuousSuccessfulCount)
    {
        if (continuousSuccessfulCount < 3)
        {
            comboText.text = $"{continuousSuccessfulCount}";
        }
        else
        {
            comboText.text = "Max";
        }
        
        // NOTE: コンボ仕様はオミット
        // float comboBonusRate = InGameModel.GetComboBonus(continuousSuccessfulCount);
        float comboBonusRate = 0f;

        bonusText.text = $"{(comboBonusRate):F1}";
    }
}
