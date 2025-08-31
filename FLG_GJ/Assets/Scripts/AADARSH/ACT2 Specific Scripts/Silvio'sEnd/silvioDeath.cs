using UnityEngine;

public class silvioDeath : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("The radius of the circle used to detect the player.")]
    [SerializeField] private float detectionRadius = 3f;

    [Tooltip("The LayerMask used to identify the player. Set this to your 'Player' layer in the Inspector.")]
    [SerializeField] private LayerMask playerLayer;

    [Tooltip("The key the player must press to trigger the action.")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] NPC_A cantalk;

    // --- NEW ---
    // Add a variable to hold the Animator component.
    private Animator anim;

    // --- NEW ---
    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Get the Animator component attached to this same GameObject.
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Cast a circle to see if the player is within range.
        Collider2D detectedCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        // Check if we found a collider on the player layer.
        if (detectedCollider != null)
        {
            // The player is in range. Now, check if they press the interaction key.
            if (Input.GetKeyDown(interactionKey))
            {
                Debug.Log("Player pressed E! " + gameObject.name + " is dying.");
                cantalk.enabled = true;
                KillSilvio();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void KillSilvio()
    {
        // --- MODIFIED ---
        // Trigger the "Die" animation parameter we created.
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        StoryManagertAct1A.Instance.SetFlag("DeathTalk", true);
        gameObject.GetComponent<NPC_A>().TriggerDialouge();
        gameObject.GetComponent<silvioDeath>().enabled = false;
    }
}