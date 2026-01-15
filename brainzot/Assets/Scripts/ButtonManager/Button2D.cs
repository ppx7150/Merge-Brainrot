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
    public void OnPointerClick(PointerEventData eventData) //Kiểm tra va chạm raycast của Button
    {
        switch(buttonType)
        {
            case ButtonType.SpawnMelee:
                if (BattleManager.Instance.startPvP || !Char.Instance.SubCoins((int)UnitSpawner.Instance.costMelee)) return;
                UnitSpawner.Instance.UpgradeCost(true);
                UnitSpawner.Instance.SpawnMeleeUnit(0);
                AudioManager.Instance.Play(GameSound.buyMeleeSound);
                break;
            case ButtonType.SpawnRange:
                if (BattleManager.Instance.startPvP! || !Char.Instance.SubCoins((int)UnitSpawner.Instance.costRange)) return;
                UnitSpawner.Instance.UpgradeCost(false);
                UnitSpawner.Instance.SpawnRangeUnit(0);
                AudioManager.Instance.Play(GameSound.buyRangeSound);
                break;
            case ButtonType.Battle:
                if (BattleManager.Instance.startPvP) return;
                BattleManager.Instance.StartBattle();
                AudioManager.Instance.Play(GameSound.fightSound);
                break;
            case ButtonType.DailyReward:
                PanelManager.Instance.showDailyRewardPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                break;
            case ButtonType.Collection:
                PanelManager.Instance.showCollectionPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                break;
            case ButtonType.Setting:
                PanelManager.Instance.showSettingPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                break;
            case ButtonType.Summon:
                PanelManager.Instance.showSummonPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                break;
        }

    }
}