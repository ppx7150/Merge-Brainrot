using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveUpSpeed = 1f;
    public float lifeTime = 1f;

    private TextMeshPro text;
    private Color color;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        color = text.color;
    }

    public void SetText(string value)
    {
        text.SetText(value);
    }

    void Update()
    {
        // Bay lên
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        // Mờ dần
        color.a -= Time.deltaTime / lifeTime;
        text.color = color;

        // Tự huỷ
        if (color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
