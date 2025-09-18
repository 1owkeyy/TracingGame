using UnityEngine;

public class Dot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Assign the two sprites in the Inspector
    public Sprite normalSprite;
    public Sprite highlightedSprite;

    private bool isHighlighted = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (normalSprite != null)
            spriteRenderer.sprite = normalSprite;
    }

    public void Highlight()
    {
        if (isHighlighted) return;

        if (highlightedSprite != null)
            spriteRenderer.sprite = highlightedSprite;

        isHighlighted = true;
    }
}
