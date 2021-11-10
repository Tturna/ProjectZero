using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // General
    private Stats stats;
    private SpriteRenderer spriteRenderer;
    private SpriteMask spriteMask;
    private Player player;
    private EnemySpawner enemySpawner;

    // Specific stats
    [SerializeField] private int deathPrize; // How much points the player gets from killing this enemy
    [SerializeField] private int hitPrize; // How much points the player gets from damaging this enemy
    [SerializeField] float hitStretchHorizontal;
    [SerializeField] float hitStretchVertical;
    [SerializeField] float stretchTime;
    [SerializeField] Material hitFlashMaterial;
    [SerializeField] float hitFlashTime;

    // Movement direction calculation
    Vector2 moveDirection = Vector2.zero;
    Vector2 lastPos;

    // Attacking
    private bool canAttack = true;

    // Juice stuff
    Material originalMaterial;
    bool flashing;
    bool stretching;

    private void Start()
    {
        // Init
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteMask = GetComponentInChildren<SpriteMask>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        stats = new Stats(100f, 100f, 1000f, 100f, 100f, 10f, 1f);
        player = FindObjectOfType<Player>();

        originalMaterial = spriteRenderer.material;

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
        spriteRenderer.flipX = moveDirection.x < 0f && !flashing;

        // Update mask sprite
        spriteMask.sprite = spriteRenderer.sprite;
        if (!stretching)
        {
            spriteMask.transform.localScale = moveDirection.x < 0f ? new Vector3(-1, 1, 1) : Vector3.one;
        }
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

        // Squish and stretch
        Vector3 ogs = spriteRenderer.transform.localScale, ogm = spriteMask.transform.localScale;
        stretching = true;
        spriteRenderer.transform.localScale = Vector3.Scale(spriteRenderer.transform.localScale, new Vector3(1f + hitStretchHorizontal, 1f + hitStretchVertical, 0f));
        spriteMask.transform.localScale = Vector3.Scale(spriteMask.transform.localScale, new Vector3(1f + hitStretchHorizontal, 1f + hitStretchVertical, 0f));
        StartCoroutine(SquishAndStretch(ogs, ogm));

        // Flash white
        spriteRenderer.flipX = false; // Sprite can't be flipped when flashing, but the flipped mask will make it look like it is
        flashing = true; // Make sure the sprite doesn't flip during the flash
        spriteRenderer.material = hitFlashMaterial;
        StartCoroutine(ResetColor(hitFlashTime));
    }

    IEnumerator SquishAndStretch(Vector3 originalSprite, Vector3 originalMask)
    {
        float t = 0;
        while (spriteRenderer.transform.localScale != Vector3.one)
        {
            spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, originalSprite, t);
            spriteMask.transform.localScale = Vector3.Lerp(spriteMask.transform.localScale, originalMask, t);
            t += Time.deltaTime;
            if (t > stretchTime) t = stretchTime;

            yield return new WaitForEndOfFrame();
        }
        stretching = false;
    }

    IEnumerator ResetColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.material = originalMaterial;
        flashing = false;
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
