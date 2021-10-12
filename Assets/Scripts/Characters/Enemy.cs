using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // General
    private Stats stats;
    private SpriteRenderer spriteRenderer;
    private Player player;

    // Specific stats
    [SerializeField] private int scorePrize; // How much points the player gets from this enemy

    // Movement direction calculation
    Vector2 moveDirection = Vector2.zero;
    Vector2 lastPos;

    // Attacking
    private bool canAttack = true;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

        if (stats.health <= 0f)
        {
            // Give the player score for killing this enemy
            if (source.tag == "Player")
            {
                source.GetComponent<Player>().AddCurrency(scorePrize);
            }

            // deth
            Destroy(gameObject);
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
