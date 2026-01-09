using UnityEngine;
using UnityEngine.EventSystems;
public enum ButtonType
{
    SpawnMelee,
    SpawnRange,
    Battle
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
        }

    }
}