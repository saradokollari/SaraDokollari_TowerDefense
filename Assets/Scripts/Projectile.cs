using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 4f;

    private Rigidbody body;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = false;
        body.linearDamping = 0f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Launch(Vector3 velocity, int shotDamage)
    {
        damage = shotDamage;
        body.linearVelocity = velocity;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        EnemyHealth enemy = collision.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
            enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}