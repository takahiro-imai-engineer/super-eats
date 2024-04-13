using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleStageView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject stageRoot;
    [SerializeField] private Light light1;
    [SerializeField] private Light light2;
    [SerializeField] private List<string> backGroundNameList;
    //================================================================================
    // ローカル
    //================================================================================
    private int currentBaseIndex = 0;
    private TitleBaseView baseView = null;
    private List<(int, int)> baseInfo = new List<(int, int)>
    {
        (0, 0),
        (0, 1),
        (0, 2),
        (1, 0),
        (1, 1),
        (1, 2),
        (2, 0),
        (2, 1),
        (2, 2),
        (3, 0),
        (3, 1),
        (3, 2),
        (3, 3),
    };

    //================================================================================
    // メソッド
    //================================================================================
    public void Init()
    {
        foreach (Transform item in stageRoot.transform)
        {
            Destroy(item.gameObject);
        }
        int baseLevel = UserDataProvider.Instance.GetSaveData().BaseLevel;
        int step;
        (currentBaseIndex, step) = GetBaseInfo(baseLevel);
        var titleBase = AssetManager.Instance.LoadBase(backGroundNameList[currentBaseIndex]);
        baseView = Instantiate(titleBase, stageRoot.transform);
        baseView.Init(step);
        // NOTE: 2023/02ワールドによってライティングが変わる仕様はオミット
        // SetUpLighting(currentStageWorldData.GraphicData);
    }

    public void Show()
    {
        stageRoot.SetActive(true);
    }

    public void Hide()
    {
        stageRoot.SetActive(false);
    }

    public void LevelUpBase(int baseLevel, UnityAction onCompleteCallback)
    {
        (int baseIndex, int step) = GetBaseInfo(baseLevel);
        if (baseIndex != currentBaseIndex)
        {
            // 拠点が変わった時は切り替え演出
            onCompleteCallback?.Invoke();
            Destroy(baseView.gameObject);
            var titleBase = AssetManager.Instance.LoadBase(backGroundNameList[baseIndex]);
            baseView = Instantiate(titleBase, stageRoot.transform);
            baseView.Init(step);
            currentBaseIndex = baseIndex;
            return;
        }
        baseView.ShowLevelUp(step, onCompleteCallback);
    }

    (int, int) GetBaseInfo(int baseLevel)
    {
        if (baseLevel < 1 || baseLevel > baseInfo.Count)
        {
            Debug.LogWarning($"BaseLevelが範囲外です. baseLevel={baseLevel}");
            return baseInfo[baseInfo.Count - 1];
        }
        return baseInfo[baseLevel - 1];
    }
}