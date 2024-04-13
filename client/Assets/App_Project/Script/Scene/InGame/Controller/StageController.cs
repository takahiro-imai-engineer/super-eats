using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmazingAssets.CurvedWorld;
using AmazingAssets.CurvedWorld.Example;
using UnityEngine;

/// <summary>
/// ステージ管理クラス
/// </summary>
public class StageController : MonoBehaviour
{

    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject stageRoot;
    [SerializeField] private Light light1;
    [SerializeField] private Light light2;
    [SerializeField] private List<StageData> stageDataList;
    [Header("DebugStageIndexで指定しているステージをプレイするか")]
    [SerializeField] private bool isDebugStage = false;
    [SerializeField] private int debugStageIndex = 0;
    [Header("ステージを繰り返しプレイするか")]
    [SerializeField] private bool isDebugRepeatStage = false;

    //================================================================================
    // ローカル
    //================================================================================
    private StageData currentStageData;
    private int currentTipIndex;
    [NonReorderable] private List<GameObject> generatedStageList = new List<GameObject>();
    private GameObject playerFollowObject = null;
    [NonReorderable] private List<MovingGimick> poolMoveGimickList = new List<MovingGimick>();

    private readonly int PRE_INSTANTIATE = 2;
    //================================================================================
    // プロパティ
    //================================================================================
    public StageData CurrentStageData => currentStageData;
    public int StageCount => stageDataList.Count;
    public bool IsDebugRepeatStage => isDebugRepeatStage;
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        if (generatedStageList != null && generatedStageList.Count > 0)
        {
            foreach (var stage in generatedStageList)
            {
                Destroy(stage);
            }
            foreach (var moveGimick in poolMoveGimickList)
            {
                if (moveGimick != null)
                {
                    Destroy(moveGimick.gameObject);
                }
            }
            generatedStageList = new List<GameObject>();
            poolMoveGimickList = new List<MovingGimick>();
        }
        var inGameVariant = GameVariant.Instance.Get<InGameVariant>();
        if (isDebugStage)
        {
            currentStageData = stageDataList[debugStageIndex];
            inGameVariant.SelectGraphicData = StageManager.Instance.GetGraphicData(currentStageData.GroupId);
        }
        else if (inGameVariant.SelectStageData != null)
        {
            currentStageData = inGameVariant.SelectStageData;
        }
        else
        {
            Debug.LogError("StageDataがnullです。");
            return;
        }
        Debug.Log($"{currentStageData.Id}:{currentStageData.StageName}");

        // NOTE: 自機を追従するオブジェクトを生成
        if (currentStageData.PlayerFollowObject != null)
        {
            Destroy(playerFollowObject);
            playerFollowObject = Instantiate(currentStageData.PlayerFollowObject);
            playerFollowObject.transform.parent = stageRoot.transform;
        }

        // NOTE: CurvedWorldのオンオフ
        if (CurvedWorldController.Instance != null)
        {
            CurvedWorldController.Instance.enabled = currentStageData.IsCurvedWorld;
            CurvedWorldController.Instance.bendHorizontalSize = currentStageData.HorizontalSize;
            CurvedWorldController.Instance.bendHorizontalOffset = currentStageData.HorizontalOffset;
            CurvedWorldController.Instance.bendVerticalSize = currentStageData.VerticalSize;
            CurvedWorldController.Instance.bendVerticalOffset = currentStageData.VerticalOffset;
        }
        currentTipIndex = 0;
        // var startStage = GenerateStage(0);
        // generatedStageList.Add(startStage);
        // _UpdateStage(PRE_INSTANTIATE);
        GenerateAllStage();

