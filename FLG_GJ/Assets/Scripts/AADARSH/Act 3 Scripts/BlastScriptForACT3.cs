using UnityEngine;
using System.Collections; // Required for using Coroutines

public class BlastScriptForACT3 : MonoBehaviour
{
    // --- NEW ---
    [Header("Animation Settings")]
    [Tooltip("The total length of your 'BlastAct3' animation clip in seconds.")]
    [SerializeField] private float blastAnimationLength = 1.0f; // Set this in the Inspector!

    [Header("Game Object References")]
    [SerializeField] GameObject fences;

    // --- Private Variables ---
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Store a reference to the SpriteRenderer

    // --- Initialization ---
    void Awake()
    {
        // Get the components attached to this GameObject.
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("BlastScriptForACT3 requires an Animator component, but none was found!", this.gameObject);
            return;
        }

        // IMPORTANT: Ensure the animator is disabled by default.
        animator.enabled = false;
    }

    // --- Public Function to be called by Unity Events ---
    public void TriggerBlast()
    {
        if (animator != null)
        {
            StartCoroutine(BlastCoroutine());
        }
        else
        {
            Debug.LogWarning("TriggerBlast was called, but no Animator is available.", this.gameObject);
        }
    }

    // --- Coroutine for the Timed Blast ---
    private IEnumerator BlastCoroutine()
    {
        // 1. Wait for the initial 2-second delay.
        yield return new WaitForSeconds(2f);

        // 2. Start the blast sequence.
        fences.SetActive(false);
        Debug.Log("Enabling blast animation on " + gameObject.name);
        animator.enabled = true; // The animation will now start playing.

        // 3. Set a flag shortly after the blast begins.
        yield return new WaitForSeconds(0.3f);
        StoryManagertAct1A.Instance.SetFlag("PoliceComing", true);

        // 4. --- MODIFIED ---
        //    Wait for the remainder of the animation to finish playing.
        float remainingAnimationTime = blastAnimationLength - 0.3f;
        if (remainingAnimationTime > 0)
        {
            yield return new WaitForSeconds(remainingAnimationTime);
        }

        // The animation is now complete.
        Debug.Log("Blast animation finished.");

        // NOTE: I have removed the line that was setting the sprite manually.
        // `gameObject.GetComponent<SpriteRenderer>().sprite = sprite;`
        // This line conflicts with the Animator, which controls the sprite during the animation. Removing it ensures the animation plays fully.

        // 5. Set the final flag after a brief delay.
        yield return new WaitForSeconds(1f);
        StoryManagertAct1A.Instance.SetFlag("RunToCentralBB", true);

        // 6. --- NEW ---
        //    Finally, disable the entire GameObject.
        Debug.Log("Disabling blast object: " + gameObject.name);
        gameObject.SetActive(false);
    }
}
