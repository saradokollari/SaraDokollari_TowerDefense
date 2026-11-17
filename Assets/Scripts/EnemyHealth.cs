using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private int coinReward = 2;
    [SerializeField] private Vector3 aimOffset = new Vector3(0f, 0.2f, 0f);
    [SerializeField] private float hitFlashDuration = 0.15f;

    private int health;
    private bool defeated;

    private Renderer[] bodyRenderers;
    private MaterialPropertyBlock[] originalBlocks;
    private MaterialPropertyBlock[] hitBlocks;

    private bool flashing;
    private float flashEndsAt;

    public Vector3 AimPoint => transform.TransformPoint(aimOffset);

    private void Awake()
    {
        health = maxHealth;

        bodyRenderers = GetComponentsInChildren<Renderer>(true);
        originalBlocks = new MaterialPropertyBlock[bodyRenderers.Length];
        hitBlocks = new MaterialPropertyBlock[bodyRenderers.Length];

        Color hitColor = new Color(1f, 0.25f, 0.25f, 1f);

        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            originalBlocks[i] = new MaterialPropertyBlock();
            bodyRenderers[i].GetPropertyBlock(originalBlocks[i]);

            hitBlocks[i] = new MaterialPropertyBlock();
            bodyRenderers[i].GetPropertyBlock(hitBlocks[i]);

            // Support URP and standard material color properties.
            hitBlocks[i].SetColor("_BaseColor", hitColor);
            hitBlocks[i].SetColor("_Color", hitColor);
        }
    }

    private void Update()
    {
        if (flashing && Time.time >= flashEndsAt)
        {
            RestoreAppearance();
        }
    }

    public void TakeDamage(int damage)
    {
        if (defeated || damage <= 0) return;

        health -= damage;

        if (health <= 0)
        {
            defeated = true;

            if (TowerShop.Instance != null)
                TowerShop.Instance.AddCoins(coinReward);

            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            if (bodyRenderers[i] != null)
                bodyRenderers[i].SetPropertyBlock(hitBlocks[i]);
        }

        flashing = true;
        flashEndsAt = Time.time + hitFlashDuration;
    }

    private void RestoreAppearance()
    {
        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            if (bodyRenderers[i] != null)
                bodyRenderers[i].SetPropertyBlock(originalBlocks[i]);
        }

        flashing = false;
    }

    private void OnDisable()
    {
        if (flashing)
            RestoreAppearance();
    }
}