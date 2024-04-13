using UnityEngine;
using UnityEngine.UI;
using template;
using DG.Tweening;
using TMPro;

public class TitleTownView : MonoBehaviour
{
    //================================================================================
    // インスペクタ
    //================================================================================
    [SerializeField] private GameObject currentTownRoot;
    [SerializeField] private Image currentTownIcon;
    [SerializeField] private TextMeshProUGUI currentTownNameText;
    [SerializeField] private GameObject nextTownRoot;
    [SerializeField] private Image nextTownIcon;
    [SerializeField] private TextMeshProUGUI nextTownNameText;
    [SerializeField] private Image stageProgressGauge;
    [SerializeField] private RectTransform gaugeOutlineRectTransform;
    [SerializeField] private SpriteAnimation spriteAnimation;
    [SerializeField] private TextMeshProUGUI getMoneyText;
    [SerializeField] private GameObject nextTownGaugeRoot;
    [SerializeField] private Sprite lastStageSprite;
    [SerializeField] private string lastStageName = "???";

    //================================================================================
    // ローカル
    //================================================================================
    private int maxMoneyNum = 0;

    //================================================================================
    // 定数
    //================================================================================
    private static readonly Vector2 MAX_GAUGE_SIZE = new Vector2(156, 30);

    //================================================================================
    // メソッド
    //================================================================================
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        currentTownRoot.SetActive(false);
        nextTownRoot.SetActive(false);
        stageProgressGauge.fillAmount = 0f;
        gaugeOutlineRectTransform.gameObject.SetActive(false);
        gaugeOutlineRectTransform.anchoredPosition = Vector2.zero;
        gaugeOutlineRectTransform.sizeDelta = Vector2.zero;
        maxMoneyNum = 0;
        spriteAnimation.Init();
        getMoneyText.gameObject.SetActive(false);
    }

    /// <summary>
    /// ワールドステージ情報設定
    /// </summary>
    /// <param name="currenStageWorldData"></param>
    /// <param name="nextStageWorldData"></param>
    public void SetStageData(StageWorldData currenStageWorldData, StageWorldData nextStageWorldData, int totalMoneyNum)
    {
        currentTownRoot.SetActive(true);
        if (currenStageWorldData)
        {
            currentTownIcon.sprite = currenStageWorldData.StageIcon;
            currentTownNameText.text = currenStageWorldData.StageGroupName;
            maxMoneyNum = currenStageWorldData.NextWorldNecessaryTotalMoneyNum;
            UpdateProgress(totalMoneyNum);
        }

        if (nextStageWorldData != null)
        {
            nextTownRoot.SetActive(true);
            nextTownIcon.sprite = nextStageWorldData.StageIcon;
            nextTownNameText.text = nextStageWorldData.StageGroupName;
            nextTownGaugeRoot.SetActive(true);
        }
        else
        {
            nextTownRoot.SetActive(true);
            nextTownNameText.text = lastStageName;
            nextTownIcon.sprite = lastStageSprite;
            // 最後のステージの場合
            nextTownGaugeRoot.SetActive(false);
        }
    }

    /// <summary>
    /// 進捗ゲージ更新
    /// </summary>
    /// <param name="totalMoneyNum"></param>
    public void UpdateProgress(int totalMoneyNum)
    {
        float progressRate = (float)totalMoneyNum / (float)maxMoneyNum;
        stageProgressGauge.fillAmount = progressRate;
        gaugeOutlineRectTransform.anchoredPosition = new Vector2(-(MAX_GAUGE_SIZE.x - MAX_GAUGE_SIZE.x * progressRate) / 2f, 0);
        gaugeOutlineRectTransform.sizeDelta = new Vector2(MAX_GAUGE_SIZE.x * progressRate, MAX_GAUGE_SIZE.y);
    }

    public void ShowMaxProgress()
    {
        SoundManager.Instance.Play<SEContainer>(SEName.SE_MAX_GAUGE);
        // ゲージを白と黄色で明滅
        stageProgressGauge.DOColor(Color.white, 0.25f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutElastic)
            .SetLink(gameObject);
        // エフェクト発生
        spriteAnimation.Show();
    }

    public Tweener ShowGetMoneyTweener(int getMoneyNum)
    {
        var getMoneyRectTransform = getMoneyText.GetComponent<RectTransform>();
        getMoneyText.gameObject.SetActive(true);
        getMoneyText.text = $"+{getMoneyNum}";
        return getMoneyRectTransform
            .DOLocalMove(Vector3.up * 200f, 1f)
            .SetRelative()
            .OnComplete(() =>
            {
                getMoneyText.gameObject.SetActive(false);
            });
    }

    public void ShowGaugeEffect()
    {
        gaugeOutlineRectTransform.gameObject.SetActive(true);
    }

    public void HideGaugeEffect()
    {
        gaugeOutlineRectTransform.gameObject.SetActive(false);
    }
}
