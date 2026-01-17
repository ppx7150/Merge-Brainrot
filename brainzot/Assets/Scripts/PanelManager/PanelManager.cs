using DG.Tweening;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject dailyRewardPanel;

    public GameObject adsPanel;

    public GameObject collectionPanel;
    public GameObject rangeCollection;
    public GameObject meleeCollection;

    public GameObject settingPanel;

    public GameObject summonPanel;
    public GameObject rangeSummon;
    public GameObject meleeSummon;

    public static PanelManager Instance;

    [Header("Cài đặt")]
    public float duration = 0.5f; // Thời gian hiệu ứng
    public Ease openEase = Ease.OutBack; // Kiểu nảy khi mở
    public Ease closeEase = Ease.InBack; // Kiểu thu vào khi đóng

    private Vector3 initialScale;

    // Gọi hàm này để MỞ Panel
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        AudioManager.Instance.Play(GameSound.clickButtonSound);
        initialScale = new Vector3(1, 1, 1);
        panel.transform.localScale = Vector3.zero;

        panel.transform.DOScale(initialScale, duration)
            .SetEase(openEase);
    }

    // Gọi hàm này để ĐÓNG Panel
    public void ClosePanel(GameObject panel)
    {
        AudioManager.Instance.Play(GameSound.clickButtonSound);
        // Thu nhỏ về 0
        panel.transform.DOScale(Vector3.zero, duration) // Đóng thì nên nhanh hơn mở 1 chút
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                // Sau khi thu nhỏ xong -> Tắt toàn bộ Container (biến mất cả nền đen)
                panel.SetActive(false);
            });
    }

    private void Awake()
    {
        Instance = this;
    }

    public void showDailyRewardPanel()
    {
        if (dailyRewardPanel != null)
        {
            OpenPanel(dailyRewardPanel);
        }
    }
    public void hideDailyRewardPanel()
    {
        if (dailyRewardPanel != null)
        {
            ClosePanel(dailyRewardPanel);
        }
    }
    public void showDailyRewardAds()
    {
        if (adsPanel != null)
        {
            OpenPanel(adsPanel);
        }
    }
    public void hideDailyRewardAds()
    {
        if (adsPanel != null)
        {
            ClosePanel(adsPanel);
        }
    }

    public void showCollectionPanel()
    {
        if (collectionPanel != null)
        {
            OpenPanel(collectionPanel);
            rangeCollection.SetActive(true);
            meleeCollection.SetActive(false);
        }
    }
    public void hideCollectionPanel()
    {
        if (collectionPanel != null)
        {
            ClosePanel(collectionPanel);
        }
    }
    public void showRangeCollection()
    {
        if (rangeCollection != null)
        {
            rangeCollection.SetActive(true);
            meleeCollection.SetActive(false);
            AudioManager.Instance.Play(GameSound.clickButtonSound);
        }
    }
    public void showMeleeCollection()
    {
        if (rangeCollection != null)
        {
            rangeCollection.SetActive(false);
            meleeCollection.SetActive(true);
            AudioManager.Instance.Play(GameSound.clickButtonSound);
        }
    }

    public void showSettingPanel()
    {
        if (settingPanel != null)
        {
            OpenPanel(settingPanel);
        }
    }

    public void hideSettingPanel()
    {
        if (settingPanel != null)
        {
            ClosePanel(settingPanel);
        }
    }

    public void showSummonPanel()
    {
        if (summonPanel != null)
        {
            OpenPanel(summonPanel);
            rangeSummon.SetActive(true);
            meleeSummon.SetActive(false);
        }
    }
    public void hideSummonPanel()
    {
        if (summonPanel != null)
        {
            ClosePanel(summonPanel);
        }
    }
    public void showRangeSummon()
    {
        if (rangeSummon != null)
        {
            rangeSummon.SetActive(true);
            meleeSummon.SetActive(false);
            AudioManager.Instance.Play(GameSound.clickButtonSound);
        }
    }
    public void showMeleeSummon()
    {
        if (rangeSummon != null)
        {
            rangeSummon.SetActive(false);
            meleeSummon.SetActive(true);
            AudioManager.Instance.Play(GameSound.clickButtonSound);
        }
    }
}
