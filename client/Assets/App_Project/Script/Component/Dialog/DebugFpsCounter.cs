using System.Collections;
using System.Collections.Generic;
using app_system;
// using CodeStage.AdvancedFPSCounter;
using UnityEngine;

public class DebugFpsCounter : MonoBehaviourSingleton<DebugFpsCounter>
{

    //================================================================================
    // インスペクタ
    //================================================================================
    // 基準とする解像度
    [Header("基準となる解像度")]
    [SerializeField] private Vector2 guiScreenSize = new Vector2(1080, 1920);
    [Header("FPSデバッグボタン表示するか")]
    [SerializeField] private bool isShowFPSDebugButon = false;
    //================================================================================
    // ローカル
    //================================================================================
    // デバッグタイプ
    private DebugType debugType = DebugType.Disable;
    // NOTE: 有料アセットのため、パブリックリポジトリではコメントアウト
    // private AFPSCounter fPSCounter;

    private enum DebugType
    {
        Disable = 0,
        FPSOnly,
        AllInfo,
    }

    //================================================================================
    // メソッド
    //================================================================================

    private void Start()
    {
        // fPSCounter = gameObject.GetComponent<AFPSCounter>();
        // fPSCounter.OperationMode = OperationMode.Disabled;
    }

    void OnGUI()
    {
        if (!isShowFPSDebugButon)
        {
            return;
        }

        // GUI用の解像度設定
        float scale = (Screen.width > Screen.height) ? Screen.width / guiScreenSize.x : Screen.height / guiScreenSize.y;
        GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);

        // ボタン
        if (GUI.Button(new Rect(50, 50, 100, 100), "FPS"))
        {
            if (debugType == DebugType.Disable)
            {
                debugType = DebugType.FPSOnly;
                // fPSCounter.OperationMode = OperationMode.Normal;
                // fPSCounter.deviceInfoCounter.Enabled = false;
            }
            else if (debugType == DebugType.FPSOnly)
            {
                debugType = DebugType.AllInfo;
                // fPSCounter.OperationMode = OperationMode.Normal;
                // fPSCounter.deviceInfoCounter.Enabled = true;
            }
            else if (debugType == DebugType.AllInfo)
            {
                debugType = DebugType.Disable;
                // fPSCounter.OperationMode = OperationMode.Disabled;
                // fPSCounter.deviceInfoCounter.Enabled = false;
            }
        }

        // GUIの解像度を元に戻す
        GUI.matrix = Matrix4x4.identity;
    }
}