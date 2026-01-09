using UnityEngine;

public class HPBar : MonoBehaviour
{
    public Transform fill;
    private float maxWidth = 1f;

    public void SetHP(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        Vector3 scale = fill.localScale;
        scale.x = maxWidth * ratio;
        fill.localScale = scale;
        Vector3 pos = fill.localPosition;
        pos.x = -(maxWidth - scale.x) / 2f;
        fill.localPosition = pos;
    }
}
