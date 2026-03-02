using UnityEngine;

public class EndlessBackgroundManager : MonoBehaviour
{
    [Header("Scene pieces (2 is enough for infinite scrolling)")]
    [SerializeField] private SpriteRenderer pieceA;
    [SerializeField] private SpriteRenderer pieceB;

    [Header("Background variations")]
    [SerializeField] private Sprite[] variants;

    [Header("Scroll")]
    [SerializeField] private float scrollSpeed = 1.5f;
    [SerializeField] private bool randomize = false;

    [Header("Tiling")]
    [SerializeField] private float verticalOverlap = 0.05f; // world units to overlap pieces

    private Camera cam;
    private int nextIndex = 0;

    private void Awake()
    {
        cam = Camera.main;

        if (pieceA == null || pieceB == null || variants == null || variants.Length == 0)
        {
            Debug.LogError("Missing references on EndlessBackgroundManager.");
            enabled = false;
            return;
        }

        // Set initial sprites
        pieceA.sprite = variants[0];
        pieceB.sprite = variants[Mathf.Min(1, variants.Length - 1)];

        // Stretch them to camera height
        StretchToCameraHeight(pieceA);
        StretchToCameraHeight(pieceB);

        // Snap B above A
        SnapAbove(pieceB, pieceA, verticalOverlap);
    }

    private void Update()
    {
        float dy = scrollSpeed * Time.deltaTime;

        pieceA.transform.position += Vector3.down * dy;
        pieceB.transform.position += Vector3.down * dy;

        TryWrap(pieceA, pieceB);
        TryWrap(pieceB, pieceA);
    }

    private void TryWrap(SpriteRenderer moving, SpriteRenderer other)
    {
        float camBottom = cam.transform.position.y - cam.orthographicSize;

        if (moving.bounds.max.y < camBottom)
        {
            moving.sprite = GetNextSprite();

            // Re-stretch because sprite may have different size
            StretchToCameraHeight(moving);

            SnapAbove(moving, other, verticalOverlap);
        }
    }

    private void StretchToCameraHeight(SpriteRenderer sr)
    {
        float worldHeight = cam.orthographicSize * 2f;
        float spriteHeight = sr.sprite.bounds.size.y;

        float scaleFactor = worldHeight / spriteHeight;

        Vector3 scale = sr.transform.localScale;
        scale.y = scaleFactor;
        sr.transform.localScale = scale;
    }

    private static void SnapAbove(SpriteRenderer top, SpriteRenderer bottom, float overlap = 0f)
    {
        float offset = top.transform.position.y - top.bounds.center.y;
        float desiredY = bottom.bounds.max.y + top.bounds.extents.y - overlap;

        Vector3 p = top.transform.position;
        p.y = desiredY + offset;
        top.transform.position = p;
    }

    private Sprite GetNextSprite()
    {
        if (variants.Length == 1) return variants[0];

        if (randomize)
            return variants[Random.Range(0, variants.Length)];

        Sprite s = variants[nextIndex];
        nextIndex = (nextIndex + 1) % variants.Length;
        return s;
    }
}