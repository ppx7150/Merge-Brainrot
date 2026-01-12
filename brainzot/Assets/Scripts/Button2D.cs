using UnityEngine;
using UnityEngine.EventSystems;
public enum ButtonType
{
    SpawnMelee,
    SpawnRange,
    Battle,
    Summon,
    Setting,
    Collection,
    DailyReward
}
public class Button2D : MonoBehaviour, IPointerClickHandler
{
    public ButtonType buttonType;
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(buttonType)
        {
            case ButtonType.SpawnMelee:
                if (BattleManager.Instance.startPvP || !Char.Instance.SubCoins(UnitSpawner.Instance.costMelee)) return;
                UnitSpawner.Instance.UpgradeCost(true);
                UnitSpawner.Instance.SpawnMeleeUnit(0);
                break;
            case ButtonType.SpawnRange:
                if (BattleManager.Instance.startPvP! || Char.Instance.SubCoins(UnitSpawner.Instance.costRange)) return;
                UnitSpawner.Instance.UpgradeCost(false);
                UnitSpawner.Instance.SpawnRangeUnit(0);
                break;
            case ButtonType.Battle:
                if (BattleManager.Instance.startPvP) return;
                BattleManager.Instance.StartBattle();
                break;
            case ButtonType.DailyReward:
                PanelManager.Instance.showDailyRewardPanel();
                break;
            case ButtonType.Collection:
                PanelManager.Instance.showCollectionPanel();
                break;
            case ButtonType.Setting:
                PanelManager.Instance.showSettingPanel();
                break;
            case ButtonType.Summon:
                PanelManager.Instance.showSummonPanel();
                break;
        }

    }
}