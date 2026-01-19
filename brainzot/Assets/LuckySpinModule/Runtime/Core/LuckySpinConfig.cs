using System.Collections.Generic;
using UnityEngine;

namespace LuckySpinModule.Core
{
    /// <summary>
    /// Configuration cho Lucky Spin module
    /// </summary>
    [CreateAssetMenu(fileName = "LuckySpinConfig", menuName = "Lucky Spin Module/Config")]
    public class LuckySpinConfig : ScriptableObject
    {
        [Header("Spin Settings")]
        [Tooltip("Tên ads setting cho spin")]
        public string adsName = "reward_spin";

        [Tooltip("Tên ads setting cho interstitial break")]
        public string adsNameInterBreak = "interstitial_break";

        [Tooltip("Thời gian quay wheel (giây)")]
        public float spinDuration = 6f;

        [Tooltip("Số vòng quay đầy đủ")]
        public int fullRounds = 5;

        [Header("Reward Settings")]
        [Tooltip("Danh sách rewards có thể nhận")]
        public List<SpinRewardData> rewards = new List<SpinRewardData>();

        [Header("UI Settings")]
        [Tooltip("Tên prefab reward popup")]
        public string rewardPopupName = "BuySuccessPopup";

        [Tooltip("Tên prefab ads not ready popup")]
        public string adsNotReadyPopupName = "IAANotReady";

        [Tooltip("Tên prefab loading inter ads popup")]
        public string loadingInterAdsPopupName = "LoadingInterAdsPopup";
    }

    [System.Serializable]
    public class SpinRewardData
    {
        public string id;
        public long amount;
        public ItemType type;
        public int ratio = 10; // Weight cho random
    }

    public enum ItemType
    {
        Gold,
        Booster_1,
        Booster_2,
        Booster_3,
        Booster_6
    }
}
