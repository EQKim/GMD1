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
    [Tooltip("World-units overlap between pieces to hide any seam.")]
    [SerializeField] private float verticalOverlap = 0.05f;

    [Tooltip("Wrap earlier than camera bottom to prevent 1-frame gaps (blue lines).")]
    [SerializeField] private float wrapBuffer = 0.15f;

    [Header("Optional: Pixel Snapping")]
    [Tooltip("If true, snaps Y positions to a pixel grid each frame (helps with sub-pixel seams).")]
    [SerializeField] private bool pixelSnapY = false;

    [Tooltip("Used only when PixelSnapY is enabled. Must match your sprite PPU (Pixels Per Unit).")]
    [SerializeField] private float pixelsPerUnit = 16f;

    private Camera cam;
    private int nextIndex = 0;

    private void Awake()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("No Main Camera found. Tag your camera as MainCamera.");
            enabled = false;
            return;
        }

        if (pieceA == null || pieceB == null || variants == null || variants.Length == 0)
        {
            Debug.LogError("Missing references on EndlessBackgroundManager.");
            enabled = false;
            return;
        }

        pieceA.sprite = variants[0];
        pieceB.sprite = variants[Mathf.Min(1, variants.Length - 1)];

        StretchToCameraHeight(pieceA);
        StretchToCameraHeight(pieceB);

        SnapAbove(pieceB, pieceA, verticalOverlap);

        if (pixelSnapY)
        {
            SnapToPixelGrid(pieceA.transform);
            SnapToPixelGrid(pieceB.transform);
        }
    }

    private void Update()
    {
        float dy = scrollSpeed * Time.deltaTime;

        pieceA.transform.position += Vector3.down * dy;
        pieceB.transform.position += Vector3.down * dy;

        if (pixelSnapY)
        {
            SnapToPixelGrid(pieceA.transform);
            SnapToPixelGrid(pieceB.transform);
        }

        TryWrap(pieceA, pieceB);
        TryWrap(pieceB, pieceA);
    }

    public void SetScrollSpeed(float newSpeed)
    {
        scrollSpeed = Mathf.Max(0f, newSpeed);
    }

    public float GetScrollSpeed()
    {
        return scrollSpeed;
    }

    private void TryWrap(SpriteRenderer moving, SpriteRenderer other)
    {
        float camBottom = cam.transform.position.y - cam.orthographicSize;

        if (moving.bounds.max.y < camBottom + wrapBuffer)
        {
            moving.sprite = GetNextSprite();

            StretchToCameraHeight(moving);
            SnapAbove(moving, other, verticalOverlap);

            if (pixelSnapY)
                SnapToPixelGrid(moving.transform);
        }
    }

    private void StretchToCameraHeight(SpriteRenderer sr)
    {
        if (sr.sprite == null) return;

        float worldHeight = cam.orthographicSize * 2f;
        float spriteHeight = sr.sprite.bounds.size.y;

        if (spriteHeight <= 0f) return;

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
        if (variants == null || variants.Length == 0) return null;
        if (variants.Length == 1) return variants[0];

        if (randomize)
            return variants[Random.Range(0, variants.Length)];

        Sprite s = variants[nextIndex];
        nextIndex = (nextIndex + 1) % variants.Length;
        return s;
    }

    private void SnapToPixelGrid(Transform t)
    {
        float unitsPerPixel = 1f / Mathf.Max(0.0001f, pixelsPerUnit);

        Vector3 p = t.position;
        p.y = Mathf.Round(p.y / unitsPerPixel) * unitsPerPixel;
        t.position = p;
    }
}