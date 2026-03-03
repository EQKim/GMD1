using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AutoDestroyAfterAnimation : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
        yield return null;
        var info = anim.GetCurrentAnimatorClipInfo(0);
        float len = (info != null && info.Length > 0) ? info[0].clip.length : 1f;
        Destroy(gameObject, len);
    }
}