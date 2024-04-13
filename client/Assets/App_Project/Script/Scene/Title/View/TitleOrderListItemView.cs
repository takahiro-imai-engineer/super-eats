using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TitleOrderListItemView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private RectTransform root;
    [SerializeField] private GameObject normalRoot;
    [SerializeField] private Button normalButton;
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private List<Image> normalLevelStarIconList;
    [SerializeField] private GameObject specialRoot;
    [SerializeField] private Button specialButton;
    [SerializeField] private List<Image> specialLevelStarIconList;

    //================================================================================
    // 定数
    //================================================================================
    private readonly float INIT_OFFSET_POSITION_X = 700f;
    private readonly float SLIDE_IN_TIME = 1.0f;
    //================================================================================
    // メソッド
    //================================================================================
    public void Init()
    {
        normalRoot.SetActive(false);
        foreach (var item in normalLevelStarIconList)
        {
            item.gameObject.SetActive(false);
        }
        specialRoot.SetActive(false);
        foreach (var item in specialLevelStarIconList)
        {
            item.gameObject.SetActive(false);
        }

        root.anchoredPosition = new Vector2(INIT_OFFSET_POSITION_X, root.anchoredPosition.y);
        Hide();
    }

    /// <summary>
    /// データを設定
    /// </summary>
    /// <param name="stageData"></param>
    public void SetStageData(StageData stageData)
    {
        if (stageData.IsBonusStage)
        {
            specialRoot.SetActive(true);
            for (int i = 0; i < stageData.Difficulty; i++)
            {
                specialLevelStarIconList[i].gameObject.SetActive(true);
            }
        }
        else
        {
            normalRoot.SetActive(true);
            characterIcon.sprite = AssetManager.Instance.LoadCharacterIconSprite(stageData.OrdererData.IconName);
            characterName.text = stageData.OrdererData.Name;
            for (int i = 0; i < stageData.Difficulty; i++)
            {
                normalLevelStarIconList[i].gameObject.SetActive(true);
            }
        }
    }

    public void Show()
    {
        root.gameObject.SetActive(true);
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }

    public void SlideIn(float delayTime)
    {
        root.DOAnchorPos(new Vector2(0, root.anchoredPosition.y), SLIDE_IN_TIME).SetDelay(delayTime);
    }
}
