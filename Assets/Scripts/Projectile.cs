using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float lifetime = 3f;

    private Rigidbody2D rb;
    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float speed, int bulletDamage)
    {
        damage = bulletDamage;

        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the player.
        if (other.CompareTag("Player"))
            return;

        // Ignore collectibles so bullets pass through them.
        if (other.GetComponentInParent<Flower>() != null)
            return;

        // Break pots.
        Pot pot = other.GetComponentInParent<Pot>();

        if (pot != null)
        {
            pot.BreakPot();
            Destroy(gameObject);
            return;
        }

        // Damage enemies.
        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        FlyEnemy flyEnemy = other.GetComponentInParent<FlyEnemy>();

        if (flyEnemy != null)
        {
            flyEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // Destroy bullet when hitting other objects.
        Destroy(gameObject);
    }
}
