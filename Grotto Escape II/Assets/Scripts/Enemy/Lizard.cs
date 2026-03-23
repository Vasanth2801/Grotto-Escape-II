using System.Collections;
using UnityEngine;

public class Lizard : MonoBehaviour
{
    [Header("Movement for the Enemy")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private int facingDirection = 1;
    [SerializeField] private float bulletForce = 10f;

    [Header("Shoot Settings")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private bool isAttacking;
    [SerializeField] private float attackCoolDown;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform player;
    [SerializeField] private ObjectPooler pooler;
    [SerializeField] private Transform  firePoint;
    [SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pooler = FindAnyObjectByType<ObjectPooler>();
    }

    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceFromPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if(!isAttacking)
            {
                StartCoroutine(ShootCoroutine());
            }

            return;
        }
        else
        {
            rb.linearVelocity = new Vector2(speed, 0f);
        }

        if(player.position.x < transform.position.x && facingDirection == 1 || player.position.x > transform.position.x && facingDirection == -1)
        {
            Flip();
        }
    }


    IEnumerator ShootCoroutine()
    {
        isAttacking = true;
        
        while(true)
        {
            if(player == null)
            {
                break;
            }

            float distance = Vector2.Distance(player.position, transform.position);
            if(distance >  attackRange)
            {
                break;
            }

            Shoot();

            yield return new WaitForSeconds(attackCoolDown);
        }
        isAttacking = false;
    }

    void Shoot()
    {
        GameObject enemyBullet = pooler.SpawnFromPools("EnemyBullet", firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = enemyBullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        speed = -speed;
        FlipEnemy();
    }

    void FlipEnemy()
    {
        transform.localScale = new Vector2(Mathf.Sign(rb.linearVelocity.x), 1f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}