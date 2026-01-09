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

    private void Awake()
    {
        Instance = this;
    }

    public void showDailyRewardPanel()
    {
        if (dailyRewardPanel != null)
        {
            dailyRewardPanel.SetActive(true);
        }
    }
    public void hideDailyRewardPanel()
    {
        if (dailyRewardPanel != null)
        {
            dailyRewardPanel.SetActive(false);
        }
    }
    public void showDailyRewardAds()
    {
        if (adsPanel != null)
        {
            adsPanel.SetActive(true);
        }
    }
    public void hideDailyRewardAds()
    {
        if (adsPanel != null)
        {
            adsPanel.SetActive(false);
        }
    }

    public void showCollectionPanel()
    {
        if (collectionPanel != null)
        {
            collectionPanel.SetActive(true);
            rangeCollection.SetActive(true);
            meleeCollection.SetActive(false);
        }
    }
    public void hideCollectionPanel()
    {
        if (collectionPanel != null)
        {
            collectionPanel.SetActive(false);
            rangeCollection.SetActive(false);
            meleeCollection.SetActive(false);
        }
    }
    public void showRangeCollection()
    {
        if (rangeCollection != null)
        {
            rangeCollection.SetActive(true);
            meleeCollection.SetActive(false);
        }
    }
    public void showMeleeCollection()
    {
        if (rangeCollection != null)
        {
            rangeCollection.SetActive(false);
            meleeCollection.SetActive(true);
        }
    }

    public void showSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }

    public void hideSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    public void showSummonPanel()
    {
        if (summonPanel != null)
        {
            summonPanel.SetActive(true);
            rangeSummon.SetActive(true);
            meleeSummon.SetActive(false);
        }
    }
    public void hideSummonPanel()
    {
        if (summonPanel != null)
        {
            summonPanel.SetActive(false);
            rangeSummon.SetActive(false);
            meleeSummon.SetActive(false);
        }
    }
    public void showRangeSummon()
    {
        if (rangeSummon != null)
        {
            rangeSummon.SetActive(true);
            meleeSummon.SetActive(false);
        }
    }
    public void showMeleeSummon()
    {
        if (rangeSummon != null)
        {
            rangeSummon.SetActive(false);
            meleeSummon.SetActive(true);
        }
    }
}
