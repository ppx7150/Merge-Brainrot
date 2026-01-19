using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LuckySpinModule.Core;

namespace LuckySpinModule.UI
{
    /// <summary>
    /// Popup quản lý Lucky Spin
    /// </summary>
    public class LuckySpinPopup : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private LuckySpinConfig config;

        [Header("UI References")]
        public Button spinBtn;
        public Transform pointer;
        public Transform wheel;
        public GameObject center;
        public GameObject centerAnim; // Có thể là SkeletonGraphic
        public UnityEngine.UI.Text spinCountText;
        public UnityEngine.UI.Text countdownText;
        public UnityEngine.UI.Text btnCountdownText;
        public Button hideBtn;
        public Button closeBtn;
        public Sprite[] buttonSprites;
        public GameObject timeBox;

        private ILuckySpinDataProvider dataProvider;
        private bool isClickSpin;
        private int lastSpinReward
        {
            get => PlayerPrefs.GetInt("SpinReward", 0);
            set
            {
                PlayerPrefs.SetInt("SpinReward", value);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Initialize với data provider và config
        /// </summary>
        public void Initialize(ILuckySpinDataProvider provider, LuckySpinConfig configData = null)
        {
            dataProvider = provider;
            if (configData != null)
            {
                config = configData;
            }

            if (config == null)
            {
                Debug.LogError("LuckySpinConfig is missing!");
            }
        }

        private void Start()
        {
            if (dataProvider == null || config == null)
            {
                Debug.LogError("LuckySpinPopup chưa được initialize!");
                return;
            }

            UpdateSpinCount();
            CheckSpinStatus();
            StartCountdown();

            if (spinBtn != null)
                spinBtn.onClick.AddListener(Spin);
        }

        private void OnEnable()
        {
            if (dataProvider != null && dataProvider.GetSpinLimit() == 2)
                lastSpinReward = 0;
            
            if (wheel != null)
                wheel.rotation = Quaternion.identity;
        }

        private void UpdateSpinCount()
        {
            if (dataProvider == null || config == null) return;

            int quota = dataProvider.GetAdsQuota(config.adsName);
            int used = dataProvider.GetDailySpinNum();
            int left = Math.Max(0, quota - used);

            if (spinCountText != null)
                spinCountText.text = $"{left}/{quota}";

            dataProvider.SetSpinLimit(left);
        }

        private void CheckSpinStatus()
        {
            if (dataProvider == null || config == null) return;

            var btnImage = spinBtn?.transform.GetChild(0)?.GetComponent<Image>();
            int limit = dataProvider.GetSpinLimit();

            if (limit > 0)
            {
                if (btnImage != null && buttonSprites != null && buttonSprites.Length > 0)
                    btnImage.sprite = buttonSprites[0];
                
                if (timeBox != null)
                    timeBox.SetActive(true);
                
                if (btnCountdownText != null)
                    btnCountdownText.text = "SPIN";
                
                UpdateSpinCount();
            }
            else
            {
                if (timeBox != null)
                    timeBox.SetActive(false);
                
                if (btnImage != null && buttonSprites != null && buttonSprites.Length > 1)
                    btnImage.sprite = buttonSprites[1];
                
                StartCountdown();
            }
        }

        public void Spin()
        {
            if (dataProvider == null || config == null) return;

            dataProvider.PlaySound("sfx_button_click_1");

            bool canShowAds = dataProvider.CanShowAds(config.adsName) && dataProvider.IsRewardVideoReady();
            int limit = dataProvider.GetSpinLimit();

            if (limit > 0)
            {
                isClickSpin = true;
                if (canShowAds)
                {
                    dataProvider.ShowRewardVideo(() =>
                    {
                        dataProvider.SetSpinLimit(limit - 1);
                        dataProvider.SetDailySpinNum(dataProvider.GetDailySpinNum() + 1);
                        UpdateSpinCount();
                        CheckSpinStatus();

                        SpinRewardData reward = GetRandomReward();
                        dataProvider.OnWatchAdsCompleted(config.adsName);

                        int index = config.rewards.IndexOf(reward);
                        float targetAngle = GetFinalAngle(index);
                        StartCoroutine(RotateWheel(targetAngle, config.spinDuration, reward));

                        dataProvider.SendMessage("OnSpinLuckyWheel");
                    }, () => { });
                }
                else
                {
                    dataProvider.ShowAdsNotReadyPopup();
                }
            }
        }

        private SpinRewardData GetRandomReward()
        {
            if (config == null || config.rewards == null || config.rewards.Count == 0)
                return null;

            List<SpinRewardData> filteredList;

            if (lastSpinReward == 0)
            {
                filteredList = config.rewards;
            }
            else
            {
                if (lastSpinReward == 1)
                    filteredList = config.rewards.Where(r => r.type != ItemType.Gold).ToList();
                else
                    filteredList = config.rewards.Where(r => r.type == ItemType.Gold).ToList();
            }

            float totalWeight = filteredList.Sum(r => r.ratio);
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentSum = 0f;

            foreach (var reward in filteredList)
            {
                currentSum += reward.ratio;
                if (randomValue <= currentSum)
                {
                    lastSpinReward = reward.type == ItemType.Gold ? 1 : 2;
                    return reward;
                }
            }

            return filteredList[0];
        }

        private float GetFinalAngle(int index)
        {
            if (config == null || config.rewards == null) return 0;

            float anglePerSlice = 360f / config.rewards.Count;
            int fixedIndex = (config.rewards.Count - index) % config.rewards.Count;
            float totalRotation = 360f * config.fullRounds + fixedIndex * anglePerSlice;
            return -totalRotation;
        }

        private IEnumerator RotateWheel(float targetAngle, float duration, SpinRewardData reward)
        {
            if (spinBtn != null) spinBtn.enabled = false;
            if (hideBtn != null) hideBtn.enabled = false;
            if (closeBtn != null) closeBtn.enabled = false;

            yield return new WaitForSeconds(0.5f);

            if (dataProvider != null)
                dataProvider.PlaySound("sfx_luckywheel");

            float startAngle = wheel.eulerAngles.z;
            if (startAngle > 180f) startAngle -= 360f;

            float elapsed = 0f;
            float segmentAngle = 360f / config.rewards.Count;
            float lastSegment = Mathf.Floor(startAngle / segmentAngle);

            PlayAnim();

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float angle = Mathf.Lerp(startAngle, targetAngle, EaseOutQuart(t));
                wheel.eulerAngles = new Vector3(0, 0, angle);

                float currentSegment = Mathf.Floor(angle / segmentAngle);
                if (currentSegment != lastSegment)
                {
                    PointerBounce();
                    lastSegment = currentSegment;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            wheel.eulerAngles = new Vector3(0, 0, targetAngle);
            DeactiveAnim();
            ClaimReward(reward);

            yield return new WaitForSeconds(0.5f);

            if (spinBtn != null) spinBtn.enabled = true;
            if (hideBtn != null) hideBtn.enabled = true;
            if (closeBtn != null) closeBtn.enabled = true;
        }

        private void ClaimReward(SpinRewardData reward)
        {
            if (dataProvider == null || reward == null) return;

            dataProvider.ShowRewardPopup(
                reward.type.ToString(),
                reward.amount,
                reward.id
            );

            switch (reward.type)
            {
                case ItemType.Gold:
                    dataProvider.AddCoin(reward.amount);
                    dataProvider.SendMessage("OnCoinChange");
                    break;
                case ItemType.Booster_1:
                    dataProvider.AddBooster("8000000", reward.amount);
                    break;
                case ItemType.Booster_2:
                    dataProvider.AddBooster("9000000", reward.amount);
                    break;
                case ItemType.Booster_3:
                    dataProvider.AddBooster("1100000", reward.amount);
                    break;
                case ItemType.Booster_6:
                    dataProvider.AddBooster("1200000", reward.amount);
                    break;
            }
        }

        private float EaseOutQuart(float t)
        {
            return 1 - Mathf.Pow(1 - t, 2);
        }

        private void PointerBounce()
        {
            // Implement pointer bounce animation
            // Có thể dùng DOTween hoặc simple animation
        }

        private void PlayAnim()
        {
            if (centerAnim != null) centerAnim.SetActive(true);
            if (center != null) center.SetActive(false);
        }

        private void DeactiveAnim()
        {
            if (center != null) center.SetActive(true);
            if (centerAnim != null) centerAnim.SetActive(false);
        }

        private Coroutine countdownCoroutine;

        private void StartCountdown()
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            countdownCoroutine = StartCoroutine(CountdownText());
        }

        private IEnumerator CountdownText()
        {
            DateTime lastResetDate = DateTime.Now.Date;

            while (true)
            {
                DateTime now = DateTime.Now;
                DateTime nextReset = lastResetDate.AddDays(1);
                TimeSpan remaining = nextReset - now;

                if (remaining.TotalSeconds <= 0)
                {
                    ResetSpin();
                    lastResetDate = now.Date;
                    remaining = nextReset.AddDays(1) - now;
                }

                if (btnCountdownText != null)
                {
                    btnCountdownText.text = string.Format(
                        "{0:D2}:{1:D2}:{2:D2}",
                        remaining.Hours, remaining.Minutes, remaining.Seconds
                    );
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void ResetSpin()
        {
            if (dataProvider == null || config == null) return;

            DateTime now = DateTime.Now;
            string today = now.ToString("yyyyMMdd");
            string lastDate = dataProvider.GetLastSpinDate();

            if (lastDate != today)
            {
                int quota = dataProvider.GetAdsQuota(config.adsName);
                dataProvider.SetSpinLimit(quota);
                dataProvider.SetDailySpinNum(0);
                dataProvider.SetLastSpinDate(today);
                UpdateSpinCount();
                CheckSpinStatus();
            }
        }

        public void OnClickClose()
        {
            if (dataProvider != null)
                dataProvider.PlaySound("sfx_button_click_1");

            // Hide popup logic
            if (gameObject != null)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
        }
    }
}
