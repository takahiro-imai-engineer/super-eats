using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using template;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleBaseProgressView : MonoBehaviour
{
    [SerializeField] private Image progressGauge;
    [SerializeField] private BaseRewardIcon firstBaseReward;
    [SerializeField] private BaseRewardIcon secondBaseReward;
    [SerializeField] private BaseRewardIcon thirdBaseReward;

    private float afterProgressValue = 0f;

    [System.Serializable]
    private class BaseRewardIcon
    {
        public RectTransform RewardIcon;
        public GameObject PresentIconRoot;
        public GameObject HomeIconRoot;
        public TextMeshProUGUI LevelText;

        public void Init(BaseRewardData.BaseLevelUpReward baseLevelUpReward, bool isObtained)
        {
            if (baseLevelUpReward == null)
            {
                // Debug.LogError("baseLevelUpReward not found.");
                RewardIcon.gameObject.SetActive(false);
                LevelText.text = string.Empty;
                return;
            }
            RewardIcon.gameObject.SetActive(!isObtained);
            if (PresentIconRoot != null)
            {
                PresentIconRoot.SetActive(!isObtained && baseLevelUpReward.BaseStep != 0);
            }
            if (HomeIconRoot != null)
            {
                HomeIconRoot.SetActive(!isObtained && baseLevelUpReward.BaseStep == 0);
            }
            LevelText.text = $"Lv.{baseLevelUpReward.StageLevel}";
        }
    }

    public void Init(int baseLevel, int currentStageLevel, List<BaseRewardData.BaseLevelUpReward> currentRewardList, bool isClearStage)
    {
        float progressValue = 0f;
        afterProgressValue = 0f;
        float beforeStageLevel = currentRewardList.FirstOrDefault().BaseLevel;
        var baseRewardIcons = new BaseRewardIcon[4]
        {
            null,
            firstBaseReward,
            secondBaseReward,
            thirdBaseReward
        };
        for (int i = 1; i < baseRewardIcons.Length; i++)
        {
            var currentReward = currentRewardList[i];
            baseRewardIcons[i].Init(currentReward, baseLevel >= currentReward?.BaseLevel);
        }
        progressValue = GetProgressValue(currentStageLevel - 1, currentRewardList);
        afterProgressValue = GetProgressValue(currentStageLevel, currentRewardList);
        progressGauge.fillAmount = isClearStage ? progressValue : afterProgressValue;
    }

    public void UpdateProgress(UnityAction onCompleteCallback)
    {
        if (progressGauge.fillAmount == afterProgressValue)
        {
            onCompleteCallback.Invoke();
            return;
        }
        float tempProgressValue = progressGauge.fillAmount;
        const float PROGRESS_GAUGE_DURATION = 0.5f;
        SoundManager.Instance.Play<SEContainer>(SEName.SE_PROGRESS_GAUGE);
        progressGauge.DOFillAmount(afterProgressValue, PROGRESS_GAUGE_DURATION)
        .OnComplete(() =>
        {
            SoundManager.Instance.Stop<SEContainer>(SEName.SE_PROGRESS_GAUGE);
            onCompleteCallback.Invoke();
        });
    }

    public void ShowBaseEarnStaging(BaseRewardData.BaseLevelUpReward clearReward, UnityAction onCompleteCallback)
    {
        // 画面の重心にプレゼントアイコンを移動する
        var moveBaseReward = clearReward.BaseStep switch
        {
            0 => thirdBaseReward,
            1 => firstBaseReward,
            2 => secondBaseReward,
            3 => thirdBaseReward,
            _ => null
        };
        if (moveBaseReward == null)
        {
            onCompleteCallback.Invoke();
            return;
        }
        var prevPosition = moveBaseReward.RewardIcon.transform.position;
        moveBaseReward.RewardIcon
            .DOScale(3f, 1f);
        moveBaseReward.RewardIcon
            .DOMove(new Vector3(transform.position.x, 1f, transform.position.z), 1f)
            .OnComplete(() =>
            {
                moveBaseReward.RewardIcon.gameObject.SetActive(false);
                moveBaseReward.RewardIcon.transform.localScale = Vector3.one;
                moveBaseReward.RewardIcon.transform.position = prevPosition;
                onCompleteCallback.Invoke();
            });
    }

    private float GetProgressValue(int stageLevel, List<BaseRewardData.BaseLevelUpReward> currentRewardList)
    {
        float progressValue = 0f;
        float beforeStageLevel = currentRewardList.FirstOrDefault().StageLevel;
        for (int i = 1; i < currentRewardList.Count; i++)
        {
            var currentReward = currentRewardList[i];
            if (currentReward == null)
            {
                continue;
            }
            if (stageLevel >= currentReward.StageLevel)
            {
                progressValue += 1f / 3;
                beforeStageLevel = currentReward.StageLevel;
                continue;
            }
            if (currentReward.StageLevel != beforeStageLevel && stageLevel != beforeStageLevel)
            {
                float levelBetweenProgress = (1f / 3) - ((1f / 3) * (float)(currentReward.StageLevel - stageLevel) / (currentReward.StageLevel - beforeStageLevel));
                progressValue += levelBetweenProgress;
            }
            break;
        }
        return progressValue;
    }
}
