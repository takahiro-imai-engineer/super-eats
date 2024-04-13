using template;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpNextStageScene : SceneBase
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private bool isDebugEnding = false;
    [SerializeField] private WarpNextStageView warpNextStageView;

    //================================================================================
    // メソッド
    //================================================================================
    void Start()
    {
        Init();
    }

    private void Init()
    {
        SoundManager.Instance.Stop<BGMContainer>();
        warpNextStageView.Init();
        Fade.FadeIn(() =>
        {
            if (GameVariant.Instance.Get<InGameVariant>().IsShowEnding || isDebugEnding)
            {
                // 最終ステージクリアなら、エンディング
                ShowEnding();
            }
            else
            {
                SoundManager.Instance.Play<BGMContainer>(BGMName.BGM_WARP_HOLE);
                warpNextStageView.MovePlayer(ToTitleScene);
            }
        }, duration: 1f);
    }

    private void ShowEnding()
    {
        SoundManager.Instance.Play<BGMContainer>(BGMName.BGM_ENDING);
        warpNextStageView.ShowEnding(() =>
        {
            warpNextStageView.MovePlayer(ToTitleScene);
        });
    }

    private void ToTitleScene()
    {
        GameVariant.Instance.Get<InGameVariant>().IsEndWarpStage = true;
        GameVariant.Instance.Get<InGameVariant>().IsShowEnding = false;
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(GameConstant.Scene.Title.ToString());
        asyncLoadScene.allowSceneActivation = false;
        SoundManager.Instance.CrossFade<BGMContainer>(null, null, 0.5f);
        Fade.WhiteOut(() =>
        {
            asyncLoadScene.allowSceneActivation = true;
        }, duration: 0.5f);
    }
}
