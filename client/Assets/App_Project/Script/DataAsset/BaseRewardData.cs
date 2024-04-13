using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 拠点報酬データ
/// </summary>
[CreateAssetMenu]
public class BaseRewardData : ScriptableObject
{
    [SerializeField] private List<BaseLevelUpReward> baseLevelUpRewardList;
    [System.Serializable]
    public class BaseLevelUpReward
    {
        public int StageLevel;
        public int BaseLevel;
        public int BaseGroupId;
        public int BaseStep;
        public Sprite FurnitureSprite;
    }

    public List<BaseLevelUpReward> BaseLevelUpRewardList => baseLevelUpRewardList;

    public List<BaseLevelUpReward> GetCurrentRewardList(int baseLevel)
    {
        List<BaseLevelUpReward> currentBaseLevelUpRewardList = new();
        int baseGroupId = baseLevelUpRewardList.FirstOrDefault(d => d.BaseLevel == baseLevel).BaseGroupId;
        currentBaseLevelUpRewardList.AddRange(baseLevelUpRewardList.FindAll(d => d.BaseGroupId == baseGroupId));
        if (baseGroupId != InGameConstant.LastBaseGroupId)
        {
            currentBaseLevelUpRewardList.Add(baseLevelUpRewardList.FirstOrDefault(d => d.BaseGroupId == baseGroupId + 1 && d.BaseStep == 0));
        }
        currentBaseLevelUpRewardList.OrderBy(d => d.BaseLevel);

        return currentBaseLevelUpRewardList;
    }
}