using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField] private Transform turret;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private float range = 3f;
    [SerializeField] private float shotsPerSecond = 1.5f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int damage = 1;

    private float nextShotTime;

private void Update()
{
    if (turret == null || firePoint == null || projectilePrefab == null)
        return;

    EnemyHealth target = FindTarget();
    if (target == null) return;

    Vector3 velocity = Vector3.zero;
    EnemyMovement movement = target.GetComponent<EnemyMovement>();

    if (movement != null)
        velocity = movement.Velocity;

    Vector3 aim = PredictPosition(target.AimPoint, velocity);

    Vector3 facing = aim - turret.position;
    facing.y = 0f;

    if (facing.sqrMagnitude > 0.0001f)
        turret.rotation = Quaternion.LookRotation(facing);

    // Keep tracking above, but only fire when the cooldown ends.
    if (Time.time < nextShotTime) return;

    aim = PredictPosition(target.AimPoint, velocity);
    Vector3 direction = (aim - firePoint.position).normalized;

    Projectile shot = Instantiate(
        projectilePrefab,
        firePoint.position,
        Quaternion.LookRotation(direction)
    );

    shot.Launch(direction * projectileSpeed, damage);
    nextShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);
}

    private EnemyHealth FindTarget()
    {
        EnemyHealth closest = null;
        float closestDistance = range * range;

        foreach (EnemyHealth enemy in
                 FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            Vector3 offset = enemy.transform.position - transform.position;
            offset.y = 0f;

            if (offset.sqrMagnitude < closestDistance)
            {
                closestDistance = offset.sqrMagnitude;
                closest = enemy;
            }
        }

        return closest;
    }

    private Vector3 PredictPosition(Vector3 position, Vector3 velocity)
    {
        Vector3 predicted = position;
        float speed = Mathf.Max(0.1f, projectileSpeed);

        for (int i = 0; i < 4; i++)
        {
            float travelTime = Vector3.Distance(firePoint.position, predicted) / speed;
            predicted = position + velocity * travelTime;
        }

        return predicted;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}