        SetUpLighting(inGameVariant.SelectGraphicData);
    }

    private void SetUpLighting(GraphicData graphicData)
    {
        if (graphicData == null)
        {
            Debug.LogWarning("グラフィック情報がnullです。");
            return;
        }
        if (light1 != null)
        {
            // NOTE: ライト1設定
            light1.enabled = graphicData.LightSetting1.isActive;
            light1.transform.rotation = Quaternion.Euler(graphicData.LightSetting1.lightAngle);
            light1.color = graphicData.LightSetting1.lightColor;
            light1.intensity = graphicData.LightSetting1.lightIntensity;
            light1.intensity = graphicData.LightSetting1.lightIntensity;
        }
        if (light2 != null)
        {
            // NOTE: ライト2設定
            light2.enabled = graphicData.LightSetting2.isActive;
            light2.transform.rotation = Quaternion.Euler(graphicData.LightSetting2.lightAngle);
            light2.color = graphicData.LightSetting2.lightColor;
            light2.intensity = graphicData.LightSetting2.lightIntensity;
            light2.intensity = graphicData.LightSetting2.lightIntensity;
        }
        // NOTE: Skybox設定
        RenderSettings.skybox = AssetManager.Instance.LoadSkyboxMaterial(graphicData.SkyboxMaterialName);
        // NOTE: 環境光設定
        RenderSettings.subtractiveShadowColor = graphicData.RealtimeShadowColor;
        RenderSettings.ambientLight = graphicData.EnvironmentAmbientLight;
        // NOTE: フォグ設定
        RenderSettings.fogColor = graphicData.FogColor;
        RenderSettings.fogDensity = graphicData.FogDensity;
    }

    /// <summary>
    /// ステージ更新
    /// </summary>
    /// <param name="playerPosition"></param>
    public void UpdateStage(Vector3 playerPosition)
    {
        // キャラクターの位置によって現在のステージチップのインデックスを計算
        int playerPositionIndex = (int)(playerPosition.z / currentStageData.StageTipSize);

        if (playerFollowObject != null)
        {
            playerFollowObject.transform.position = new Vector3(0, 0, playerPosition.z);
        }

        // 次のステージチップに入ったらステージの更新処理を行う
        if (currentTipIndex >= currentStageData.StageList.Count)
        {
            return;
        }
        // NOTE: 最初に全てのステージチップを生成するようにしたので、これ以降の条件は入らない
        else if (currentTipIndex == currentStageData.StageList.Count - 1)
        {
            // ステージを生成しきったら、ゴール生成
            currentTipIndex = currentStageData.StageList.Count;
            GenerateGoal();
        }
        else if (playerPositionIndex + PRE_INSTANTIATE > currentTipIndex)
        {
            _UpdateStage(playerPositionIndex + PRE_INSTANTIATE);
        }
    }

    /// <summary>
    /// ステージ更新
    /// </summary>
    /// <param name="toTipIndex"></param>
    private void _UpdateStage(int toTipIndex)
    {
        toTipIndex =
            toTipIndex < currentStageData.StageList.Count ?
            toTipIndex : currentStageData.StageList.Count - 1;
        if (toTipIndex <= currentTipIndex)
            return;

        // 指定のステージチップまで生成
        for (int i = currentTipIndex + 1; i <= toTipIndex; i++)
        {
            GameObject stageObject = GenerateStage(i);

            // 生成したステージチップを管理リストに追加
            generatedStageList.Add(stageObject);
        }
        // ステージ保持上限になるまで古いステージを削除
        while (generatedStageList.Count > PRE_INSTANTIATE + 2)
        {
            DestroyOldestStage();
        }

        currentTipIndex = toTipIndex;
    }

    /// <summary>
    /// 指定のインデックス位置にstageオブジェクトをランダムに生成
    /// </summary>
    /// <param name="tipIndex"></param>
    /// <returns></returns>
    private GameObject GenerateStage(int tipIndex)
    {
        GameObject stageObject = (GameObject)Instantiate(currentStageData.StageList[tipIndex],
            new Vector3(0, 0, tipIndex * currentStageData.StageTipSize), Quaternion.identity);

        stageObject.transform.parent = stageRoot.transform;
        // NOTE: TransformDynamicPositionにCurvedWorldをアタッチ
        if (currentStageData.IsCurvedWorld)
        {
            var transformDynamicPositionList = stageObject.GetComponentsInChildren<TransformDynamicPosition>(true);
            foreach (var item in transformDynamicPositionList)
            {
                item.curvedWorldController = CurvedWorldController.Instance;
                // Debug.Log (item.transform);
            }
        }
        // ステージと一緒に削除されないように対応
        // var movingGimickList = stageObject.GetComponentsInChildren<MovingGimick>();
        // foreach (var item in movingGimickList)
        // {
        //     if (!item.IsLoop)
        //     {
        //         item.transform.parent = stageRoot.transform;
        //         poolMoveGimickList.Add(item);
        //     }
        // }

        return stageObject;
    }

    /// <summary>
    /// ゴール生成
    /// </summary>
    public void GenerateGoal()
    {
        GameObject stageObject = (GameObject)Instantiate(currentStageData.GoalStage,
            new Vector3(0, 0, currentTipIndex * currentStageData.StageTipSize), Quaternion.identity);

        stageObject.transform.parent = stageRoot.transform;
        // NOTE: 食べ物に自機の情報を渡す
        var playerInfo = GameVariant.Instance.Get<InGameVariant>().InGameModel.PlayerInfo;
        var foodViewList = stageObject.GetComponentsInChildren<FoodView>();
        foreach (var item in foodViewList)
        {
            item.SetPlayerInfo(playerInfo);
        }
        // NOTE: TransformDynamicPositionにCurvedWorldをアタッチ
        if (currentStageData.IsCurvedWorld)
        {
            var transformDynamicPositionList = stageObject.GetComponentsInChildren<TransformDynamicPosition>(true);
            foreach (var item in transformDynamicPositionList)
            {
                item.curvedWorldController = CurvedWorldController.Instance;
                // Debug.Log (item.transform);
            }
        }
        generatedStageList.Add(stageObject);
    }

    /// <summary>
    /// 一番古いステージを削除
    /// </summary>
    private void DestroyOldestStage()
    {
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt(0);
        Destroy(oldStage);
    }


    private void GenerateAllStage()
    {
        for (int i = 0; i < currentStageData.StageList.Count; i++)
        {
            GameObject stageObject = GenerateStage(i);

            // 生成したステージチップを管理リストに追加
            generatedStageList.Add(stageObject);
        }
        currentTipIndex = currentStageData.StageList.Count;
        GenerateGoal();
    }
}