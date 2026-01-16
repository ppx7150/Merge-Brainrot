using UnityEngine;
using UnityEngine.EventSystems;
using System;
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
    public static event Action<ButtonType> OnButton2DClicked;
    public void OnPointerClick(PointerEventData eventData) //Kiểm tra va chạm raycast của Button
    {
        bool isSuccess = false;
        switch (buttonType)
        {
            case ButtonType.SpawnMelee:
                if (BattleManager.Instance.startPvP || GridManager.Instance.isFull() || (TutorialController.Instance.currentState == TutorialController.TutorialState.None && !Char.Instance.SubCoins((int)UnitSpawner.Instance.costMelee))) return;
                UnitSpawner.Instance.SpawnMeleeUnit(0);
                AudioManager.Instance.Play(GameSound.buyMeleeSound);
                isSuccess = true;
                break;
            case ButtonType.SpawnRange:
                if (BattleManager.Instance.startPvP! || GridManager.Instance.isFull() || (TutorialController.Instance.currentState == TutorialController.TutorialState.None && !Char.Instance.SubCoins((int)UnitSpawner.Instance.costRange))) return;
                UnitSpawner.Instance.SpawnRangeUnit(0);
                AudioManager.Instance.Play(GameSound.buyRangeSound);
                isSuccess = true;
                break;
            case ButtonType.Battle:
                if (BattleManager.Instance.startPvP) return;
                BattleManager.Instance.StartBattle();
                AudioManager.Instance.Play(GameSound.fightSound);
                isSuccess = true;
                break;
            case ButtonType.DailyReward:
                PanelManager.Instance.showDailyRewardPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                isSuccess = true;
                break;
            case ButtonType.Collection:
                PanelManager.Instance.showCollectionPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                isSuccess = true;
                break;
            case ButtonType.Setting:
                PanelManager.Instance.showSettingPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                isSuccess = true;
                break;
            case ButtonType.Summon:
                PanelManager.Instance.showSummonPanel();
                AudioManager.Instance.Play(GameSound.clickButtonSound);
                isSuccess = true;
                break;
        }
        if (isSuccess)
        {
            OnButton2DClicked?.Invoke(buttonType);
        }
    }
}