using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using template;

public class TitleOrderListView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject root;
    [SerializeField] private List<TitleOrderListItemView> titleOrderListItemViewList;
    //================================================================================
    // メソッド
    //================================================================================
    public void Init()
    {
        root.SetActive(false);
        foreach (var item in titleOrderListItemViewList)
        {
            item.Init();
        }
    }

    public void SetStageData(List<StageData> selectableStageDataList)
    {
        for (int i = 0; i < selectableStageDataList.Count; i++)
        {
            titleOrderListItemViewList[i].SetStageData(selectableStageDataList[i]);
        }
    }

    public void Show()
    {
        root.SetActive(true);
        SoundManager.Instance.Play<SEContainer>(SEName.SE_ORDER);
        float SLIDE_IN_DELAY_TIME = 0.25f;
        for (int i = 0; i < titleOrderListItemViewList.Count; i++)
        {
            titleOrderListItemViewList[i].Show();
            titleOrderListItemViewList[i].SlideIn(SLIDE_IN_DELAY_TIME * i);
        }
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
