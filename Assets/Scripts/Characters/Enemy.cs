using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // General
    private Stats stats;
    private SpriteRenderer spriteRenderer;
    private Player player;
    private EnemySpawner enemySpawner;

    // Specific stats
    [SerializeField] private int deathPrize; // How much points the player gets from killing this enemy
    [SerializeField] private int hitPrize; // How much points the player gets from damaging this enemy

    // Movement direction calculation
    Vector2 moveDirection = Vector2.zero;
    Vector2 lastPos;

    // Attacking
    private bool canAttack = true;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        stats = new Stats(100f, 100f, 1000f, 100f, 100f, 10f, 1f);
        player = FindObjectOfType<Player>();

        // Set pathfinding target
        GetComponent<Pathfinding.AIDestinationSetter>().target = player.transform;
    }

    private void Update()
    {
        // Calculate the movement direction
        if (lastPos != (Vector2)transform.position)
        {
            if (lastPos != null)
            {
                moveDirection = (Vector2)transform.position - lastPos;
            }

            lastPos = transform.position;
        }

        // Update sprite orientation depending on movement direction if the enemy is moving
        spriteRenderer.flipX = moveDirection.x < 0f;
    }

    public void ReceiveDamage(float amount, GameObject source)
    {
        // Reduce health until dead
        stats.health -= amount;

        if (source.tag == "Player")
        {
            // Give the player score for killing this enemy
            if (stats.health <= 0f)
            {
                source.GetComponent<Player>().AddCurrency(deathPrize);

                // deth
                enemySpawner.AddTally();
                Destroy(gameObject);
            }
            // Give the player score for damading this enemy
            else
            {
                source.GetComponent<Player>().AddCurrency(hitPrize);
            }

        }
    }

    // IEnumerator for implementing an attack delay between hits
    private IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the enemy is hitting the player
        if (collision.gameObject.tag == "Player")
        {
            // Damage the player with given intervals
            if (canAttack)
            {
                collision.gameObject.GetComponent<Player>().ReceiveDamage(stats.damage);
                canAttack = false;
                StartCoroutine(AttackDelay(stats.attackDelay));
            }
        }
    }
}
