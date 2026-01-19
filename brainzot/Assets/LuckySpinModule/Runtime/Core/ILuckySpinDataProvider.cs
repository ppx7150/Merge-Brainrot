namespace LuckySpinModule.Core
{
    /// <summary>
    /// Interface để inject dependencies cho Lucky Spin module
    /// </summary>
    public interface ILuckySpinDataProvider
    {
        /// <summary>
        /// Lấy số spin còn lại
        /// </summary>
        int GetSpinLimit();

        /// <summary>
        /// Set số spin còn lại
        /// </summary>
        void SetSpinLimit(int limit);

        /// <summary>
        /// Lấy số spin đã dùng hôm nay
        /// </summary>
        int GetDailySpinNum();

        /// <summary>
        /// Set số spin đã dùng hôm nay
        /// </summary>
        void SetDailySpinNum(int num);

        /// <summary>
        /// Lấy quota spin từ ads setting
        /// </summary>
        int GetAdsQuota(string adsName);

        /// <summary>
        /// Kiểm tra có thể show ads không
        /// </summary>
        bool CanShowAds(string adsName);

        /// <summary>
        /// Kiểm tra reward video ready không
        /// </summary>
        bool IsRewardVideoReady();

        /// <summary>
        /// Show reward video
        /// </summary>
        void ShowRewardVideo(System.Action onRewarded, System.Action onFailed);

        /// <summary>
        /// Callback khi watch ads completed
        /// </summary>
        void OnWatchAdsCompleted(string adsName);

        /// <summary>
        /// Thêm coin
        /// </summary>
        void AddCoin(long amount);

        /// <summary>
        /// Thêm booster
        /// </summary>
        void AddBooster(string boosterId, long amount);

        /// <summary>
        /// Show reward popup
        /// </summary>
        void ShowRewardPopup(string rewardType, long amount, string itemType);

        /// <summary>
        /// Show popup khi ads not ready
        /// </summary>
        void ShowAdsNotReadyPopup();

        /// <summary>
        /// Play sound
        /// </summary>
        void PlaySound(string soundName);

        /// <summary>
        /// Send message
        /// </summary>
        void SendMessage(string messageType);

        /// <summary>
        /// Lấy last spin date
        /// </summary>
        string GetLastSpinDate();

        /// <summary>
        /// Set last spin date
        /// </summary>
        void SetLastSpinDate(string date);
    }
}
