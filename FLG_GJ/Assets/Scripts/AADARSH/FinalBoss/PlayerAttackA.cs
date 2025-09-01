using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class PlayerAttackA : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float Range = 2f;
    [SerializeField] private KeyCode attackKey = KeyCode.Space;

    [Tooltip("At what point during the animation (0.0 to 1.0) should damage be applied?")]
    [Range(0, 1)]
    [SerializeField] private float damagePointNormalizedTime = 0.5f;

    [Header("Animation State Names")]
    [Tooltip("The exact name of the melee attack state or Blend Tree in the Animator.")]
    [SerializeField] private string attackStateName = "Melee";

    private Animator anim;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private bool dealtDamageThisAttack = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleAttackInput();

        if (isAttacking)
        {
            CheckAttackState();
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                anim.SetTrigger("Attack");
                isAttacking = true;
                dealtDamageThisAttack = false; // Reset damage flag for the new attack.
            }
        }
    }

    private void CheckAttackState()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(attackStateName))
        {
            if (stateInfo.normalizedTime >= damagePointNormalizedTime && !dealtDamageThisAttack)
            {
                DealDamageToBoss();
                dealtDamageThisAttack = true; // Set flag so we only deal damage once per attack.
            }

            if (stateInfo.normalizedTime >= 1.0f)
            {
                isAttacking = false;
            }
        }
        else if (!anim.IsInTransition(0))
        {
            // Safety Net to prevent getting stuck in an attack state.
            isAttacking = false;
        }
    }

    private void DealDamageToBoss()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Range);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Boss"))
            {
                Debug.Log("Hit the boss!");
                if (hit.TryGetComponent<StationaryBossA>(out StationaryBossA boss))
                {
                    boss.TakeDamage(damage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}