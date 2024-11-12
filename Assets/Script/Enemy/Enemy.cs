using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.5f; // Tầm đánh cận chiến
    [SerializeField] private float attackCooldown = 1f; // Thời gian hồi chiêu giữa các đòn đánh
    public int attackDamage = 10;
    private Transform target;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Tìm đối tượng có tag "Tower" và gán làm target
        GameObject tower = GameObject.FindWithTag("Tower");
        if (tower != null)
        {
            target = tower.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy đối tượng nào với tag 'Tower'");
        }
    }

    private void Update()
    {
        if (target == null) return;

        // Kiểm tra khoảng cách tới mục tiêu
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Nếu trong tầm đánh, dừng di chuyển và thực hiện tấn công
        if (distanceToTarget <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("Run", false);

            // Thực hiện tấn công nếu đủ thời gian hồi chiêu
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                // Gọi phương thức gây sát thương ở đây nếu có
            }
        }
        else
        {
            // Nếu chưa đến gần, tiếp tục di chuyển
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetBool("Run", true);
        }

        // Kiểm tra hướng di chuyển và thay đổi flipX
        spriteRenderer.flipX = target.position.x < transform.position.x;
    }
    public void AttackTower()
    {
        isAttacking = true;
        TowerHealth towerHealth = target.GetComponent<TowerHealth>();
        if (towerHealth != null)
        {
            towerHealth.TakeDamage(attackDamage); // Gây damage lên tower
        }
        Invoke("ResetAttack", 1f); // Reset attack sau 1 giây
    }
}
