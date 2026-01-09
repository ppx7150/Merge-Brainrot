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
        if (BattleManager.Instance.startPvP) return;
        switch(buttonType)
        {
            case ButtonType.SpawnMelee:
                UnitSpawner.Instance.SpawnMeleeUnit();
                break;
            case ButtonType.SpawnRange:
                UnitSpawner.Instance.SpawnRangeUnit();
                break;
            case ButtonType.Battle:
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