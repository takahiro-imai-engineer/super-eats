using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランダムにステージを生成し続けるクラス
/// NOTE: 現在は使用していません。
/// </summary>
public class RamdomStageController : MonoBehaviour {

    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject[] stageTips;
    public int startTipIndex;
    public int preInstantiate;
    //================================================================================
    // ローカル
    //================================================================================
    const int StageTipSize = 100;
    int currentTipIndex;
    [SerializeField] private List<GameObject> generatedStageList = new List<GameObject> ();
    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="selectStageId"></param>
    public void Init (int selectStageId) {
        currentTipIndex = startTipIndex - 1;
        _UpdateStage (preInstantiate);
    }

    /// <summary>
    /// ステージ更新
    /// </summary>
    /// <param name="playerPosition"></param>
    public void UpdateStage (Vector3 playerPosition) {
        // キャラクターの位置によって現在のステージチップのインデックスを計算
        int playerPositionIndex = (int) (playerPosition.z / StageTipSize);

        // 次のステージチップに入ったらステージの更新処理を行う
        if (playerPositionIndex + preInstantiate > currentTipIndex) {
            _UpdateStage (playerPositionIndex + preInstantiate);
        }
    }

    /// <summary>
    /// ステージ更新
    /// </summary>
    /// <param name="toTipIndex"></param>
    private void _UpdateStage (int toTipIndex) {
        if (toTipIndex <= currentTipIndex)
            return;

        // 指定のステージチップまで生成
        for (int i = currentTipIndex + 1; i <= toTipIndex; i++) {
            GameObject stageObject = GenerateStage (i);

            // 生成したステージチップを管理リストに追加
            generatedStageList.Add (stageObject);
        }
        // ステージ保持上限になるまで古いステージを削除
        while (generatedStageList.Count > preInstantiate + 2) {
            DestroyOldestStage ();
        }

        currentTipIndex = toTipIndex;
    }

    /// <summary>
    /// 指定のインデックス位置にstageオブジェクトをランダムに生成
    /// </summary>
    /// <param name="tipIndex"></param>
    /// <returns></returns>
    private GameObject GenerateStage (int tipIndex) {
        int nextStageTip = Random.Range (0, stageTips.Length);
        GameObject stageObject = (GameObject) Instantiate (stageTips[nextStageTip],
            new Vector3 (0, 0, tipIndex * StageTipSize), Quaternion.identity);

        return stageObject;
    }

    /// <summary>
    /// 一番古いステージを削除
    /// </summary>
    private void DestroyOldestStage () {
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt (0);
        Destroy (oldStage);
    }
}