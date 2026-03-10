using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class SpawnPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Behavior")]
    [SerializeField] private bool spawnPlayerOnStart = true;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float extraPlayerHeight = 0.05f;

    private SpriteRenderer sr;
    private Collider2D col;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        // Do not auto-start here anymore.
        // The GameStartScreen / GameManager will decide when the round begins.
        SetPlatformVisible(true);
    }

    public void StartPlatformSequence()
    {
        if (spawnPlayerOnStart)
            RespawnPlayer();
        else
            BeginFade();
    }

    public void RespawnPlayer()
    {
        if (player == null)
        {
            Debug.LogError($"{name}: Player reference not set.");
            return;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) rb = player.GetComponentInParent<Rigidbody2D>();

        Transform target = rb != null ? rb.transform : player;

        Collider2D playerCol = target.GetComponent<Collider2D>();
        if (playerCol == null) playerCol = target.GetComponentInChildren<Collider2D>();

        if (playerCol == null)
        {
            Debug.LogError($"{name}: Could not find a Collider2D on the player.");
            return;
        }

        SetPlatformVisible(true);

        Bounds platformB = col.bounds;
        Bounds playerB = playerCol.bounds;

        float skin = Mathf.Max(0.02f, extraPlayerHeight);

        float deltaY = (platformB.max.y + skin) - playerB.min.y;
        float deltaX = transform.position.x - playerB.center.x;

        Vector3 newPos = target.position + new Vector3(deltaX, deltaY, 0f);
        target.position = newPos;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        BeginFade();
    }

    private void BeginFade()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeAndDisable());
    }

    private IEnumerator FadeAndDisable()
    {
        float wait = Mathf.Max(0f, lifetime - fadeDuration);
        yield return new WaitForSeconds(wait);

        Color start = sr.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            sr.color = new Color(start.r, start.g, start.b, a);
            yield return null;
        }

        SetPlatformVisible(false);
    }

    private void SetPlatformVisible(bool visible)
    {
        if (visible)
        {
            sr.enabled = true;
            col.enabled = true;
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 1f);
        }
        else
        {
            sr.enabled = false;
            col.enabled = false;
        }
    }
}