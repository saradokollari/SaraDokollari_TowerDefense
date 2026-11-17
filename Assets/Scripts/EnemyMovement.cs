using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform waypointRoot;
    [SerializeField, Min(0.1f)] private float speed = 1.5f;

    private Rigidbody body;
    private Transform[] points;
    private int nextPoint;

    public Vector3 Velocity { get; private set; }

    public void SetPath(Transform root)
    {
        waypointRoot = root;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (waypointRoot == null || waypointRoot.childCount < 2)
        {
            Debug.LogError("Enemy needs a valid Waypoints container.", this);
            enabled = false;
            return;
        }

        points = new Transform[waypointRoot.childCount];

        for (int i = 0; i < points.Length; i++)
            points[i] = waypointRoot.GetChild(i);

        body.position = points[0].position;
        nextPoint = 1;
    }

    private void FixedUpdate()
    {
        if (nextPoint >= points.Length)
        {
            Velocity = Vector3.zero;

            if (WaveManager.Instance != null)
                WaveManager.Instance.EnemyEscaped();

            Destroy(gameObject);
            return;
        }

        Vector3 start = body.position;
        Vector3 position = start;
        float distanceLeft = speed * Time.fixedDeltaTime;

        while (distanceLeft > 0f && nextPoint < points.Length)
        {
            Vector3 target = points[nextPoint].position;
            float distance = Vector3.Distance(position, target);

            if (distance <= distanceLeft)
            {
                position = target;
                distanceLeft -= distance;
                nextPoint++;
            }
            else
            {
                position = Vector3.MoveTowards(position, target, distanceLeft);
                distanceLeft = 0f;
            }
        }

        Velocity = (position - start) / Time.fixedDeltaTime;
        body.MovePosition(position);

        Vector3 facing = new Vector3(Velocity.x, 0f, Velocity.z);

        if (facing.sqrMagnitude > 0.0001f)
            body.MoveRotation(Quaternion.LookRotation(facing));
    }
}