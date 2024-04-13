using System.Collections.Generic;

namespace template
{
    /// <summary>
    /// セーブデータ
    /// </summary>
    [System.Serializable]
    public class SaveData : FileStorageObject<SaveData>
    {
        public int StageLevel = 1;
        public int TotalStageLevel = 1;

        /// <summary>強制広告用ステージプレイ回数</summary>
        public int InterstitialAdStagePlayCount = 0;

        /// <summary>全クリア回数</summary>
        public int AllClearCount = 0;

        /// <summary>拠点のレベル</summary>
        public int BaseLevel = 1;

        /// <summary>所持金数</summary>
        public int MoneyNum = 0;

        /// <summary>累計所持金数</summary>
        public int TotalMoneyNum = 0;

        /// <summary>ジュエル数</summary>
        public int JewelNum = 0;

        /// <summary>バッグ番号</summary>
        public int SelectBagId = 1;
        /// <summary>アバター番号</summary>
        public int SelectAvatarId = 1;
        /// <summary>自転車番号</summary>
        public int SelectBicycleId = 1;

        /// <summary>クリア済のチュートリアルID</summary>
        public List<int> ClearedTutorialIds = new List<int>();

        /// <summary>購入したバッグID</summary>
        public List<int> PurchaseBagIds = new List<int>();
        /// <summary>購入したアバターID</summary>
        public List<int> PurchaseAvatarIds = new List<int>();
        /// <summary>購入した自転車ID</summary>
        public List<int> PurchaseBicycleIds = new List<int>();

        /// <summary>TipsのID</summary>
        public int TipsId = 0;

        /// <summary>グラフィックレベル</summary>
        public int GraphicLevel = 0;

        public bool IsMuteSe = false;
        public bool IsMuteBgm = false;
        public bool IsFrameRate60 = true;

        public bool IsVersionUp = false;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            StageLevel = 1;
            TotalStageLevel = 1;
            InterstitialAdStagePlayCount = 0;
            AllClearCount = 0;
            BaseLevel = 1;
            MoneyNum = 0;
            TotalMoneyNum = 0;
            JewelNum = 0;
            SelectBagId = 1;
            SelectAvatarId = 1;
            SelectBicycleId = 1;
            ClearedTutorialIds = new List<int>();
            PurchaseBagIds = new List<int>() { 1 };
            PurchaseAvatarIds = new List<int>() { 1 };
            PurchaseBicycleIds = new List<int>() { 1 };
            TipsId = 0;
            GraphicLevel = 0;
            IsMuteSe = false;
            IsMuteBgm = false;
            IsFrameRate60 = true;
        }

        /// <summary>
        /// 強制広告用カウント加算
        /// </summary>
        public void AddInterstitialAdStagePlayCount()
        {
            // 初期離脱を防ぐため、ステージ3クリアまではカウントしない
            if (AllClearCount == 0 && AllClearCount == 0 && StageLevel < InGameConstant.INTERSTITIAL_AD_STAGE_THRESHOLD_VALUE)
            {
                return;
            }
            InterstitialAdStagePlayCount++;
        }
    }
} // template