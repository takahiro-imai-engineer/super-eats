using UnityEngine;
using UnityEngine.UI;
public class LifeIconView : MonoBehaviour {
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init () { }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show () {
        icon.sprite = activeSprite;
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide () {
        icon.sprite = inactiveSprite;
    }
}